using LexiTrainer.Db;
using LexiTrainer.Db.Model;
using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.Input;
using System.Text;
using System;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Linq;

namespace LexiTrainer.Pages.ViewModels;

public class RegistrationPageViewModel : ErrorViewModel
{
    private IWizard _wizard;
    private Session _session;
    private IDatabaseProvider _dbProvider;
    private string _name;
    private string _password; 
    private string _repeatPassword;
    private bool _IsNavigateToLoginVisible;

    public RegistrationPageViewModel(IWizard wizard, Session session, IDatabaseProvider dbProvider)
    {
        _wizard = wizard;
        _session = session;
        _dbProvider = dbProvider;

        IsNavigateToLoginVisible = session.AreUsersExist;
        NavigateToLoginCommand = new RelayCommand(NavigateToLoginPage);
        RegistrationCommand = new RelayCommand(Register);
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

    public string RepeatPassword
    {
        get { return _repeatPassword; }
        set { SetProperty(ref _repeatPassword, value); }
    }

    public bool IsNavigateToLoginVisible
    {
        get { return _IsNavigateToLoginVisible; }
        set { SetProperty(ref _IsNavigateToLoginVisible, value); }
    }

    public ICommand RegistrationCommand { get; }

    public ICommand NavigateToLoginCommand { get; }

    private void NavigateToLoginPage()
    {
        _wizard.Navigate<LoginPageViewModel>();
    }

    private void Register()
    {
        // TODO: Use validation attributes.

        ClearErrors();

        // Validate the Name property
        if (string.IsNullOrWhiteSpace(Name))
        {
            AddValidationError(nameof(Name), "Name is required.");
            return;
        }

        // Validate the Password property
        if (string.IsNullOrWhiteSpace(Password))
        {
            AddValidationError(nameof(Password), "Password is required.");
            return;
        }
        else if (Password != RepeatPassword)
        {
            AddValidationError(nameof(RepeatPassword), "Passwords do not match.");
            return;
        }

        // Check if the user already exists
        var existingUser = _dbProvider.GetAll<User>().FirstOrDefault(u => u.Name == Name);
        if (existingUser != null)
        {
            AddValidationError(nameof(Name), "User with this name already exists.");
            return;
        }

        // TODO: Move hash logic to the separate service.
        // Hash the password
        byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
        using var sha256 = SHA256.Create();
        byte[] hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
        string hashedPassword = Convert.ToBase64String(hashedPasswordBytes);

        var user = new User
        {
            Name = Name,
            Password = hashedPassword,
            IsCurrent = true
        };

        // Save user to database
        _dbProvider.Insert(user);
        _session.AreUsersExist = true;
        _session.User = user;

        _wizard.Navigate<LanguageSelectionPageViewModel>();
    }

    private void ClearErrors()
    {
        ClearErrors(nameof(Name));
        ClearErrors(nameof(Password));
        ClearErrors(nameof(RepeatPassword));
    }
}
