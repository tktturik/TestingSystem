using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using testingSystem.Utilities;

namespace testingSystem.ViewModels
{
    public class HomePageVM:ViewModelBase
    {
        public HomePageVM()
        {
            Task.Run(Initialize);
        }
        private async Task Initialize()
        {
            await DataService.Initialize();
;
        }

    }
}
