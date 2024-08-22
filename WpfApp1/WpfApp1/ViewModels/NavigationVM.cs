using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Utilities;
using WpfApp1.View.UserControls;

namespace WpfApp1.ViewModels
{
    internal class NavigationVM:ViewModelBase
    {
        private object currControl;


        public object UserControl
        {
            get { return currControl; }
            set
            {
                currControl = value;
                OnPropertyChanged();
            }

        }

       

        public ICommand ShowCreateTestViewCommand { get; }
        public ICommand ShowTakeTestViewCommand { get; }
        public ICommand ShowHomeViewCommand { get; }

        private void ShowCreateTestView(object parameter)
        {
            UserControl = new CreateTestVM();
        }
        private void ShowTakeTestView(object parameter)
        {
            //хуйня
            ChoosingTestVM choosingTestVM1 = new ChoosingTestVM();
            choosingTestVM1.SetNavigationVM(this);
            UserControl = new ChoosingTest { DataContext = choosingTestVM1 };

        }
        private void ShowHomeView(object parameter)
        {
            UserControl = new HomePageVM();
        }

        public void ShowSelectedTest(Test test)
        {
            if(test != null)
            {
                //хуйня

                TakingTestVM takingTest = new TakingTestVM();
                takingTest.SetTest(test);
                UserControl = new TakingTest() { DataContext =  takingTest };
            }
        }

        public NavigationVM()
        {
            ShowCreateTestViewCommand = new RelayCommand(ShowCreateTestView);
            ShowTakeTestViewCommand = new RelayCommand(ShowTakeTestView);
            ShowHomeViewCommand = new RelayCommand(ShowHomeView);

            UserControl = new HomePageVM();
        }

    }
}
