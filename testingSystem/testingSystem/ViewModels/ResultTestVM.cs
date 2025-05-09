using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using testingSystem.Models;
using testingSystem.Utilities;

namespace testingSystem.ViewModels
{
    internal class ResultTestVM:ViewModelBase
    {
        Test test;
        string experienced;
        string nameOfTest;
        ObservableCollection<Question> questions;
        bool reportIsSend = false;
        int reportProgress=10;
        string result;

        public Test CurTest
        {
            get { return test; }
            set
            {
                test = value;
                Experienced = test.Experienced;
                Questions = test.questions;
                NameOfTest = test.Title;
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
        public bool ReportIsSend
        {
            get { return reportIsSend; }
            set
            {
                reportIsSend = value;
                OnPropertyChanged();
            }
        }
        public int ReportProgress
        {
            get { return reportProgress; }
            set
            {
                reportProgress = value;
                OnPropertyChanged();
            }
        }
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged();
            }
        }


        public ResultTestVM()
        {
            TestClick = new RelayCommand(TestClickA);
        }
        public void SetTest(Test finishedTest)
        {
            CurTest = finishedTest;
            NameOfTest = CurTest.Title;
            Experienced = CurTest.Experienced;
            Questions = CurTest.questions;
            Debug.WriteLine("AAAAAAAaa");
            OnPropertyChanged();

        }
        public ICommand TestClick { get; }
        
        public void TestClickA(object param){
            Debug.WriteLine(CurTest.Title + NameOfTest + CurTest.questions.Count);
        }
        public async Task SendReportAsync(int sumOfPoints, int maxPoints)
        {
            for (int i = 0; i <= 100; i += 10)
            {
                ReportProgress = i;
                await Task.Delay(100); 
            }

            Email email = new Email(CurTest);
            bool isSent = await email.SendEmail(
                DataService.CreateReportDocument(CurTest, sumOfPoints, maxPoints),
                sumOfPoints, maxPoints
            );

            if (isSent)
            {
                ReportIsSend = true;
                DataService.RemoveTest(CurTest, "tempTest.json");
            }
        }


    }
    
}
