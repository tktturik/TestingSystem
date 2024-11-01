﻿using System;
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
using System.Diagnostics;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System.IO;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

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
            SaveTemporaryFileCommand = new RelayCommand(SaveTemporaryFile);
        }

        public void SetTest(Test test)
        {
            _test = test;
            Questions = _test.questions;
        }

        public ICommand FinishTestCommand { get; }
        public ICommand SaveTemporaryFileCommand { get; }

        public ObservableCollection<Question> Questions
        {
            get { return questions; }
            set
            {
                questions = value;
                OnPropertyChanged();
            }
        }

        private void SendEmail(string body, string path)
        {
            try
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
                        Attachment attachment = new Attachment(path);
                        mailMessage.Attachments.Add(attachment);

                        try
                        {
                            smtpClient.Send(mailMessage);
                            Debug.WriteLine("Сообщение отправлено");
                            MessageBox.Show("Сообщение отправлено");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Сообщение не отправлено: {ex.Message}");
                            Debug.WriteLine($"Сообщение не отправлено: {ex.Message}");
                        }
                        finally
                        {
                            attachment.Dispose();
                            File.Delete(path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в SendEmail: {ex.Message}");
                Debug.WriteLine($"Ошибка в SendEmail: {ex.Message}");
            }
        }
        private void CreateWordDocument(Test test, int resultPoints, int maxPoints, string filePath)
        {
            try
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    Paragraph title = body.AppendChild(new Paragraph());
                    Run titleRun = title.AppendChild(new Run());
                    titleRun.AppendChild(new Text($"Результаты теста ученика {experienced}:\nНабрано баллов {resultPoints} из {maxPoints}, что соответствует {resultPoints * 100 / maxPoints}%"));

                    foreach (Question question in test.questions)
                    {
                        Paragraph questionPara = body.AppendChild(new Paragraph());
                        Run questionRun = questionPara.AppendChild(new Run());
                        questionRun.AppendChild(new Text($"Вопрос: {question.Text}"));

                        foreach (Answer answer in question.Answers)
                        {
                            Paragraph answerPara = body.AppendChild(new Paragraph());
                            Run answerRun = answerPara.AppendChild(new Run());
                            string symbol = "";
                            if (answer.IsCorrectAnswer && answer.IsSelected) symbol = "🎉";
                            else if (answer.IsCorrectAnswer && !answer.IsSelected) symbol = "✅";
                            else if (!answer.IsCorrectAnswer && answer.IsSelected) symbol = "✔️";
                            answerRun.AppendChild(new Text($"- {answer.Text} {symbol} {(answer.IsCorrectAnswer ? $"({answer.Points} баллов)" : "")}"));
                        }

                        body.AppendChild(new Paragraph());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в CreateWordDocument: {ex.Message}");
                Debug.WriteLine($"Ошибка в CreateWordDocument: {ex.Message}");
            }
        }
        private async void SaveTemporaryFile(object parameter)
        {
            await DataService.SaveTest(_test, "tempTest.json");
        }
        private string FormatTestResults(Test test, int resultPoints, int maxPoints)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Результаты теста ученика {experienced}:\nНабрано баллов {resultPoints} из {maxPoints}, " +
                    $"что соответствует {resultPoints* 100 / maxPoints  }%");
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в FormatTestResults: {ex.Message}");
                Debug.WriteLine($"Ошибка в FormatTestResults: {ex.Message}");
                return string.Empty;
            }
        }
        private void FinishTest(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Experienced) || Experienced.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < 2)
            {
                MessageBox.Show("Пожалуйста, введите свое имя и фамилию.");
                return;
            }
            //int CountCorrect = 0;
            int sumOfPoints = 0;
            int maxPoints = 0;
            foreach (Question question in questions)
            {
                int correctAnswersCount = question.Answers.Count(a => a.IsCorrectAnswer);
                int selectedAnswersCount = question.Answers.Count(a=>a.IsSelected);
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
            MessageBox.Show(resultMessage);
            string body = FormatTestResults(_test,sumOfPoints,maxPoints);
            string tempFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Tests\\{Experienced}_{_test.Title}.docx";
            CreateWordDocument(_test, sumOfPoints, maxPoints, tempFilePath);
            SendEmail(body,tempFilePath);
            DataService.RemoveTest(_test, "tempTest.json");

        }

    
    }
}
