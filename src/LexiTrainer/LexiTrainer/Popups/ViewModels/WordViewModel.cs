using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;

namespace LexiTrainer.Popups.ViewModels;

public class WordViewModel : ObservableObject
{
    private int _id;
    private string _language;
    private string _theme;
    private string _original;
    private string _translation;
    private string _description;
    private int _learningCount;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Language
    {
        get => _language;
        set => SetProperty(ref _language, value);
    }

    public string Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    public string Original
    {
        get => _original;
        set => SetProperty(ref _original, value);
    }

    public string Translation
    {
        get => _translation;
        set => SetProperty(ref _translation, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public int LearningCount
    {
        get => _learningCount;
        set => SetProperty(ref _learningCount, value);
    }
}
