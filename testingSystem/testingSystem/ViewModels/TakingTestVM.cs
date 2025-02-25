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
using testingSystem.Models;
using testingSystem.Utilities;
using System.Diagnostics;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System.IO;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

using System.Configuration;


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
        public string NameOfTest
        {
            set
            {
                nameOfTest = value;
                OnPropertyChanged();
            }
            get { return nameOfTest; }
        }

        public ICommand FinishTestCommand { get; }
        public ICommand SaveTemporaryFileCommand { get; }
        public ICommand BackBtnCommand { get; }

        public ObservableCollection<Question> Questions
        {
            get { return questions; }
            set
            {
                questions = value;
                OnPropertyChanged();
            }
        }


        private void BackBtn(object paremeter)
        {
            if (MessageBox.Show("Завершить тестирование, потратив попытку?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                FinishTest(null);
            }
        }
        private void OpenChosingTestUC()
        {
            navigation.SetUserControl(new ChoosingTestVM() {Attemps = attemps.CountAttemps });
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
                        mailMessage.To.Add("skynet.college@mail.ru"); 
                        mailMessage.Subject = $"Результаты теста {_test.Title} ученика {Test.Experienced}";
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
                    titleRun.AppendChild(new Text($"Результаты теста ученика {Test.Experienced}:\nНабрано баллов {resultPoints} из {maxPoints}, что соответствует {resultPoints * 100 / maxPoints}%"));

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
                MessageBox.Show($"Ошибка в CreateWordDocument: {ex.Message} путь {filePath}");
                Debug.WriteLine($"Ошибка в CreateWordDocument: {ex.Message} путь {filePath}");
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
                sb.AppendLine($"Результаты теста ученика {test.Experienced}:\nНабрано баллов {resultPoints} из {maxPoints}, " +
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
            catch (DivideByZeroException ex)
            {
                MessageBox.Show($"В тесте максимальное количество баллов 0: {ex.Message}");
                Debug.WriteLine($"Ошибка в FormatTestResults: {ex.Message}");
                return string.Empty;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка в FormatTestResults: {ex.Message}");

                Debug.WriteLine($"Ошибка в FormatTestResults: {ex.Message}");
                return string.Empty;
            }
        }

        public void FinishTest(object parameter)
        {
            try
            {
                //int attemptsAvailable = int.Parse(ConfigurationManager.AppSettings.Get("AttemptsAvailable"));
                //if (attemps.CountAttemps <= 0)
                //{
                //    MessageBox.Show("Попытки на сегодня закончились");
                //    return;
                //}
                //if ((string.IsNullOrWhiteSpace(Experienced) || Experienced.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < 2) && Experienced[0] != ' ')
                //{
                //    MessageBox.Show("Пожалуйста, введите свое имя и фамилию.");
                //    return;
                //}
                //int CountCorrect = 0;
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
                MessageBox.Show(resultMessage);
                string body = FormatTestResults(_test, sumOfPoints, maxPoints);
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                string directoryPath = Path.Combine(appDataPath, "SkyNetTS", "Tests");
                string fileName = $"{_test.Experienced}_{_test.Title}_{DateTime.Now:dd.MM.yyyy HH mm}.docx".Replace(":", " ");
                string fullPath = Path.Combine(directoryPath, fileName);

                CreateWordDocument(_test, sumOfPoints, maxPoints, fullPath);
                MessageBox.Show("Отчет отправляется...");
                SendEmail(body, fullPath);
                

                DataService.RemoveTest(_test, "tempTest.json");
                attemps--;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            OpenChosingTestUC();
        }
        //private void UpdateAttemps()
        //{
        //    int attemptsAvailable = int.Parse(ConfigurationManager.AppSettings.Get("AttemptsAvailable"));
        //    attemptsAvailable--;
        //    ConfigurationManager.AppSettings.Set("AttemptsAvailable", attemptsAvailable.ToString());
        //    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    config.AppSettings.Settings["AttemptsAvailable"].Value = attemptsAvailable.ToString();
        //    config.Save(ConfigurationSaveMode.Modified);
        //    ConfigurationManager.RefreshSection("appSettings");

        //    Debug.WriteLine(ConfigurationManager.AppSettings.Get("AttemptsAvailable"));
        //}

    }
}
