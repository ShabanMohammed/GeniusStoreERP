using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GeniusStoreERP.UI.Common
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            if (value is byte[] bytes)
            {
                isNull = bytes.Length == 0;
            }

            bool result = Invert ? !isNull : isNull;
            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
