using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WpfApp1.Models;

namespace WpfApp1.Utilities
{
    public static class GoogleAPI
    {
        private static string idFolder;
        private static DriveService _driveService;
        private static string nameApiKey;

        static GoogleAPI()
        {
            InitializeConfiguration();
            InitializeDriveService();
        }
        private static void InitializeConfiguration()
        {
            idFolder = "1ZsvGd8t9DMTyCwY8QrOWoLCbja5S6Yv-";
            nameApiKey = "credentials.json";
        }

        private static void InitializeDriveService()
        {
            UserCredential credential;
            using (var stream = new FileStream(nameApiKey, FileMode.Open, FileAccess.Read))
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
        public static void LoadTestsToLocalFolder(string path)
        {

            var request = _driveService.Files.List();
            request.Q = $"'{idFolder}' in parents and mimeType='application/json'";
            var files = request.Execute().Files;

            if (files.Count == 0)
            {
                Debug.WriteLine("СПИСОК ПУСТ");
                return;
            }

            foreach (var file in files)
            {
                DownloadFileToLocalFolderAsync(file.Id, file.Name, path);
            }

            Debug.WriteLine($"{files.Count} tests загружено на локальный диск.");
        }

        private static void DownloadFileToLocalFolderAsync(string fileId, string fileName, string path)
        {
            var request = _driveService.Files.Get(fileId);
            var filePath = System.IO.Path.Combine(path, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                Debug.WriteLine($"Файл {fileName} уже существует и будет перезаписан.");
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                request.Download(fileStream);
            }

            Debug.WriteLine($"Файл {fileName} загружен на локальный диск.");
        }

        public static async void UploadQuestionsAsync(Test questions, string fileName)
        {
            string json = JsonConvert.SerializeObject(questions, Newtonsoft.Json.Formatting.Indented);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string> { idFolder }
            };

            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

            var request = _driveService.Files.Create(fileMetadata, stream, "application/json");


            var file = await request.UploadAsync();

            if (file.Status == Google.Apis.Upload.UploadStatus.Completed)
            {
                Debug.WriteLine("Файл успешно загружен.");
            }
            else
            {
                Debug.WriteLine("Ошибка при загрузке файла.");
            }
        }
        private static async Task<List<Google.Apis.Drive.v3.Data.File>> GetFilesInFolder(string folderId)
        {

            var request = _driveService.Files.List();
            request.Q = $"'{folderId}' in parents and trashed=false";
            request.Fields = "files(id, name, modifiedTime)";
            var response = await request.ExecuteAsync();
            var files = response.Files;
            return files.ToList();
        }
        private static async Task UpdateFileOnGoogleDriveAsync(string fileId, string localFilePath)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Id = fileId
            };

            using (var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
            {
                var request = _driveService.Files.Update(fileMetadata, fileId, stream, "application/json");
                await request.UploadAsync();
            }

            Debug.WriteLine($"Файл {fileId} обновлен на Google Диске.");
        }
        private static async Task CreateFileOnGoogleDriveAsync(string localFilePath, string folderId, string fileName)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            using (var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
            {
                var request = _driveService.Files.Create(fileMetadata, stream, "application/json");
                await request.UploadAsync();
            }

            Debug.WriteLine($"Файл {fileName} загружен на Google Диске.");
        }
        private static async Task DeleteFileAsync(string fileId)
        {
            await _driveService.Files.Delete(fileId).ExecuteAsync();
            Debug.WriteLine($"Файл {fileId} удален с Google Диска.");
        }
        public static async Task SyncLocalFilesWithGoogleDrive(string localFolderPath)
        {

            var googleFiles = await GetFilesInFolder(idFolder);
            var localFiles = Directory.GetFiles(localFolderPath, "*.json");

            foreach (var localFilePath in localFiles)
            {
                string fileName = System.IO.Path.GetFileName(localFilePath);
                var googleFile = googleFiles.FirstOrDefault(f => f.Name == fileName);

                if (googleFile != null)
                {
                    if (IsFileModified(localFilePath, googleFile.ModifiedTimeDateTimeOffset.GetValueOrDefault()))
                    {
                        await UpdateFileOnGoogleDriveAsync(googleFile.Id, localFilePath);
                    }
                }
                else
                {
                    await CreateFileOnGoogleDriveAsync(localFilePath, idFolder, fileName);
                }
            }

            foreach (var googleFile in googleFiles)
            {
                if (!localFiles.Any(f => System.IO.Path.GetFileName(f) == googleFile.Name))
                {
                    await DeleteFileAsync(googleFile.Id);
                }
            }

            Debug.WriteLine("Синхронизация завершена.");
        }




        private static bool IsFileModified(string localFilePath, DateTimeOffset googleModifiedTime)
        {
            var localModifiedTime = File.GetLastWriteTimeUtc(localFilePath);
            return localModifiedTime > googleModifiedTime.UtcDateTime;
        }

    }
}

