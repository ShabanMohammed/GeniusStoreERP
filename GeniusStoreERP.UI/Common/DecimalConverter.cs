using System.Globalization;
using System.Windows.Data;

namespace GeniusStoreERP.UI.Common;

public class DecimalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue.ToString("0.00", culture);
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
        {
            return 0m;
        }

        if (value is string str)
        {
            if (decimal.TryParse(str, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, culture, out var result))
            {
                return result;
            }
            else
            {
                // محاولة مع فاصل عشري مختلف
                var altStr = str.Replace(',', '.').Replace('.', culture.NumberFormat.NumberDecimalSeparator[0]);
                if (decimal.TryParse(altStr, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, culture, out var altResult))
                {
                    return altResult;
                }
            }
        }

        return 0m;
    }
}
