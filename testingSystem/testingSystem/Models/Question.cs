using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingSystem
{
    /// <summary>
    /// Сущность Вопрос, включает Text - текст вопроса
    /// Answers - коллекция ответов
    /// 
    /// </summary>
    public class Question
    {
        public string Text { get; set; }
        public ObservableCollection<Answer> Answers { get; set; }
        public byte[] ImageBytes { get; set; }

        public Question()
        {
            Answers = new ObservableCollection<Answer>();
        }
    }
}
