using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.View.UserControls;

namespace WpfApp1
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private UserControl currControl;

        public UserControl UserControl { get { return currControl; } 
            set
            {
                currControl = value;
                OnPropertyChanged();
            }
        
        }

        public MainViewModel()
        {
            ShowCreateTestViewCommand = new RelayCommand(ShowCreateTestView);
            ShowTakeTestViewCommand = new RelayCommand(ShowTakeTestView);
        }

        public ICommand ShowCreateTestViewCommand { get; }
        public ICommand ShowTakeTestViewCommand { get; }


        public ICommand AddQuestionCommand { get; }
        public ICommand AddAnswerCommand { get; }

        private void ShowCreateTestView(object parameter)
        {
            UserControl = new CreateTest();
            System.Diagnostics.Debug.WriteLine("CreateTest UserControl set");
        }
        private void ShowTakeTestView(object parameter)
        {
            UserControl = new TakingTest();
        }

      
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
