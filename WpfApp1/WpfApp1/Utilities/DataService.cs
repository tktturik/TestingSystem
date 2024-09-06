using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WpfApp1.Models;

public class DataService
{
    private static DriveService _driveService;

    static DataService()
    {
        InitializeDriveService();
    }

    private static void InitializeDriveService()
    {
        UserCredential credential;
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { DriveService.Scope.Drive },
                "medvedshura1@gmail.com",
                CancellationToken.None).Result;
        }

        _driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "YourApplicationName"
        });
    }

    public static Test LoadQuestions(string fileId)
    {
        var request = _driveService.Files.Get(fileId);
        using (var stream = new MemoryStream())
        {
            request.Download(stream);
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<Test>(json);
            }
        }
    }

    public static void SaveQuestions(Test questions, string fileId)
    {
        string json = JsonConvert.SerializeObject(questions, Newtonsoft.Json.Formatting.Indented);
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = "questions.json",
            Id = fileId
        };
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
        var request = _driveService.Files.Update(fileMetadata, fileId, stream, "application/json");
        request.Upload();
    }

    public static ObservableCollection<Test> LoadTests(string folderId)
    {
        ObservableCollection<Test> tests = new ObservableCollection<Test>();
        var request = _driveService.Files.List();
        request.Q = $"'{folderId}' in parents and mimeType='application/json'";
        var files = request.Execute().Files;

        foreach (var file in files)
        {
            tests.Add(LoadQuestions(file.Id));
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

    public static bool RemoveTest(string fileId)
    {
        try
        {
            _driveService.Files.Delete(fileId).Execute();
            return true;
        }
        catch
        {
            return false;
        }
    }
}