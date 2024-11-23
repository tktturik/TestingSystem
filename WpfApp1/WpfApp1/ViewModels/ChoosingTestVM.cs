using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Utilities;
using WpfApp1.Models;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace WpfApp1.ViewModels
{
    internal class ChoosingTestVM : ViewModelBase
    {

        private NavigationVM _navigationVM;
        private Test selectedTest;
        private ObservableCollection<Test> tests { get; set; }
        public NavigationVM navigation
        {
            get { return _navigationVM; }
            set
            {
                _navigationVM = value;
                OnPropertyChanged();
            }
        }
        public Test SelectedTest
        {
            get { return selectedTest; }
            set
            {
                selectedTest = value;
                OnPropertyChanged();
                if (selectedTest != null)
                {
                    TakingTestVM takingTestVM = new TakingTestVM();
                    takingTestVM.SetTest(selectedTest);
                    _navigationVM.ShowSelectedTest(takingTestVM);
                }
            }
        }
        public ICommand EditTestCommand { get; }
        public ICommand DeleteTestCommand { get; }
        public ICommand SyncFilesCommand { get; }
        public ICommand SelectTabCommand { get; }
        public ICommand DownloadFilesCommand { get; }
        public ICommand UpdateFilesCommand { get; }
        

        private void DownloadFiles(object parametr)
        {
            DataService.LoadFiles();
        }
        private void SyncFiles(object parametr)
        {

            DataService.SyncFiles();
        }
        private void SelectTab(object parametr)
        {
            string folder = Convert.ToString(parametr);

            Tests = DataService.LoadTestsFromFolder(folder);
        }

        private void EditTest(object parametr)
        {
            if (parametr is Test testToUpdate)
            {
                Debug.WriteLine("check");

                CreateTestVM createTestVM = new CreateTestVM();
                createTestVM.Test = testToUpdate;
                _navigationVM.ShowSelectedTest(createTestVM);
            }

        }
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
        public ChoosingTestVM()
        {

            Tests = DataService.LoadTestsFromFolder("Python");
            EditTestCommand = new RelayCommand(EditTest);
            DeleteTestCommand = new RelayCommand(DeleteTest);
            SyncFilesCommand = new RelayCommand(SyncFiles);
            SelectTabCommand = new RelayCommand(SelectTab);
            DownloadFilesCommand = new RelayCommand(DownloadFiles);
        }

        public void SetNavigationVM(NavigationVM navigationVM)
        {
            navigation = navigationVM;

        }
    }
}
