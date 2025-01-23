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
using testingSystem.Utilities;

/// <summary>
/// Класс отвечающий за работу с данными
/// path - путь к директории с файлами
/// 
/// </summary>
public class DataService
{
    private static string path; 

    private static bool _isInitialized;

    static DataService()
    {
        InitializeConfiguration();
    }
    /// <summary>
    /// Инициализация конфигурации класса, создает директорию по path, если она не существует
    /// </summary>
    private static void InitializeConfiguration()
    {
        path = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\SkyNetTS\\Tests";
        if(!Directory.Exists(path))
        {
           Directory.CreateDirectory(path);
        }
    }
    /// <summary>
    /// Запуск скачивания файлов с GoogleDrive
    /// </summary>
    public static async Task LoadFiles()
    {
        await GoogleAPI.LoadDirFromGoogleDrive(path);
    }
    /// <summary>
    /// Инициализация GoogleAPI
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Сохранение теста, он сериализуется и находится по path
    /// </summary>
    /// <param name="test">Тест, который будет сериализован</param>
    /// <param name="secondPartPath">Имя файла</param>
    /// <returns></returns>
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

    /// <summary>
    /// Загрузка тестов с папки, все тесты с директории path десериализуются и добавляются в коллекцию, которая возвращается
    /// </summary>
    /// <param name="folder"></param>
    /// <returns>Коллекцию тестов</returns>
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

            return tests;
        }catch(IOException e)
        {
            MessageBox.Show("Повторите попытку чуть позже, тесты загружаются");
            return null;
        }

    }
    /// <summary>
    /// Десериализация теста
    /// </summary>
    /// <param name="json"></param>
    /// <returns>Десериализованный объект Test</returns>
    public static Test JsonConverter(string json)
    {
        return JsonConvert.DeserializeObject<Test>(json);
    }

    /// <summary>
    /// Удаляет тест
    /// </summary>
    /// <param name="test"></param>
    /// <param name="secondPartPath"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Запуск синхранизации файлов текущего компьютера с GoogleDrive
    /// </summary>
    public static void SyncFiles()
    {
        var a = GoogleAPI.SyncLocalFilesWithGoogleDrive(path);
    }
    /// <summary>
    /// Проверка существования файла
    /// </summary>
    /// <param name="secondPartPath"></param>
    /// <returns>True, если файл существует
    /// False, если нет</returns>
    public static bool FileExists(string secondPartPath)
    {
        return File.Exists(Path.Combine(path, secondPartPath));
    }
    /// <summary>
    /// Удаление всех временных файлов
    /// </summary>
    public static void DeleteTempFile()
    {
        var files = Directory.GetFiles(path, "temp*");
        foreach (var file in files) 
        {
            File.Delete(file);
        }
    }
    /// <summary>
    /// Сериализация объекта попыток
    /// </summary>
    /// <param name="attemps"></param>
    public static void SerializeAttemps(Attemps attemps)
    {
        Task.Run(() =>
        {
            string json = JsonConvert.SerializeObject(attemps, Newtonsoft.Json.Formatting.Indented);
            string filePath = Path.Combine(path, "attemps.json");
            File.WriteAllText(filePath, json);
        });    
    }
    /// <summary>
    /// Десериазация файла в объект попыток
    /// </summary>
    /// <returns></returns>
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
            Attemps attemps = new Attemps() { CountAttemps = 1, LastUpdate = DateTime.Now };
            SerializeAttemps(attemps);
            return attemps;
        }
    }
    public static void UpdateVersrionFile(Version version)
    {
        File.WriteAllText("versions.txt",version.ToString());
    }

}