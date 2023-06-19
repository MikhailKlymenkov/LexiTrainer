using Autofac;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows;

namespace LexiTrainer.Services;

public class Wizard : ObservableObject, IWizard
{
    private ObservableObject _page;

    public ObservableObject Page
    {
        get => _page;
        set => SetProperty(ref _page, value);
    }


    public void Navigate<T>() where T : ObservableObject
    {
        Page = ((App)Application.Current).AppContainer.Resolve<T>();
    }
}

