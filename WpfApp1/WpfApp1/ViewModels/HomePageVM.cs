using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Utilities;

namespace WpfApp1.ViewModels
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
