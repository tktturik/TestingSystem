using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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


        public Test Test {  
            get { return _test; } 
            set 
            { 
                _test = value; 
                _questions = _test.questions;
                TestTitle = _test.Title;
                OnPropertyChanged(); 
            } 
        }
        public CreateTestVM()
        {
            Debug.WriteLine("Создан CreateTest");
            _test = new Test();
            Questions = _test.questions;
            TestTitle = _test.Title;
            AddQuestionCommand = new RelayCommand(AddQuestion);
            AddAnswerCommand = new RelayCommand(AddAnswer);
            SaveTestCommand = new RelayCommand(SaveTest);
            UpdatePointsCommand=new RelayCommand(UpdatePoints);
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
        public ICommand UpdatePointsCommand { get; }

        private void AddQuestion(object parameter)
        {
            Questions.Add(new Question());
        }

        private void AddAnswer(object parameter)
        {
            if (parameter is Question question)
            {
                question.Answers.Add(new Answer());
            }
        }

        private void SaveTest(object parameter)
        {
            path = path.Replace("name", _test.Title);
            DataService.SaveQuestions(_test);
        }
        private void UpdatePoints(object parameter)
        {
            if (parameter is Answer answer)
            {
                Debug.WriteLine("xh");
                answer.Points = answer.IsCorrectAnswer ? 1 : 0;
                Debug.WriteLine(answer.Points);
            }
        }

    }
}
