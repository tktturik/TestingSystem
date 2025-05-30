﻿using Google.Apis.Auth.OAuth2;
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

namespace testingSystem.Utilities
{
    public static class GoogleAPI
    {
        private static string mainIdFolder;
        private static DriveService _driveService;
        private static string nameApiKey;

        public static async Task Initialize()
        {
            await InitializeConfiguration();
            await InitializeDriveService();
        }

        private static async Task InitializeConfiguration()
        {
            mainIdFolder = "1ZsvGd8t9DMTyCwY8QrOWoLCbja5S6Yv-";
            nameApiKey = "credentials.json";
        }

        private static async Task InitializeDriveService()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolderPath = System.IO.Path.Combine(appDataPath, "SkyNetTS");
            string credPath = System.IO.Path.Combine(appFolderPath, nameApiKey);

            try
            {
                if (!File.Exists(credPath))
                {
                    MessageBox.Show($"Файл credentials.json не найден по пути: {credPath}");
                }

                GoogleCredential credential;
                using (var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.Scope.Drive);
                }

                _driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "TestingSystem"
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при инициализации DriveService: {ex.Message}", ex);
            }
        }

        public static async Task LoadDirFromGoogleDrive(string path)
        {
            var request = _driveService.Files.List();
            

            request.Q = $"'{mainIdFolder}' in parents and mimeType='application/vnd.google-apps.folder'";

            
                var files = await request.ExecuteAsync();

                if (files.Files.Count == 0)
                {
                    Debug.WriteLine("Папок нет");
                    Debug.WriteLine("СПИСОК ПУСТ DDDD");
                    return;
                }
                else
                {
                    foreach (var folder in files.Files)
                    {
                        string pathFolder = System.IO.Path.Combine(path, folder.Name);
                        await LoadTestsToLocalFolder(pathFolder, folder.Id);
                    }
                }
            
            //catch (TokenResponseException ex)
            //{
            //    await InitializeDriveService();
            //    await LoadDirFromGoogleDrive(path);
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine($"Непредвиденная ошибка: {ex.Message}");
            //    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
            //}
        }

        public static async Task LoadTestsToLocalFolder(string path, string idFolder)
        {
            var request = _driveService.Files.List();
            request.Q = $"'{idFolder}' in parents and mimeType='application/json'";
            request.Fields = "files(id, name, modifiedTime)";
            var files = await request.ExecuteAsync();

            if (files.Files.Count == 0)
            {
                Debug.WriteLine("СПИСОК ПУСТ DDDD");
                return;
            }

            foreach (var file in files.Files)
            {
                string localFilePath = System.IO.Path.Combine(path, file.Name);

                if (!System.IO.File.Exists(localFilePath) || IsFileModified(localFilePath, file.ModifiedTimeDateTimeOffset.GetValueOrDefault()))
                {
                    Debug.WriteLine($"{localFilePath}");
                    Debug.WriteLine($"{file.Name} отредактированный? {IsFileModified(localFilePath, file.ModifiedTimeDateTimeOffset.GetValueOrDefault())} время{file.ModifiedTimeDateTimeOffset.GetValueOrDefault()}");
                    await DownloadFileToLocalFolderAsync(file.Id, file.Name, path);
                }
            }

            Debug.WriteLine($"{files.Files.Count} tests загружено на локальный диск.");
        }

        private static async Task DownloadFileToLocalFolderAsync(string fileId, string fileName, string path)
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
                await request.DownloadAsync(fileStream);
            }
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Length == 0)
            {
                Debug.WriteLine($"Файл {fileInfo.Name} был загружен пустой");
                await DownloadFileToLocalFolderAsync(fileId, fileName, path);
            }
            Debug.WriteLine($"Файл {fileName} загружен на локальный диск.");
        }

        public static async Task SyncLocalFilesWithGoogleDrive(string localFolderPath)
        {
            Debug.WriteLine("AAAAAAAAAAA");
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
            DateTime localModifiedTime = File.GetLastWriteTimeUtc(localFilePath);
            Debug.WriteLine($"файл {localFilePath} был изменен {File.GetLastWriteTimeUtc(localFilePath)}");
            return localModifiedTime < googleModifiedTime.UtcDateTime;
        }
    }
}