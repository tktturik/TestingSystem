using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using testingSystem.Utilities;
using testingSystem.View.UserControls;
using HandyControl.Controls;

namespace testingSystem.ViewModels
{
    public class HomePageVM:ViewModelBase
    {
        public HomePageVM()
        {
            Task.Run(Initialize);
            TestBut = new RelayCommand(Test);
        }
        private async Task Initialize()
        {
            await DataService.Initialize();
;
        }
        public ICommand TestBut { get; }

        private void Test(object parameter)
        {
            Dialog.Show(new AuthModalWindow() { DataContext = new AuthVM() });
        }
    }
}
