using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Utilities;

public class DataService
{
    private static string path;

    private static bool _isInitialized;

    static DataService()
    {
        InitializeConfiguration();
    }
    private static void InitializeConfiguration()
    {
        
        path = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Tests\\";
        if(!Directory.Exists(path))
        {
           Directory.CreateDirectory(path);
        }
    }
    public static void Initialize()
    {
        if (!_isInitialized)
        {
            InitializeConfiguration();
            GoogleAPI.InitializeDriveService();
            GoogleAPI.RefreshAccessTokenAsync();
            try
            {

                GoogleAPI.LoadTestsToLocalFolder(path);
                _isInitialized = true;
            }
            catch (TokenResponseException ex)
            {
                MessageBox.Show($"Ошибка аутентификации: {ex.Message}");
                Debug.WriteLine(" ");
                Debug.WriteLine($"Ошибка аутентификации: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
                Debug.WriteLine(" ");
                Debug.WriteLine($"Непредвиденная ошибка: {ex.Message}");
            }
        }
    }

    public static void SaveQuestions(Test test)
    {
        string json = JsonConvert.SerializeObject(test, Newtonsoft.Json.Formatting.Indented);
        string filePath = Path.Combine(path, test.Title + ".json");
        System.IO.File.WriteAllText(filePath, json);
    }


    public static ObservableCollection<Test> LoadTestsFromFolder()
    {
        ObservableCollection<Test> tests = new ObservableCollection<Test>();

        var files = Directory.GetFiles(path, "*.json");

        foreach (var filePath in files)
        {
            string json = System.IO.File.ReadAllText(filePath);

            Test test = JsonConvert.DeserializeObject<Test>(json);

            tests.Add(test);
        }

        if (tests.Count == 0)
        {
            Debug.WriteLine("СПИСОК ПУСТ ФФФ");
        }
        else
        {
            Debug.WriteLine($"{tests.Count} tests");
        }

        return tests;
    }

    public static bool RemoveTest(Test test)
    {
        string filePath = Path.Combine(path, test.Title + ".json");
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            return true;
        }
        return false;
    }

    public static void SyncFiles()
    {
        var a = GoogleAPI.SyncLocalFilesWithGoogleDrive(path);
    }


}