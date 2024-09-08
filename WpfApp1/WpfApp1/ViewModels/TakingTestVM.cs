using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Utilities;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

namespace WpfApp1.ViewModels
{
    internal class TakingTestVM : ViewModelBase
    {

        ObservableCollection<Question> questions;
        private Test _test;
        private string experienced;

        public string Experienced
        {
            get { return experienced; }
            set
            {
                experienced = value;
                OnPropertyChanged();
            }
        }
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
       
private void SendEmail( string body)
    {

            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "avkhvbg@gmail.com";
            string smtpPassword = "qdultocmhecowxdu";

            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add("medvedshura1@gmail.com");
                    mailMessage.Subject = $"Результаты теста {_test.Title} ученика {experienced}";
                    mailMessage.Body = body;

                    try
                    {
                        smtpClient.Send(mailMessage);
                        Debug.WriteLine("Сообщение отправлено");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Сообщение не отправлено" + ex.Message);

                    }

                }
            }
           }

        private string FormatTestResults(Test test,int resultPoints, int maxPoints)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Результаты теста ученика {experienced}:\nНабрано баллов {resultPoints} из {maxPoints}, " +
                $"что соответствует {resultPoints*100/maxPoints}%");
            sb.AppendLine();

            foreach (Question question in test.questions)
            {
                sb.AppendLine($"Вопрос: {question.Text}");
                sb.AppendLine("Ответы:");

                foreach (Answer answer in question.Answers)
                {
                    string symbol = "";
                    if (answer.IsCorrectAnswer && answer.IsSelected) symbol = "🎉";
                    else if (answer.IsCorrectAnswer && !answer.IsSelected) symbol = "✅";
                    else if (!answer.IsCorrectAnswer && answer.IsSelected) symbol = "✔️";
                    sb.AppendLine($"- {answer.Text} {symbol} {(answer.IsCorrectAnswer ? $"({answer.Points} баллов)" : "")}");
                }

                sb.AppendLine("\n\n");
            }

            return sb.ToString();
        }
        private void FinishTest(object parameter)
        {
            //int CountCorrect = 0;
            int sumOfPoints = 0;
            int maxPoints = 0;
            foreach (Question question in questions)
            {
                foreach (Answer answer in question.Answers)
                {
                    if(answer.IsCorrectAnswer && answer.IsSelected)
                    {
                        //CountCorrect++;
                        sumOfPoints += answer.Points;
                        maxPoints += answer.Points;
                    }
                    else
                    {
                        maxPoints += answer.Points;
                    }
                }

            }
            string resultMessage = $"Набранные баллы: {sumOfPoints}";
            MessageBox.Show(resultMessage);

            // Отправка результатов на почту
            string body = FormatTestResults(_test,sumOfPoints,maxPoints);

            SendEmail(body);
            //MessageBox.Show($"Верных ответов: {CountCorrect}");
            MessageBox.Show($"Набранные баллы:  {sumOfPoints}");

        }

    
    }
}
