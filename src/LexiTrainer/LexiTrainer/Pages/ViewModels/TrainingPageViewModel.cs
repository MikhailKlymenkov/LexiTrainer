using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows;

namespace LexiTrainer.Pages.ViewModels;

public abstract class TrainingPageViewModel : ObservableObject
{
    private IWizard _wizard;
    private Session _session;

    protected TrainingPageViewModel(IWizard wizard, Session session)
    {
        _wizard = wizard;
        _session = session;
        HomeCommand = new RelayCommand(GoHome);
    }

    public ICommand HomeCommand { get; }

    private void GoHome()
    {
        MessageBoxResult result = MessageBox.Show(
            "Are you sure you want to go back to the home page? All progress will be lost.",
            "Confirm",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _session.ClearTrainingData();
            _wizard.Navigate<LanguageSelectionPageViewModel>();
        }
    }
}
