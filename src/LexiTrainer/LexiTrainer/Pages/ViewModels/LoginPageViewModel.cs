using LexiTrainer.Db;
using LexiTrainer.Db.Model;
using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.Input;
using System.Linq;
using System.Text;
using System;
using System.Windows.Input;
using System.Security.Cryptography;

namespace LexiTrainer.Pages.ViewModels;

public class LoginPageViewModel : ErrorViewModel
{
    private IWizard _wizard;
    private Session _session;
    private IDatabaseProvider _dbProvider;
    private string _name;
    private string _password;

    public LoginPageViewModel(IWizard wizard, Session session, IDatabaseProvider dbProvider)
    {
        _wizard = wizard;
        _session = session;
        _dbProvider = dbProvider;

        NavigateToRegistrationCommand = new RelayCommand(NavigateToRegistrationPage);
        SignInCommand = new RelayCommand(SignIn);
    }

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public string Password
    {
        get { return _password; }
        set { SetProperty(ref _password, value); }
    }

    public ICommand SignInCommand { get; }

    public ICommand NavigateToRegistrationCommand { get; }

    private void NavigateToRegistrationPage()
    {
        _wizard.Navigate<RegistrationPageViewModel>();
    }

    private void SignIn()
    {
        // TODO: Use validation attributes.

        ClearErrors(nameof(Password));

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Password))
        {
            AddValidationError(nameof(Password), "Incorrect name or password.");
            return;
        }

        var user = _dbProvider.GetAll<User>().FirstOrDefault(u => u.Name == Name);
        if (user == null)
        {
            AddValidationError(nameof(Password), "Incorrect name or password.");
            return;
        }

        // TODO: Move hash logic to the separate service.
        // Hash the input password
        byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
        using var sha256 = SHA256.Create();
        byte[] hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
        string hashedPassword = Convert.ToBase64String(hashedPasswordBytes);

        // Compare the hashed passwords
        if (hashedPassword != user.Password)
        {
            AddValidationError(nameof(Password), "Incorrect name or password.");
            return;
        }

        user.IsCurrent = true;
        _dbProvider.Update(user);
        _session.User = user;

        _wizard.Navigate<LanguageSelectionPageViewModel>();
    }
}
