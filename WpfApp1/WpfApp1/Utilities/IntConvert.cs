using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApp1.Utilities
{
    public class IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString();
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                if (string.IsNullOrEmpty(strValue))
                {
                    return 0; // или return DependencyProperty.UnsetValue; для отмены привязки
                }
                if (int.TryParse(strValue, out int result))
                {
                    return result;
                }
            }
            return 0; // или return DependencyProperty.UnsetValue; для отмены привязки
        }
    }
}
