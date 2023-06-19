using System.Windows;
using System.Windows.Controls;

namespace LexiTrainer.Extensions;

public static class PasswordBoxExtensions
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.RegisterAttached("Text", typeof(string), typeof(PasswordBoxExtensions), new PropertyMetadata(string.Empty, OnTextChanged));

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox passwordBox)
        {
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
    }

    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            SetText(passwordBox, passwordBox.Password);
        }
    }

    public static string GetText(DependencyObject dp)
    {
        return (string)dp.GetValue(TextProperty);
    }

    public static void SetText(DependencyObject dp, string value)
    {
        dp.SetValue(TextProperty, value);
    }
}
