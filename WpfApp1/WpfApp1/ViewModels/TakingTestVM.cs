using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1.ViewModels
{
    internal class TakingTestVM : ViewModelBase
    {

        ObservableCollection<Question> questions;
        private Test _test;
        public TakingTestVM()
        {
          
            FinishTestCommand = new RelayCommand(FinishTest);
        }
        public void SetTest(Test test)
        {
            _test = test;
            Questions = _test.questions;
        }

        public ICommand FinishTestCommand { get; }

        public ObservableCollection<Question> Questions
        {
            get { return questions; }
            set
            {
                questions = value;
                OnPropertyChanged();
            }
        }

        private void FinishTest(object parameter)
        {
            int CountCorrect = 0;
            foreach (Question question in questions)
            {
                foreach (Answer answer in question.Answers)
                {
                    if(answer.IsCorrectAnswer && answer.IsSelected)
                    {
                        CountCorrect++;
                    }
                }

            }
            MessageBox.Show($"Верных ответов: {CountCorrect}");

        }

    
    }
}
