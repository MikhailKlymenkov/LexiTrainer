using LexiTrainer.Db;
using LexiTrainer.Db.Model;
using LexiTrainer.Pages.ViewModels;
using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Linq;

namespace LexiTrainer;

public class MainViewModel : ObservableObject
{
    private IWizard _wizard;

    public MainViewModel(IWizard wizard, Session session, IDatabaseProvider dbProvider)
    {
        _wizard = wizard;

        var users = dbProvider.GetAll<User>();
        session.AreUsersExist = users.Any();

        if (!session.AreUsersExist)
        {
            _wizard.Navigate<RegistrationPageViewModel>();
        }
        else
        {
            var currentUser = users.FirstOrDefault(x => x.IsCurrent);
            if (currentUser == null)
            {
                _wizard.Navigate<LoginPageViewModel>();
            }
            else
            {
                session.User = currentUser;
                _wizard.Navigate<LanguageSelectionPageViewModel>();
            }
        }
    }

    public IWizard Wizard
    {
        get { return _wizard; }
        set { SetProperty(ref _wizard, value); }
    }
}

