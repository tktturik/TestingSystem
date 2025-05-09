using HandyControl.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using testingSystem.Models;
using testingSystem.Utilities;
using System.Windows;
using System.Windows.Input;
using System;
using testingSystem.View.UserControls;
using System.Threading.Tasks;
namespace testingSystem.ViewModels
{
    public class TakingTestVM : ViewModelBase
    {

        ObservableCollection<Question> questions;
        private Test _test;
        private string experienced;
        private Attemps attemps;
        private string nameOfTest;

        public Test Test
        {
            get
            {
                return _test;
            }
            set
            {
                _test = value;
                Experienced = _test.Experienced;
                Questions = _test.questions;
                NameOfTest = _test.Title;
                OnPropertyChanged();
            }
        }
      

        public string Experienced
        {
            get { return experienced; }
            set
            {
                experienced = value;
                OnPropertyChanged();
            }
        }
        public string NameOfTest
        {
            set
            {
                nameOfTest = value;
                OnPropertyChanged();
            }
            get { return nameOfTest; }
        }
        public ObservableCollection<Question> Questions
        {
            get { return questions; }
            set
            {
                questions = value;
                OnPropertyChanged();
            }
        }
     
        public TakingTestVM()
        {
            attemps = DataService.DeserializeAttemps();
            FinishTestCommand = new RelayCommand(FinishTest);
            SaveTemporaryFileCommand = new RelayCommand(SaveTemporaryFile);
            BackBtnCommand = new RelayCommand(BackBtn);
        }

        public void SetTest(Test test)
        {
            _test = test;
            NameOfTest = _test.Title;
            Experienced = _test.Experienced;
            Questions = _test.questions;
            OnPropertyChanged();
        }
       

        public ICommand FinishTestCommand { get; }
        public ICommand SaveTemporaryFileCommand { get; }
        public ICommand BackBtnCommand { get; }

    
        /// <summary>
        /// Действие кнопки "назад"
        /// </summary>
        /// <param name="paremeter"></param>
        private void BackBtn(object paremeter)
        {
            if (System.Windows.MessageBox.Show("Завершить тестирование, потратив попытку?", "Подтверждение", MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                FinishTest(null);
            }
        }
        /// <summary>
        /// Открытие окна выбора тестов
        /// </summary>
        private void OpenChosingTestUC()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                navigation.SetUserControl(new HomePageVM());
            });
        }
        /// <summary>
        /// Сохранение текущего теста во временный файл
        /// </summary>
        /// <param name="parameter"></param>
        private async void SaveTemporaryFile(object parameter)
        {
            await DataService.SaveTest(_test, "tempTest.json");
        }
       
        public async void FinishTest(object parameter)
        {
            try
            {
                int sumOfPoints = 0;
                int maxPoints = 0;
                foreach (Question question in questions)
                {
                    int correctAnswersCount = question.Answers.Count(a => a.IsCorrectAnswer);
                    int selectedAnswersCount = question.Answers.Count(a => a.IsSelected);
                    maxPoints += question.Answers.Sum(a => a.Points);
                    if (selectedAnswersCount <= correctAnswersCount)
                    {
                        foreach (Answer answer in question.Answers)
                        {
                            if (answer.IsCorrectAnswer && answer.IsSelected)
                            {
                                sumOfPoints += answer.Points;
                            }
                        }
                    }
                }

                string resultMessage = $"Набранные баллы: {sumOfPoints}";
               
                attemps--;
                ResultTestVM resultTestVM = new ResultTestVM();
                resultTestVM.SetTest(_test);
                resultTestVM.Result = $"Набранные баллы: {sumOfPoints} это {sumOfPoints * 100 / maxPoints}%";

                Dialog.Show(new TestResult {DataContext = resultTestVM });
                OpenChosingTestUC();
                await resultTestVM.SendReportAsync(sumOfPoints, maxPoints);

            }
            catch (Exception ex) 
            {
                OpenChosingTestUC();
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    
    }
}
