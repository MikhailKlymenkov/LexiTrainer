namespace LexiTrainer.Db.Model;

public static class WordExtensions
{
    public static bool IsValid(this Word word)
    {
        return !string.IsNullOrEmpty(word.UserName) &&
               !string.IsNullOrEmpty(word.Language) &&
               !string.IsNullOrEmpty(word.Theme) &&
               !string.IsNullOrEmpty(word.Original) &&
               !string.IsNullOrEmpty(word.Translation);
    }

    public static bool IsEmpty(this Word word)
    {
        return string.IsNullOrEmpty(word.Original) &&
               string.IsNullOrEmpty(word.Translation);
    }
}
