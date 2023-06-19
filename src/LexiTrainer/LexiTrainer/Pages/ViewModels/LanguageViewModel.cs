using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace LexiTrainer.Pages.ViewModels;

public class LanguageViewModel : ObservableObject
{
    private string _name;
    private int _wordsCount;

    public string Name 
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public int WordsCount
    {
        get => _wordsCount;
        set => SetProperty(ref _wordsCount, value);
    }
}
