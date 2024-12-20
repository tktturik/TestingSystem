using DocumentFormat.OpenXml.Office2010.CustomUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using testingSystem;
using testingSystem.Utilities;
using testingSystem.View.UserControls;
using testingSystem.ViewModels;

namespace testingSystem.ViewModels
{
    public class WhoISVM:ViewModelBase
    {
        private string name;
        private string surname;
        private string testTitle;
        private int countQuestions;
        private int maxPoints;
        private TakingTestVM takingTest;

        public TakingTestVM Test 
        { 
            set 
            {
                takingTest = value;
                testTitle = takingTest.NameOfTest;
                countQuestions = takingTest.Questions.Count;
                maxPoints = takingTest.Questions.Sum(q => q.Answers.Sum(p => p.Points));
                OnPropertyChanged();
            } 
        }
        public int MaxPoints
        {
            get
            {
                return maxPoints;
            }
        }
        public string TestTitile
        {
            get
            {
                return testTitle;
            }
        }
        public int CountQuestions
        {
            get
            {
                return countQuestions;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public string Surname
        {
            get
            {
                return surname;
            }
            set
            {
                surname = value;
                OnPropertyChanged();
            }
        }

        public WhoISVM() 
        {
            StartTestCommad = new RelayCommand(OpenTakingTest);
        }
        public ICommand StartTestCommad { get; }


        private void OpenTakingTest(object parameter)
        {
            if(!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Surname))
            {
                takingTest.Test.Experienced = string.Join(" ", Name, Surname);
                Debug.WriteLine($"{takingTest.Experienced}");
                navigation.SetUserControl(takingTest);
            }
            else
            {
                MessageBox.Show("Введите корректно имя и фамилию");
            }
        }
    }
}
