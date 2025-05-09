using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testingSystem.Models;
using System.Windows;


namespace testingSystem.Utilities
{
    public static class WordDocument
    {
        public static bool CreateWordDocument(Test test, int resultPoints, int maxPoints, string filePath)
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
                    titleRun.AppendChild(new Text($"Результаты теста ученика {test.Experienced}:\nНабрано баллов {resultPoints} из {maxPoints}, что соответствует {resultPoints * 100 / maxPoints}%"));

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
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в CreateWordDocument: {ex.Message} путь {filePath}");
                Debug.WriteLine($"Ошибка в CreateWordDocument: {ex.Message} путь {filePath}");
                return false;
            }
        }
    }
}
