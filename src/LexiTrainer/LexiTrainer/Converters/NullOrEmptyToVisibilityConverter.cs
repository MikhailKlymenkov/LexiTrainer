using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System;

namespace LexiTrainer.Converters;

public class NullOrEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var invisibleState = (Visibility)parameter;
        return string.IsNullOrEmpty(value?.ToString()) ? invisibleState : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}

