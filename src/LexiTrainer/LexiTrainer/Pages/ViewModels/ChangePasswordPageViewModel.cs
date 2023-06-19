using LexiTrainer.Db;
using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.Input;
using System.Text;
using System;
using System.Windows.Input;
using System.Security.Cryptography;

namespace LexiTrainer.Pages.ViewModels;

public class ChangePasswordPageViewModel : ErrorViewModel
{
    private IWizard _wizard;
    private Session _session;
    private IDatabaseProvider _dbProvider;
    private string _oldPassword;
    private string _newPassword;
    private string _repeatPassword;

    public ChangePasswordPageViewModel(IWizard wizard, Session session, IDatabaseProvider dbProvider)
    {
        _wizard = wizard;
        _session = session;
        _dbProvider = dbProvider;

        ChangePasswordCommand = new RelayCommand(ChangePassword);
    }

    public string OldPassword
    {
        get { return _oldPassword; }
        set { SetProperty(ref _oldPassword, value); }
    }

    public string NewPassword
    {
        get { return _newPassword; }
        set { SetProperty(ref _newPassword, value); }
    }

    public string RepeatPassword
    {
        get { return _repeatPassword; }
        set { SetProperty(ref _repeatPassword, value); }
    }

    public ICommand ChangePasswordCommand { get; }

    private void ChangePassword()
    {
        // TODO: Use validation attributes.

        ClearErrors();
        
        if (string.IsNullOrWhiteSpace(OldPassword))
        {
            AddValidationError(nameof(OldPassword), "Old password is required.");
            return;
        }
        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            AddValidationError(nameof(NewPassword), "New password is required.");
            return;
        }
        else if (NewPassword != RepeatPassword)
        {
            AddValidationError(nameof(RepeatPassword), "Passwords do not match.");
            return;
        }

        // TODO: Move hash logic to the separate service.
        // Check if the old password matches the current user's password
        byte[] oldPasswordBytes = Encoding.UTF8.GetBytes(OldPassword);
        using var sha256 = SHA256.Create();
        byte[] hashedOldPasswordBytes = sha256.ComputeHash(oldPasswordBytes);
        string hashedOldPassword = Convert.ToBase64String(hashedOldPasswordBytes);

        if (_session.User.Password != hashedOldPassword)
        {
            AddValidationError(nameof(OldPassword), "Old password is incorrect.");
            return;
        }

        // Hash the new password
        byte[] newPasswordBytes = Encoding.UTF8.GetBytes(NewPassword);
        byte[] hashedNewPasswordBytes = sha256.ComputeHash(newPasswordBytes);
        string hashedNewPassword = Convert.ToBase64String(hashedNewPasswordBytes);

        _session.User.Password = hashedNewPassword;
        _session.User.IsCurrent = false;
        _dbProvider.Update(_session.User);

        _session.Clear();
        _wizard.Navigate<LoginPageViewModel>();
    }

    private void ClearErrors()
    {
        ClearErrors(nameof(OldPassword));
        ClearErrors(nameof(NewPassword));
        ClearErrors(nameof(RepeatPassword));
    }
}

