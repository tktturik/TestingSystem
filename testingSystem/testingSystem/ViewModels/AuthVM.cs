using Authorization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using testingSystem.Models;
using testingSystem.Utilities;
namespace testingSystem.ViewModels
{
    internal class AuthVM: ViewModelBase
    {   
        private Auth auth;
        private Dictionary<string, string> _usersDictionary = new Dictionary<string, string>(); 

        private ObservableCollection<string> _users;
        private ObservableCollection<string> _allUsers = new ObservableCollection<string>();

        private string _searchText;
        private string _selectedUserValue;
        private string _selectedUserKey;
        public string SelectedUserValue
        {
            get => _selectedUserValue;
            set
            {
                _selectedUserValue = value;
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(value))
                {
                    SelectedUserKey = _usersDictionary.FirstOrDefault(x => x.Value == value).Key;
                }
            }
        }
        public string SelectedUserKey
        {
            get => _selectedUserKey;
            private set
            {
                _selectedUserKey = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterUsers();
            }
        }


        public ICommand StartButtonCommand { get; }


        private void StartButton(object parameter)
        {
            
        }

        public AuthVM()
        {
            StartButtonCommand = new RelayCommand(StartButton);

            auth = new Auth();

            _ = InitializeAsync();
           
        }
        private async Task InitializeAsync()
        {
            try
            {
                var usersList = await auth.GetUsers();
                _allUsers = new ObservableCollection<string>(usersList.Values);
                Users = new ObservableCollection<string>(_allUsers);
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Ошибка при инициализации: " + ex.Message);
                _allUsers = new ObservableCollection<string>();
                Users = new ObservableCollection<string>();
            }
        }
        private void FilterUsers()
        {
            if (_allUsers == null)
                return; // или throw/log, если это критично

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Users = new ObservableCollection<string>(_allUsers);
            }
            else
            {
                var filtered = _allUsers
                    .Where(u => u.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
                Users = new ObservableCollection<string>(filtered);
            }
        }
    }
}
