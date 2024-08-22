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
using WpfApp1.Models;
using WpfApp1.Utilities;

namespace WpfApp1.ViewModels
{
    internal class CreateTestVM:ViewModelBase
    {
        private ObservableCollection<Question> _questions;
        private Test _test;
        private string path = $"{Environment.CurrentDirectory}\\Tests\\name.json";
        private string _testTitle;

        public CreateTestVM()
        {
            _test = new Test();
            _questions = _test.questions;
            AddQuestionCommand = new RelayCommand(AddQuestion);
            AddAnswerCommand = new RelayCommand(AddAnswer);
            SaveTestCommand = new RelayCommand(SaveTest);
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
        public string TestTitle
        {
            get { return _testTitle; }
            set
            {
                _testTitle = value;
                _test.Title = value; // Обновляем свойство Title в объекте Test
                OnPropertyChanged();
            }
        }

        public ICommand AddQuestionCommand { get; }
        public ICommand AddAnswerCommand { get; }
public ICommand SaveTestCommand { get; }

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

        private void SaveTest(object parameter)
        {
            path = path.Replace("name", _test.Title);
            DataService.SaveQuestions(_test,path);
        }
      
    }
}
