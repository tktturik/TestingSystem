using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Utilities
{
    public class TabBtn:RadioButton
    {
        static TabBtn() 
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabBtn), new FrameworkPropertyMetadata(typeof(TabBtn)));
        }
    }
}
