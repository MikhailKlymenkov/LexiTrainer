using LexiTrainer.Db.Model;
using LexiTrainer.Pages.ViewModels;

namespace LexiTrainer;

public record Session
{
    public bool AreUsersExist { get; set; }

    public User User { get; set; }

    public string Language { get; set; }

    public Word[] WordsToTraining { get; set; }

    public int LearnedWordsCount { get; set; }

    public TrainingMode TrainingMode { get; set; }

    public void Clear()
    {
        User = null;
        ClearTrainingData();
    }

    public void ClearTrainingData()
    {
        Language = null;
        WordsToTraining = null;
        LearnedWordsCount = 0;
        TrainingMode = TrainingMode.Easy;
    }
}