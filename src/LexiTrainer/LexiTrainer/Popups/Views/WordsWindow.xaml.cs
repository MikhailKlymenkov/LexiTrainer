using LexiTrainer.Messages;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows;

namespace LexiTrainer.Popups.Views
{
    /// <summary>
    /// Interaction logic for WordsWindow.xaml
    /// </summary>
    public partial class WordsWindow : Window
    {
        public WordsWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Register<CloseWordsWindowMessage>(this, (_, _) => 
            {
                DialogResult = true;
                Close();
            } );
        }

        private void WindowUnoaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Unregister<CloseWordsWindowMessage>(this);
        }
    }
}
