using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace LexiTrainer.Messages;

public class OpenWindowMessage
{
    public ObservableObject ViewModel { get; set; }

    public bool? DialogResult { get; set; }
}
