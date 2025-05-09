using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace testingSystem.Utilities
{
    internal class AnswerCheckBox: CheckBox
    {
        static AnswerCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnswerCheckBox), new FrameworkPropertyMetadata(typeof(AnswerCheckBox)));

        }
    }
}
