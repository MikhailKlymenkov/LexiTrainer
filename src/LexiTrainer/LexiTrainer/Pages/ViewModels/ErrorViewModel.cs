using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using System.Collections;

namespace LexiTrainer.Pages.ViewModels;

public abstract class ErrorViewModel : ObservableObject, INotifyDataErrorInfo
{
    private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

    protected ErrorViewModel()
    {
    }

    public bool HasErrors => _errors.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public IEnumerable GetErrors(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || !_errors.ContainsKey(propertyName))
        {
            return null;
        }

        return _errors[propertyName];
    }

    protected void AddValidationError(string propertyName, string errorMessage)
    {
        if (!_errors.ContainsKey(propertyName))
        {
            _errors.Add(propertyName, new List<string>());
        }

        _errors[propertyName].Add(errorMessage);

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected void ClearErrors(string propertName)
    {
        if (_errors.Count > 0)
        {
            _errors.Clear();
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertName));
    }
}
