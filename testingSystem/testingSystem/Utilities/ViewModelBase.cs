using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using testingSystem.ViewModels;

namespace testingSystem.Utilities
{
   public class ViewModelBase : INotifyPropertyChanged
    {
        private NavigationVM _navigationVM;
        private ViewModelBase PreviusVM;
        public ViewModelBase prevVM
        {
            get
            {
                return PreviusVM;
            }
            set
            {
                PreviusVM = value;
                OnPropertyChanged();
            }
        }
        public NavigationVM navigation
        {
            get { return _navigationVM; }
            set
            {
                _navigationVM = value;
                OnPropertyChanged();
            }
        }
        public void SetNavigationVM(NavigationVM navigationVM)
        {
            navigation = navigationVM;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
