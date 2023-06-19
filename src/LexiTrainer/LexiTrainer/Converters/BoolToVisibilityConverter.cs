using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System;

namespace LexiTrainer.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || !(value is bool))
            return Visibility.Collapsed;

        bool boolValue = (bool)value;

        if (boolValue)
            return Visibility.Visible;
        else
            return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}
