using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Answer
    {
        public string Text { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public bool IsSelected { get; set;  }

        public Answer() 
        {     
            IsCorrectAnswer = false;
        }
    }
}
