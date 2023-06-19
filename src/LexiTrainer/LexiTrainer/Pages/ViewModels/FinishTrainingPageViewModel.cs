using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace LexiTrainer.Pages.ViewModels;

public class FinishTrainingPageViewModel : ObservableObject
{
    private IWizard _wizard;
    private Session _session;
    private int _learnedWords;
    private int _totalWords;

    public FinishTrainingPageViewModel(IWizard wizard, Session session)
    {
        _wizard = wizard;
        _session = session;

        LearnedWordsCount = _session.LearnedWordsCount;
        TotalWordsCount = _session.WordsToTraining.Length;

        HomeCommand = new RelayCommand(GoHome);
    }

    public int LearnedWordsCount
    {
        get { return _learnedWords; }
        set { SetProperty(ref _learnedWords, value); }
    }

    public int TotalWordsCount
    {
        get { return _totalWords; }
        set { SetProperty(ref _totalWords, value); }
    }

    public ICommand HomeCommand { get; }

    private void GoHome()
    {
        _session.ClearTrainingData();
        _wizard.Navigate<LanguageSelectionPageViewModel>();
    }
}
