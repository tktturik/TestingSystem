using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Utilities
{
    public static class GoogleAPI
    {
        private static string mainIdFolder;
        private static DriveService _driveService;
        private static string nameApiKey;

        public static void Initialize()
        {
            InitializeConfiguration();
            InitializeDriveService();
            RefreshAccessTokenAsync();
        }

        private static void InitializeConfiguration()
        {
            mainIdFolder = "1ZsvGd8t9DMTyCwY8QrOWoLCbja5S6Yv-";
            nameApiKey = "credentials.json";
        }

        private static void InitializeDriveService()
        {
            UserCredential credential;
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolderPath = System.IO.Path.Combine(appDataPath, "SkyNetTS");

            string credPath = System.IO.Path.Combine(appFolderPath, "credentials.json");
            string tokenPath = System.IO.Path.Combine(appFolderPath, "token");

            try
            {
                if (!File.Exists(credPath))
                {
                    MessageBox.Show($"Файл credentials.json не найден по пути: {credPath}");
                }

                if (!Directory.Exists(tokenPath))
                {
                    MessageBox.Show($"Директория token не найдена по пути: {tokenPath}");
                }

                using (var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { DriveService.Scope.Drive },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(tokenPath, true)).Result;
                }

                _driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "TestingSystem"
                });
            }
            catch (TokenResponseException ex)
            {
                DeleteToken(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SkyNetTS", "token"));
                InitializeDriveService();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
            }
        }

        public static async Task RefreshAccessTokenAsync()
        {
            if (_driveService == null)
            {
                InitializeDriveService();
            }

            var credential = _driveService.HttpClientInitializer as UserCredential;
            if (credential != null)
            {
                try
                {
                    var refreshedToken = await credential.GetAccessTokenForRequestAsync();
                    if (refreshedToken != null)
                    {
                        Console.WriteLine("Токен доступа успешно обновлен.");
                    }
                    else
                    {
                        Console.WriteLine("Не удалось обновить токен доступа.");
                    }
                }
                catch (TokenResponseException ex)
                {
                    Console.WriteLine($"Ошибка при обновлении токена: {ex.Message}");
                }
            }
        }

        public static DriveService GetDriveService()
        {
            if (_driveService == null)
            {
                InitializeDriveService();
            }
            return _driveService;
        }

        public static void LoadDirFromGoogleDrive(string path)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{mainIdFolder}' in parents and mimeType='application/vnd.google-apps.folder'";

            try
            {
                var files = request.Execute().Files;

                if (files.Count == 0)
                {
                    Debug.WriteLine("СПИСОК ПУСТ DDDD");
                    return;
                }
                else
                {
                    foreach (var folder in files)
                    {
                        string pathFolder = System.IO.Path.Combine(path, folder.Name);
                        LoadTestsToLocalFolder(pathFolder, folder.Id);
                    }
                }
            }
            catch (TokenResponseException ex)
            {
                
                    DeleteToken(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SkyNetTS", "token"));
                    InitializeDriveService();
                    LoadDirFromGoogleDrive(path);
             
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
            }
        }

        private static void DeleteToken(string tokenPath)
        {
            if (File.Exists(tokenPath))
            {
                File.Delete(Path.Combine(tokenPath, "*.TokenResponse-user"));
            }
        }

        public static void LoadTestsToLocalFolder(string path, string idFolder)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{idFolder}' in parents and mimeType='application/json'";
            var files = request.Execute().Files;

            if (files.Count == 0)
            {
                Debug.WriteLine("СПИСОК ПУСТ DDDD");
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

        public static async Task SyncLocalFilesWithGoogleDrive(string localFolderPath)
        {
            var googleFolders = await GetFoldersInFolder(mainIdFolder);
            var localFolders = Directory.GetDirectories(localFolderPath);

            foreach (var localFolder in localFolders)
            {
                var folderName = new DirectoryInfo(localFolder).Name;
                var googleFolder = googleFolders.FirstOrDefault(f => f.Name == folderName);

                if (googleFolder != null)
                {
                    await SyncFilesInFolder(googleFolder.Id, localFolder);
                }
            }
            MessageBox.Show("Синхронизация завершена.");
        }

        private static async Task SyncFilesInFolder(string folderId, string localFolderPath)
        {
            var googleFiles = await GetFilesInFolder(folderId);
            string[] localFiles = Directory.GetFiles(localFolderPath, "*.json");

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
                    await CreateFileOnGoogleDriveAsync(localFilePath, folderId, fileName);
                }
            }

            foreach (var googleFile in googleFiles)
            {
                if (!localFiles.Any(f => System.IO.Path.GetFileName(f) == googleFile.Name))
                {
                    await DeleteFileAsync(googleFile.Id);
                }
            }
        }

        private static async Task<List<Google.Apis.Drive.v3.Data.File>> GetFoldersInFolder(string folderId)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{folderId}' in parents and mimeType='application/vnd.google-apps.folder'";
            request.Fields = "files(id, name)";
            var response = await request.ExecuteAsync();
            return response.Files.ToList();
        }

        private static async Task<List<Google.Apis.Drive.v3.Data.File>> GetFilesInFolder(string folderId)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{folderId}' in parents and trashed=false";
            request.Fields = "files(id, name, modifiedTime)";
            var response = await request.ExecuteAsync();
            return response.Files.ToList();
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

        private static bool IsFileModified(string localFilePath, DateTimeOffset googleModifiedTime)
        {
            var localModifiedTime = File.GetLastWriteTimeUtc(localFilePath);
            return localModifiedTime > googleModifiedTime.UtcDateTime;
        }
    }
}