using System;
using System.Collections.Generic;
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
            try
            {
                DataService.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

    }
}
