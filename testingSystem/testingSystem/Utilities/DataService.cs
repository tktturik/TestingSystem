using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using testingSystem.Models;
using testingSystem.Models;
using testingSystem.Utilities;

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
        
        path = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\SkyNetTS\\Tests";
        if(!Directory.Exists(path))
        {
           Directory.CreateDirectory(path);
        }
    }
    public static void LoadFiles()
    {
        GoogleAPI.LoadDirFromGoogleDrive(path);
    }
    public static async Task Initialize()
    {
        if (!_isInitialized)
        {
            InitializeConfiguration();
            await GoogleAPI.Initialize();
            GoogleAPI.LoadDirFromGoogleDrive(path);

            _isInitialized = true;
        }
    }

    public  static Task SaveTest(Test test, string secondPartPath)
    {
        return Task.Run(() =>
        {
            string json = JsonConvert.SerializeObject(test, Newtonsoft.Json.Formatting.Indented);
            string filePath = Path.Combine(path, secondPartPath);
            File.WriteAllText(filePath, json);

            Debug.WriteLine($"Тест {test.Title} сохранен по пути {filePath}");
        });
    }


    public static ObservableCollection<Test> LoadTestsFromFolder(string folder)
    {
        ObservableCollection<Test> tests = new ObservableCollection<Test>();
        string dirPath = Path.Combine(path, folder);
        string[] files = Directory.GetFiles(dirPath, "*.json");
        //MessageBox.Show($"По пути {dirPath} находится {files.Length}");
        try
        {
            foreach (string filePath in files)
            {
                string json = File.ReadAllText(filePath);

                Test test = JsonConverter(json);

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
        }catch(IOException e)
        {
            MessageBox.Show("Повторите попытку чуть позже, тесты загружаются");
            return null;
        }

    }
    public static Test JsonConverter(string json)
    {

        return JsonConvert.DeserializeObject<Test>(json);
    }


    public static bool RemoveTest(Test test, string secondPartPath)
    {
        string filePath = Path.Combine(path, secondPartPath);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        return false;
    }

    public static void SyncFiles()
    {
        var a = GoogleAPI.SyncLocalFilesWithGoogleDrive(path);
    }
    public static bool FileExists(string secondPartPath)
    {
        return File.Exists(Path.Combine(path, secondPartPath));
    }
    public static void DeleteTempFile()
    {
        var files = Directory.GetFiles(path, "temp*");
        foreach (var file in files) 
        {
            File.Delete(file);
        }
    }

    public static void SerializeAttemps(Attemps attemps)
    {
        Task.Run(() =>
        {
            string json = JsonConvert.SerializeObject(attemps, Newtonsoft.Json.Formatting.Indented);
            string filePath = Path.Combine(path, "attemps.json");
            File.WriteAllText(filePath, json);
        });    
    }
    public static Attemps DeserializeAttemps()
    {
        try
        {
            string filePath = Path.Combine(path, "attemps.json");
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Attemps>(json);
        }
        catch (FileNotFoundException ex)
        {
            Attemps attemps = new Attemps() { CountAttemps = 3, LastUpdate = DateTime.Now };
            SerializeAttemps(attemps);
            return attemps;
        }
    }

}