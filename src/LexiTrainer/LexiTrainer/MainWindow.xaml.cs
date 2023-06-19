using Autofac;
using LexiTrainer.Messages;
using LexiTrainer.Popups.ViewModels;
using LexiTrainer.Popups.Views;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows;

namespace LexiTrainer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = ((App)Application.Current).AppContainer.Resolve<MainViewModel>();

        InitializeComponent();
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Register<OpenWindowMessage>(this, (recipient, message) => 
        {
            switch(message.ViewModel)
            {
                case WordsWindowViewModel vm:
                    var wordsWindow = new WordsWindow
                    {
                        DataContext = vm
                    };
                    message.DialogResult = wordsWindow.ShowDialog();
                    break;
            };
        });
    }

    private void WindowUnoaded(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Unregister<OpenWindowMessage>(this);
    }
}
