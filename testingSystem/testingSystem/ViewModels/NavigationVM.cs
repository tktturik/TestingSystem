using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using testingSystem.Models;
using System.IO;
using testingSystem.Utilities;
using testingSystem.View.UserControls;
using System.Windows.Forms;

namespace testingSystem.ViewModels
{
    public class NavigationVM:ViewModelBase
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
        public ICommand ShowMyFilesViewCommand { get; }
        public ICommand ShowSettingsViewCommand { get; }
        public ICommand ShowPersonalAccountViewCommand { get; }
        public ICommand ShowTasksViewCommand { get; }



        private void ShowCreateTestView(object parameter)
        {
            UserControl = new CreateTestVM();
        }
        private void ShowMyFilesView(object parameter)
        {
            UserControl = new MyFilesVM();
        }
        private void ShowPersonalAccountView(object parameter)
        {
            UserControl = new PersonalAccountVM();
        }
        private void ShowTasksView(object parameter)
        {
            UserControl = new TasksVM();
        }
        private void ShowSettingsView(object parameter)
        {
            UserControl = new SettingsVM();
        }
        public void ShowTakeTestView(object parameter)
        {
            if (DataService.FileExists("tempTest.json"))
            {
                string json = File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\SkyNetTS\\Tests\\tempTest.json");
                Test tmpTest = DataService.JsonConverter(json);
                TakingTestVM tt = new TakingTestVM();
                tt.SetTest(tmpTest);
                ShowSelectedTest(tt);
            }
            else
            {
                ChoosingTestVM choosingTestVM1 = new ChoosingTestVM();
                choosingTestVM1.navigation = this;
                UserControl = new ChoosingTest { DataContext = choosingTestVM1 };
            }

        }
        public void SetUserControl(ViewModelBase viewModelBase)
        {
            viewModelBase.navigation = this;
            UserControlFactory factory = new UserControlFactory();
            UserControl = factory.CreateUserControl(viewModelBase);

        }
        private void ShowHomeView(object parameter)
        {
            UserControl = new HomePageVM();
        }
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public void ShowSelectedTest(object test)
        {
            if (test != null && test is TakingTestVM takingTest)
            {
                SetUserControl(takingTest);
                
                //UserControl = new TakingTest() { DataContext = takingTest };
            }
            else if (test != null && test is CreateTestVM createTest)
            {
                UserControl = new CreateTest() { DataContext = createTest };
            }
        }

        public NavigationVM()
        {
            ShowCreateTestViewCommand = new RelayCommand(ShowCreateTestView);
            ShowTakeTestViewCommand = new RelayCommand(ShowTakeTestView);
            ShowHomeViewCommand = new RelayCommand(ShowHomeView);
            ShowMyFilesViewCommand = new RelayCommand(ShowMyFilesView);
            ShowSettingsViewCommand = new RelayCommand(ShowSettingsView);
            ShowPersonalAccountViewCommand = new RelayCommand(ShowPersonalAccountView);
            ShowTasksViewCommand = new RelayCommand(ShowTasksView);

            SetUserControl(new HomePageVM());
        }

    }
}
