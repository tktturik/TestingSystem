using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Utilities;

namespace WpfApp1.ViewModels
{
    public class HomePageVM:ViewModelBase
    {
        public HomePageVM() { 
            DataService.Initialize();
        }

    }
}
