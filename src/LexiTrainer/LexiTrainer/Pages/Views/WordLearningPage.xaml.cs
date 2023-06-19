using LexiTrainer.Messages;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace LexiTrainer.Pages.Views
{
    /// <summary>
    /// Interaction logic for WordLearningPage.xaml
    /// </summary>
    public partial class WordLearningPage : UserControl
    {
        public WordLearningPage()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Register<NextWordShownMessage>(this, (recipient, message) =>
            {
                ARadioButton.IsChecked = false;
                BRadioButton.IsChecked = false;
                CRadioButton.IsChecked = false;
            });
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Unregister<NextWordShownMessage>(this);
        }
    }
}
