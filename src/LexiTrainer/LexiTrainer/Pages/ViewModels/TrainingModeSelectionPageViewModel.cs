using LexiTrainer.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace LexiTrainer.Pages.ViewModels;

public class TrainingModeSelectionPageViewModel : ObservableObject
{
    private IWizard _wizard;
    private Session _session;
    private TrainingMode _trainingMode;

    public TrainingModeSelectionPageViewModel(IWizard wizard, Session session)
    {
        _wizard = wizard;
        _session = session;

        TrainingModes = (IEnumerable<TrainingMode>)Enum.GetValues(typeof(TrainingMode));
        StartTrainingCommand = new RelayCommand(StartTraining);
    }

    public TrainingMode TrainingMode
    {
        get { return _trainingMode; }
        set 
        {
            if (SetProperty(ref _trainingMode, value))
            {
                _session.TrainingMode = value;
            }
        }
    }

    public IEnumerable<TrainingMode> TrainingModes { get; }

    public ICommand StartTrainingCommand { get; }

    private void StartTraining()
    {
        _wizard.Navigate<WordLearningPageViewModel>();
    }
}
