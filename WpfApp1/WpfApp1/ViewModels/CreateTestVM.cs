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


namespace WpfApp1.ViewModels
{
    internal class CreateTestVM:INotifyPropertyChanged
    {
        private ObservableCollection<Question> _questions;

        public CreateTestVM()
        {
            Questions = new ObservableCollection<Question>();
            AddQuestionCommand = new RelayCommand(AddQuestion);
            AddAnswerCommand = new RelayCommand(AddAnswer);
        }

        public ObservableCollection<Question> Questions
        {
            get { return _questions; }
            set
            {
                _questions = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddQuestionCommand { get; }
        public ICommand AddAnswerCommand { get; }

        private void AddQuestion(object parameter)
        {
            Questions.Add(new Question { Text = "Введите вопрос" });
        }

        private void AddAnswer(object parameter)
        {
            if (parameter is Question question)
            {
                question.Answers.Add(new Answer("Введите ответ"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
