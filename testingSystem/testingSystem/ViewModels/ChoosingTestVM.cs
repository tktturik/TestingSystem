using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testingSystem.Utilities;
using testingSystem.Models;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Configuration;
using testingSystem.ViewModels;

namespace testingSystem.ViewModels
{
    /// <summary>
    /// VM Отвечающая за поведение View - ChosingTest(Выбора тестов)
    /// </summary>
    internal class ChoosingTestVM : ViewModelBase
    {

        private Test selectedTest;
        private ObservableCollection<Test> tests { get; set; }
        //private int attemps = int.Parse(ConfigurationManager.AppSettings.Get("AttemptsAvailable"));
        private Attemps attemps;
        private string curTab;
        //private DateTime timeToUpdateAttmeps = DateTime.Parse(ConfigurationManager.AppSettings.Get("LastResetTime"));
        
        //public DateTime TimeToUpdateAttmeps
        //{
        //    get
        //    {
        //        return timeToUpdateAttmeps;
        //    }
        //    set
        //    {
        //        timeToUpdateAttmeps = value;
        //        OnPropertyChanged();
        //    }
        //}
        public int Attemps
        {
            get
            {
                return attemps.CountAttemps;
            }
            set
            {
                attemps.CountAttemps = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Открытие выбранного теста в ListView
        /// </summary>
        public Test SelectedTest
        {
            get { return selectedTest; }
            set
            {
                selectedTest = value;
                OnPropertyChanged();
                if (selectedTest != null && Attemps>0)
                {

                    WhoISVM whoISVM = new WhoISVM();
                    whoISVM.SetNavigationVM(navigation);
                    whoISVM.Test = new TakingTestVM() {Test = selectedTest};
                    navigation.SetUserControl(whoISVM);

                    //    TakingTestVM takingTestVM = new TakingTestVM();
                    //    takingTestVM.SetTest(selectedTest);
                    //    takingTestVM.SetNavigationVM(navigation);
                    //    _navigationVM.ShowSelectedTest(takingTestVM);
                }
                else
                {
                    MessageBox.Show("Попытки на сегодня закончились");
                }
            
            }
        }
        public ICommand EditTestCommand { get; }
        public ICommand DeleteTestCommand { get; }
        public ICommand SyncFilesCommand { get; }
        public ICommand SelectTabCommand { get; }
        public ICommand DownloadFilesCommand { get; }
        public ICommand UpdateFilesCommand { get; }
        
        /// <summary>
        /// Вызов процесса загрузки файлов, тестов с GoogleDrive
        /// </summary>
        /// <param name="parametr"></param>
        private async void DownloadFiles(object parametr)
        {
            await DataService.LoadFiles();
            Tests = DataService.LoadTestsFromFolder(curTab);
            OnPropertyChanged();
        }
        /// <summary>
        /// Вызов синхранизации тестов
        /// </summary>
        /// <param name="parametr"></param>
        private void SyncFiles(object parametr)
        {
            DataService.SyncFiles();
        }
        /// <summary>
        /// Загрузка тестов определенной вкладки, parametr - параметр, передающийся с View
        /// </summary>
        /// <param name="parametr"></param>
        private void SelectTab(object parametr)
        {
            curTab = Convert.ToString(parametr);

            Tests = DataService.LoadTestsFromFolder(curTab);
        }
        /// <summary>
        /// Открытие UserControl CreateTest, где будет тест, который нужен отредактировать
        /// </summary>
        /// <param name="parametr"></param>
        private void EditTest(object parametr)
        {
            if (parametr is Test testToUpdate)
            {
                CreateTestVM createTestVM = new CreateTestVM();
                createTestVM.Test = testToUpdate;
                navigation.SetUserControl(createTestVM);
            }

        }
        /// <summary>
        /// Удаление теста с коллекции тестов и с компьютера
        /// </summary>
        /// <param name="parametr"></param>
        private void DeleteTest(object parametr)
        {
            if (parametr is Test testToDelete)
            {
                Tests.Remove(testToDelete);
                DataService.RemoveTest(testToDelete,Path.Combine(testToDelete.Section,$"{testToDelete.Title}.json"));
            }
        }
        public ObservableCollection<Test> Tests
        {
            get { return tests; }
            set
            {
                tests = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Конструктор, где десериализуются файл попыток на тест и инициализурются команды и Тесты папки Python
        /// </summary>
        public ChoosingTestVM()
        {
            attemps = DataService.DeserializeAttemps();
            UpdateAttemps();

            try
            {
                curTab = "Python";
                Tests = DataService.LoadTestsFromFolder(curTab);
                EditTestCommand = new RelayCommand(EditTest);
                DeleteTestCommand = new RelayCommand(DeleteTest);
                SyncFilesCommand = new RelayCommand(SyncFiles);
                SelectTabCommand = new RelayCommand(SelectTab);
                DownloadFilesCommand = new RelayCommand(DownloadFiles);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            
        }

       
        /// <summary>
        /// Вызов обновления попыток, если прошло больше часа с последнего обновления
        /// </summary>
        private void UpdateAttemps()
        {
            if(DateTime.Now > attemps.LastUpdate.AddHours(1))
            {
                attemps.UpdateAttemps();
            }
        }
//        private void SetAttmepsAndResetTime()
//        {

//            ConfigurationManager.AppSettings.Set("AttemptsAvailable", 3.ToString());
//            ConfigurationManager.AppSettings.Set("LastTesetTime", DateTime.Now.ToString());

//            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
//            config.AppSettings.Settings["AttemptsAvailable"].Value = 3.ToString();
//            config.AppSettings.Settings["LastResetTime"].Value = DateTime.Now.ToString();
//            config.Save(ConfigurationSaveMode.Modified);
//            ConfigurationManager.RefreshSection("appSettings");

//            Attemps = int.Parse(ConfigurationManager.AppSettings.Get("AttemptsAvailable"));
//        }
    }
}   
