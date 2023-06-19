using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace LexiTrainer.Services;

public interface IWizard
{
    ObservableObject Page { get; }

    void Navigate<T>() where T : ObservableObject;
}

