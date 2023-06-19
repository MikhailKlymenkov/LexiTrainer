using LexiTrainer.Db;
using LexiTrainer.Db.Model;
using LexiTrainer.Extensions;
using LexiTrainer.Messages;
using LexiTrainer.Popups.ViewModels;
using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using IDictionaryService = LexiTrainer.Services.IDictionaryService;

namespace LexiTrainer.Pages.ViewModels;

public class DictionarySelectionPageViewModel : TrainingPageViewModel
{
    private IWizard _wizard;
    private Session _session;
    private IDatabaseProvider _dbProvider;
    private WordsWindowViewModel.Factory _wordsWindowViewModelFactory;
    private IDictionaryService _dictionaryService;
    private DictionaryGroupMode _groupMode;

    public DictionarySelectionPageViewModel(
        IWizard wizard,
        Session session,
        IDatabaseProvider dbProvider,
        WordsWindowViewModel.Factory wordsWindowViewModelFactory,
        IDictionaryService dictionaryService) : base(wizard, session)
    {
        _wizard = wizard;
        _session = session;
        _dbProvider = dbProvider;
        _wordsWindowViewModelFactory = wordsWindowViewModelFactory;
        _dictionaryService = dictionaryService;

        ImportDictionaryCommand = new RelayCommand(ImportDictionary);
        SeeDictionaryCommand = new RelayCommand<string>(SeeDictionary);
        AddDictionaryCommand = new RelayCommand(AddDictionary);
        DeleteDictionaryCommand = new RelayCommand<string>(DeleteDictionary);
        SelectDictionaryCommand = new RelayCommand<string>(SelectDictionary);
        ExportDictionaryCommand = new RelayCommand<string>(ExportDictionary);

        GroupModes = (IEnumerable<DictionaryGroupMode>)Enum.GetValues(typeof(DictionaryGroupMode));
        Refresh();
    }

    public DictionaryGroupMode GroupMode
    {
        get { return _groupMode; }
        set 
        {
            if(SetProperty(ref _groupMode, value))
            {
                Refresh();
            }
        }
    }

    public IEnumerable<DictionaryGroupMode> GroupModes { get; }

    public ICommand ImportDictionaryCommand { get; }

    public ICommand AddDictionaryCommand { get; }

    public ICommand DeleteDictionaryCommand { get; }

    public ICommand SeeDictionaryCommand { get; }

    public ICommand SelectDictionaryCommand { get; }

    public ICommand ExportDictionaryCommand { get; }

    // TODO: Add DictionaryViewModel
    public ObservableCollection<string> Dictionaries { get; } = new();

    private void Refresh()
    {
        Dictionaries.Clear();
        IEnumerable<string> dictionaries = Enumerable.Empty<string>();
        var words = _dbProvider.GetAll<Word>().Where(x => x.UserName == _session.User.Name && x.Language == _session.Language);
        switch (GroupMode)
        {
            case DictionaryGroupMode.Theme:
                dictionaries = words.Select(x => x.Theme.ToLower().FirstCharToUpper())
                                .OrderBy(x => x)
                                .Distinct();
                break;
            case DictionaryGroupMode.Length:
                dictionaries = words.OrderBy(x => x.Translation.Length)
                                .Select(x => $"Length: {x.Translation.Length}")
                                .Distinct();
                break;
        }

        foreach (var dictionary in dictionaries)
        {
            Dictionaries.Add(dictionary);
        }
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

    private void SeeDictionary(string dictionary)
    {
        bool validateByLength = GroupMode == DictionaryGroupMode.Length;
        var words = GetWordsToTraining(dictionary);
        var wordsWindowViewModel = _wordsWindowViewModelFactory(words, false, validateByLength);
        var openWindowMessage = new OpenWindowMessage
        {
            ViewModel = wordsWindowViewModel
        };
        WeakReferenceMessenger.Default.Send(openWindowMessage);
    }

    private void AddDictionary()
    {
        var wordsWindowViewModel = _wordsWindowViewModelFactory(Enumerable.Empty<Word>(), false, false);
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

    private void DeleteDictionary(string dictionary)
    {
        MessageBoxResult result = MessageBox.Show(
            "Are you sure you want to delete this dictionary?",
            "Confirm",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            var words = GetWordsToTraining(dictionary);
            foreach (var word in words)
            {
                _dbProvider.Delete<Word>(word.Id);
            }

            Refresh();
        }
    }

    private void SelectDictionary(string dictionary)
    {
        _session.WordsToTraining = GetWordsToTraining(dictionary).ToArray();

        if (_session.WordsToTraining.Length < 4)
        {
            _session.TrainingMode = TrainingMode.Hard;
            _wizard.Navigate<WordLearningPageViewModel>();
        }
        else
        {
            _wizard.Navigate<TrainingModeSelectionPageViewModel>();
        }
    }

    private void ExportDictionary(string dictionary)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            var words = GetWordsToTraining(dictionary);
            _dictionaryService.Export(saveFileDialog.FileName, words);
        }
    }

    private IEnumerable<Word> GetWordsToTraining(string dictionary)
    {
        IEnumerable<Word> words = _dbProvider.GetAll<Word>()
                                    .Where(x => x.UserName == _session.User.Name && x.Language == _session.Language);
        switch (GroupMode)
        {
            case DictionaryGroupMode.Theme:
                words = words.Where(x => x.Theme.ToLower() == dictionary.ToLower());
                break;
            case DictionaryGroupMode.Length:
                // TODO: Save length in the DictionaryViewModel.
                int length = int.Parse(dictionary.Split(' ').Last());
                words = words.Where(x => x.Translation.Length == length);
                break;
        }

        return words;
    }
}
