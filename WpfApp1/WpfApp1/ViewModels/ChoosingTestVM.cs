using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Utilities;
using WpfApp1.Models;
using System.Windows;

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
                    _navigationVM.ShowSelectedTest(selectedTest);
                }
            }
        }
        public void SetNavigationVM(NavigationVM navigationVM)
        {
            navigation = navigationVM;
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
            Tests = DataService.LoadTests(path);
        }

    }
}
