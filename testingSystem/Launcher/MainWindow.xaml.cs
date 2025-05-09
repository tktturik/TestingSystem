using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Launcher
{
    public partial class MainWindow : Window
    {
        private const string GitHubVersionUrl = "https://raw.githubusercontent.com/tktturik/Versions/main/versions.txt";
        private const string GitHubAppUrl = "https://github.com/tktturik/Versions/archive/refs/heads/main.zip";
        private const string LocalAppFolder = "C:\\testingSystem";
        private const string LocalAppExe = "testingSystem.exe";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckForUpdatesAsync();
            await CheckDirTestsAsync();
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                Version localVersion = GetLocalVersion();
                Version latestVersion = await GetLatestVersionAsync();
                Debug.WriteLine($"Версия на компьютере {localVersion.ToString()}");
                Debug.WriteLine($"Версия на github {latestVersion.ToString()}");
                if (latestVersion > localVersion)
                {
                    await DownloadAndUpdateAsync(latestVersion);
                }

                LaunchApp();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private async Task CheckDirTestsAsync()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string skyNetPath = Path.Combine(basePath, "SkyNetTS");
            string testsPath = Path.Combine(skyNetPath, "Tests");

            if (!Directory.Exists(skyNetPath))
            {
                Directory.CreateDirectory(skyNetPath);
            }

            if (!Directory.Exists(testsPath))
            {
                Directory.CreateDirectory(testsPath);
            }

            List<string> dirs = new List<string> { "Python", "ComputerLiteracy", "RobloxStudio", "Minecraft", "Unity" };
            foreach (string dir in dirs)
            {
                string dirPath = Path.Combine(testsPath, dir);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }
        }


        private Version GetLocalVersion()
        {
            string appPath = Path.Combine(LocalAppFolder, LocalAppExe);
            if (File.Exists(appPath))
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(appPath);
                return new Version(fileInfo.FileVersion);
            }
            return new Version("0.0.0.0");
        }

        private async Task<Version> GetLatestVersionAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string versionText = await client.GetStringAsync(GitHubVersionUrl);
                return new Version(versionText.Trim());
            }
        }

        private async Task DownloadAndUpdateAsync(Version version)
        {
            Dispatcher.Invoke(() =>
            {
                DownloadProgressBar.IsIndeterminate = true;
                ProgressText.Text = "Скачивание...";
            });

            string downloadUrl = string.Format(GitHubAppUrl, version);
            string tempZipPath = Path.GetTempFileName();

            using (HttpClient client = new HttpClient())
            {
                await DownloadFileWithProgressAsync(client, downloadUrl, tempZipPath);
            }

            if (Directory.Exists(LocalAppFolder))
            {
                Directory.Delete(LocalAppFolder, true);
            }

            Directory.CreateDirectory(LocalAppFolder);

            string tempExtractPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempExtractPath);
            System.IO.Compression.ZipFile.ExtractToDirectory(tempZipPath, tempExtractPath);

            string nestedFolder = Path.Combine(tempExtractPath, "Versions-main");
            if (Directory.Exists(nestedFolder))
            {
                foreach (var file in Directory.GetFiles(nestedFolder))
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(LocalAppFolder, fileName);
                    File.Move(file, destFile);
                }

                foreach (var dir in Directory.GetDirectories(nestedFolder))
                {
                    string dirName = Path.GetFileName(dir);
                    string destDir = Path.Combine(LocalAppFolder, dirName);
                    Directory.Move(dir, destDir);
                }
            }

            Directory.Delete(tempExtractPath, true);
            File.Delete(tempZipPath);

            Dispatcher.Invoke(() =>
            {
                DownloadProgressBar.IsIndeterminate = false;
                ProgressText.Text = "Завершено!";
            });
        }

        private async Task DownloadFileWithProgressAsync(HttpClient client, string url, string destinationPath)
        {
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
            }
        }

        private void LaunchApp()
        {
            string appPath = Path.Combine(LocalAppFolder, LocalAppExe);
            if (File.Exists(appPath))
            {
                Process.Start(appPath);
                Application.Current.Shutdown(); // Закрываем лаунчер
            }
            else
            {
                MessageBox.Show("Основное приложение не найдено.");
            }
        }
    }
}