using Google.Apis.Auth.OAuth2;
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
        path = $"{Environment.CurrentDirectory}\\Tests\\";
    }
    public static void Initialize()
    {
        if (!_isInitialized)
        {
            GoogleAPI.LoadTestsToLocalFolder(path);
            _isInitialized = true;
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
            Debug.WriteLine("СПИСОК ПУСТ");
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