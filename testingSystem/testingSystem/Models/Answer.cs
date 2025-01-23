using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingSystem
{
    /// <summary>
    /// Сущность Ответа, включает
    /// Text - Текст ответа
    /// IsCorrectAnswer - бульевое значение верности ответа
    /// Points - количество баллов за ответ
    /// </summary>
    public class Answer
    {
        public string Text { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public bool IsSelected { get; set;  }
        public int Points { get; set; }

        public Answer() 
        {     
            IsCorrectAnswer = false;
            Points = 0;
        }
    }
}
