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

namespace WpfApp1.ViewModels
{
    internal class ChoosingTestVM : ViewModelBase
    {
        private string path = $"{Environment.CurrentDirectory}\\Tests\\";

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
        
        private void EditTest(object parametr)
        {
            if(parametr is Test testToUpdate)
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
                //bool isDeleted = DataService.RemoveTest(path + testToDelete.Title+".json");
                //Debug.WriteLine(isDeleted);
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
            Tests = DataService.LoadTests("1ZsvGd8t9DMTyCwY8QrOWoLCbja5S6Yv-");
            EditTestCommand=new RelayCommand(EditTest);
            DeleteTestCommand=new RelayCommand(DeleteTest);
        }
        public void SetNavigationVM(NavigationVM navigationVM)
        {
            navigation = navigationVM;

        }
    }
}
