using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AdminPanel
{
    public class ConditionToColorConverter : IValueConverter
    {
        static SolidColorBrush RedBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        static SolidColorBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? GreenBrush : RedBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
