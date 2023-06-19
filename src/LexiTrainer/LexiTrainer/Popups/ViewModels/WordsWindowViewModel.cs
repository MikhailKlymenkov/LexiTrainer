using LexiTrainer.Db;
using LexiTrainer.Db.Model;
using LexiTrainer.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LexiTrainer.Popups.ViewModels;

public class WordsWindowViewModel : ObservableObject
{
    private IDatabaseProvider _dbProvider;
    private Session _session;
    private bool _isNewDictionary;
    private bool _areDetailsVisible;
    private bool _validateByLength;
    private string _dictionaryTheme;

    public WordsWindowViewModel(IEnumerable<Word> words, bool areDetailsVisible, bool validateByLength, IDatabaseProvider dbProvider, Session session)
    {
        _dbProvider = dbProvider;
        _session = session;
        _validateByLength = validateByLength;
        IsNewDictionary = !words.Any();
        AreDetailsVisible = areDetailsVisible;
        if (AreDetailsVisible)
        {
            words = words.OrderBy(x => x.Language);
        }
        else
        {
            if(!IsNewDictionary)
            {
                DictionaryTheme = words.First().Theme;
            }
            words = words.OrderBy(x => x.Original);
        }

        foreach (var word in words)
        {
            WordViewModels.Add(new WordViewModel
            {
                Id = word.Id,
                Language = word.Language,
                Theme = word.Theme,
                Original = word.Original,
                Translation = word.Translation,
                Description = word.Description,
                LearningCount = word.LearningCount
            });
        }

        SaveCommand = new RelayCommand(Save);
    }

    public delegate WordsWindowViewModel Factory(IEnumerable<Word> words, bool areDetailsVisible, bool validateByLength);

    public ICommand SaveCommand { get; }

    public ObservableCollection<WordViewModel> WordViewModels { get; } = new();

    public bool IsNewDictionary
    {
        get { return _isNewDictionary; }
        set { SetProperty(ref _isNewDictionary, value); }
    }

    public bool AreDetailsVisible
    {
        get { return _areDetailsVisible; }
        set { SetProperty(ref _areDetailsVisible, value); }
    }

    public string DictionaryTheme
    {
        get { return _dictionaryTheme; }
        set { SetProperty(ref _dictionaryTheme, value); }
    }

    private void Save()
    {
        var words = TryConvertToWords(WordViewModels, out IEnumerable<Word> deletedWords, out bool areValid);
        if (!areValid)
        {
            MessageBox.Show("Invalid dictionary");
            return;
        }

        var oldWords = _dbProvider.GetAll<Word>().Where(x => x.UserName == _session.User.Name);
        foreach (var word in words.Where(x => x.Id == 0))
        {
            if (oldWords.Contains(word, WordsEqualityComparer.Instance))
            {
                MessageBox.Show("Some words already exist");
                return;
            }
        }

        foreach(var word in deletedWords)
        {
            _dbProvider.Delete<Word>(word.Id);
        }
        _dbProvider.UpsertCollection(words);
        WeakReferenceMessenger.Default.Send(new CloseWordsWindowMessage());
    }

    private IEnumerable<Word> TryConvertToWords(IEnumerable<WordViewModel> wordViewModels, out IEnumerable<Word> deletedWords, out bool areValid)
    {
        if (!wordViewModels.Any() ||
            (IsNewDictionary && string.IsNullOrWhiteSpace(DictionaryTheme)))
        {
            areValid = false;
            deletedWords = null;
            return null;
        }

        var words = wordViewModels.Select(x => new Word
        {
            Id = x.Id,
            UserName = _session.User.Name,
            Language = IsNewDictionary || !AreDetailsVisible ? _session.Language : x.Language,
            Theme = AreDetailsVisible ?
                    IsNewDictionary ? DictionaryTheme : x.Theme :
                    DictionaryTheme,
            Original = x.Original,
            Translation = x.Translation,
            Description = x.Description,
            LearningCount = x.LearningCount
        });
        deletedWords = words.Where(x => x.IsEmpty());
        words = words.Where(x => !x.IsEmpty());

        areValid = words.Distinct(WordsEqualityComparer.Instance).Count() == words.Count();
        if (!areValid)
        {
            return null;
        }

        foreach (var word in words)
        {
            areValid = word.IsValid();
            if (!areValid)
            {
                return null;
            }
        }

        if(_validateByLength)
        {
            int length = words.First().Translation.Length;
            foreach (var word in words)
            {
                areValid = word.Translation.Length == length;
                if (!areValid)
                {
                    return null;
                }
            }
        }

        return words;
    }
}
