using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Repo1.WPF45.SDK.Converters
{
    public class VisibleIfNotZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            int val;
            if (!int.TryParse(value.ToString(), out val)) return Visibility.Collapsed;
            return val != 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
