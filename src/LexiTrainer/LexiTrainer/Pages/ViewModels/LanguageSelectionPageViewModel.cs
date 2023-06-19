using LexiTrainer.Db;
using LexiTrainer.Db.Model;
using LexiTrainer.Extensions;
using LexiTrainer.Messages;
using LexiTrainer.Popups.ViewModels;
using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using IDictionaryService = LexiTrainer.Services.IDictionaryService;

namespace LexiTrainer.Pages.ViewModels;

public class LanguageSelectionPageViewModel : ObservableObject
{
    private IWizard _wizard;
    private Session _session;
    private bool _isNoWordsVisible;
    private IDatabaseProvider _dbProvider;
    private WordsWindowViewModel.Factory _wordsWindowViewModelFactory;
    private IDictionaryService _dictionaryService;

    public LanguageSelectionPageViewModel(
        IWizard wizard,
        Session session,
        IDatabaseProvider dbProvider,
        WordsWindowViewModel.Factory wordsWindowViewModelFactory,
        IDictionaryService dictionaryService)
    {
        _wizard = wizard;
        _session = session;
        _dbProvider = dbProvider;
        _wordsWindowViewModelFactory = wordsWindowViewModelFactory;
        _dictionaryService = dictionaryService;

        SignOutCommand = new RelayCommand(SignOut);
        ChangePasswordCommand = new RelayCommand(ChangePassword);
        ImportDictionaryCommand = new RelayCommand(ImportDictionary);
        SeeWordsCommand = new RelayCommand(SeeWords);
        SelectLanguageCommand = new RelayCommand<LanguageViewModel>(SelectLanguage);
        ExportCommand = new RelayCommand(ExportWords);

        Refresh();
    }

    public ObservableCollection<LanguageViewModel> Languages { get; } = new();

    public bool IsNoWordsVisible
    {
        get { return _isNoWordsVisible; }
        set { SetProperty(ref _isNoWordsVisible, value); }
    }

    public ICommand SignOutCommand { get; }

    public ICommand ChangePasswordCommand { get; }

    public ICommand ImportDictionaryCommand { get; }

    public ICommand SeeWordsCommand { get; }

    public ICommand SelectLanguageCommand { get; }

    public ICommand ExportCommand { get; }

    private void Refresh()
    {
        var words = _dbProvider.GetAll<Word>().Where(x => x.UserName == _session.User.Name);

        if (words.Count() == 0)
        {
            IsNoWordsVisible = true;
        }
        else
        {
            IsNoWordsVisible = false;
            Languages.Clear();
            var languages = words.Select(x => x.Language.ToLower()).Distinct();
            foreach (var language in languages)
            {
                int wordsCount = words.Where(x => x.Language.ToLower() == language).Count();
                Languages.Add(new LanguageViewModel
                {
                    Name = language.FirstCharToUpper(),
                    WordsCount = wordsCount
                });
            }
        }
    }

    private void SelectLanguage(LanguageViewModel language)
    {
        _session.Language = language.Name;
        _wizard.Navigate<DictionarySelectionPageViewModel>();
    }

    private void SignOut()
    {
        var currentUser = _session.User;
        currentUser.IsCurrent = false;
        _dbProvider.Update(currentUser);
        _session.Clear();

        _wizard.Navigate<LoginPageViewModel>();
    }

    private void ImportDictionary()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try 
            {
                if (_dictionaryService.Import(openFileDialog.FileName))
                {
                    Refresh();
                }
            }
            catch
            {
                MessageBox.Show("Invalid dictionary");
            }
        }
    }


    private void SeeWords()
    {
        var words = _dbProvider.GetAll<Word>().Where(x => x.UserName == _session.User.Name);
        var wordsWindowViewModel = _wordsWindowViewModelFactory(words, true, false);
        var openWindowMessage = new OpenWindowMessage
        {
            ViewModel = wordsWindowViewModel
        };
        WeakReferenceMessenger.Default.Send(openWindowMessage);
        if (openWindowMessage.DialogResult == true)
        {
            Refresh();
        }
    }

    private void ChangePassword()
    {
        _wizard.Navigate<ChangePasswordPageViewModel>();
    }

    private void ExportWords()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            var words = _dbProvider.GetAll<Word>().Where(x => x.UserName == _session.User.Name);
            _dictionaryService.Export(saveFileDialog.FileName, words);
        }
    }
}
