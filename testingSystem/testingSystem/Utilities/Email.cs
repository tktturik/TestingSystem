using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using testingSystem.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Windows;

namespace testingSystem.Utilities
{
    internal class Email
    {
        string smtpServer = "smtp.gmail.com";
        int smtpPort = 587;
        string smtpUsername = "avkhvbg@gmail.com";
        string smtpPassword = "qdultocmhecowxdu";
        string receiver = "skynet.college.tests@mail.ru";
        Test test;

        public Email(Test _test)
        {
            this.test = _test;
        }

        public async Task<bool> SendEmail(string path, int resultPoints, int maxPoints)
        {
            string body = FormatTestResults(resultPoints, maxPoints);
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(smtpUsername);
                        mailMessage.To.Add(receiver);
                        mailMessage.Subject = $"Результаты теста {test.Title} ученика {test.Experienced}";
                        mailMessage.Body = body;
                        Attachment attachment = new Attachment(path);
                        mailMessage.Attachments.Add(attachment);

                        try
                        {
                            await smtpClient.SendMailAsync(mailMessage);
                            Debug.WriteLine("Сообщение отправлено");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Сообщение не отправлено: {ex.Message}");
                            Debug.WriteLine($"Сообщение не отправлено: {ex.Message}");
                            return false;
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
                return false;
            }
        }
        private string FormatTestResults(int resultPoints, int maxPoints)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Результаты теста ученика {test.Experienced}:\nНабрано баллов {resultPoints} из {maxPoints}, " +
                    $"что соответствует {resultPoints * 100 / maxPoints}%");
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в FormatTestResults: {ex.Message}");

                Debug.WriteLine($"Ошибка в FormatTestResults: {ex.Message}");
                return string.Empty;
            }
        }


    }
}
