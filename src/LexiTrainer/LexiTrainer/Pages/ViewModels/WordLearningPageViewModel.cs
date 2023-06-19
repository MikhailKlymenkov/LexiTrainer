using LexiTrainer.Db;
using LexiTrainer.Db.Model;
using LexiTrainer.Extensions;
using LexiTrainer.Messages;
using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LexiTrainer.Pages.ViewModels;

public class WordLearningPageViewModel : TrainingPageViewModel
{
    private readonly List<Word> _words = new List<Word>();
    private readonly Random _random = new Random();
    private List<string> _translations = new();
    private readonly IWizard _wizard;
    private readonly Session _session;
    private readonly IDatabaseProvider _dbProvider;
    private Word _currentWord;
    private string _word;
    private string _correctTranslation;
    private string _translation;
    private TrainingMode _trainingMode;
    private int _wordsLeft;
    private bool _isResultSuccessful;
    private string _aTranslation;
    private string _bTranslation;
    private string _cTranslation;
    private string _wordDescription;

    public WordLearningPageViewModel(IWizard wizard, Session session, IDatabaseProvider dbProvider)
            : base(wizard, session)
    {
        _wizard = wizard;
        _session = session;
        _dbProvider = dbProvider;

        CheckAnswerCommand = new RelayCommand(CheckAnswer);
        FinishCommand = new RelayCommand(Finish);
        NextWordCommand = new RelayCommand(ShowNextWord);
        SelectAnswerCommand = new RelayCommand<object>(SelectAnswer);

        Start();
    }

    public ICommand CheckAnswerCommand { get; }

    public ICommand FinishCommand { get; }

    public ICommand NextWordCommand { get; }

    public ICommand SelectAnswerCommand { get; }

    public string Word
    {
        get => _word;
        set => SetProperty(ref _word, value);
    }

    public string CorrectTranslation
    {
        get => _correctTranslation;
        set => SetProperty(ref _correctTranslation, value);
    }

    public string Translation
    {
        get => _translation;
        set => SetProperty(ref _translation, value);
    }

    public string ATranslation
    {
        get => _aTranslation;
        set => SetProperty(ref _aTranslation, value);
    }

    public string BTranslation
    {
        get => _bTranslation;
        set => SetProperty(ref _bTranslation, value);
    }

    public string CTranslation
    {
        get => _cTranslation;
        set => SetProperty(ref _cTranslation, value);
    }

    public TrainingMode TrainingMode
    {
        get => _trainingMode;
        private set => SetProperty(ref _trainingMode, value);
    }

    public int WordsLeft
    {
        get => _wordsLeft;
        set => SetProperty(ref _wordsLeft, value);
    }

    public bool IsResultSuccessful
    {
        get => _isResultSuccessful;
        set => SetProperty(ref _isResultSuccessful, value);
    }

    public string WordDescription 
    {
        get => _wordDescription;
        set => SetProperty(ref _wordDescription, value);
    }

    private void Start()
    {
        TrainingMode = _session.TrainingMode;
        WordsLeft = _session.WordsToTraining.Length > 10 ? 10 : _session.WordsToTraining.Length;

        AddWordsToLearn();
        ShowNextWord();
    }

    private void AddWordsToLearn()
    {
        int minLearningCount = _session.WordsToTraining.Select(x => x.LearningCount).Min();
        while (_words.Count != WordsLeft)
        {
            var priorityWords = _session.WordsToTraining.Where(x => x.LearningCount == minLearningCount);
            foreach (var word in priorityWords)
            {
                if (!_words.Contains(word))
                {
                    _words.Add(word);
                    if (_words.Count == WordsLeft)
                    {
                        break;
                    }
                }
            }

            ++minLearningCount;
        }

        _words.Shuffle();
    }

    private void SelectAnswer(object obj)
    {
        var answer = (string)obj;
        Translation = answer;
    }

    private void CheckAnswer()
    {
        if (string.IsNullOrEmpty(Translation))
        {
            MessageBox.Show(TrainingMode is TrainingMode.Easy ?
                "Select the translation!" :
                "Translation is required");
            return;
        }

        CorrectTranslation = _currentWord.Original;
        IsResultSuccessful = Translation.ToLower() == CorrectTranslation.ToLower();
        if (IsResultSuccessful)
        {
            _session.LearnedWordsCount++;
            _currentWord.LearningCount++;
        }
    }

    private void ShowNextWord()
    {
        if (WordsLeft <= 0)
        {
            return;
        }

        WeakReferenceMessenger.Default.Send(new NextWordShownMessage());
        IsResultSuccessful = false;
        CorrectTranslation = null;
        Translation = null;

        _currentWord = _words[_words.Count - WordsLeft];
        Word = _currentWord.Translation;
        WordDescription = _currentWord.Description;

        if (TrainingMode is TrainingMode.Easy)
        {
            _translations.Clear();
            _translations.Add(_currentWord.Original);
            var otherWords = _words.Where(x => x != _currentWord);
            for (int i = 0; i < 2; i++)
            {
                var word = otherWords.ElementAt(_random.Next(otherWords.Count()));
                _translations.Add(word.Original);
                otherWords = otherWords.Where(x => x != word);
            }

            _translations.Shuffle();
            ATranslation = _translations[0];
            BTranslation = _translations[1];
            CTranslation = _translations[2];
        }

        --WordsLeft;
    }

    private void Finish()
    {
        _dbProvider.UpsertCollection(_session.WordsToTraining);
        _wizard.Navigate<FinishTrainingPageViewModel>();
    }
}
