﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingSystem.Models
{
    public class Test
    {
        public string Title { get; set; }
        public int AmountQuestions { get; set; }
        public string Section { get; set; }
        public string Experienced { get; set; }
        public ObservableCollection<Question> questions { get; set; } = new ObservableCollection<Question>();

    }
}
