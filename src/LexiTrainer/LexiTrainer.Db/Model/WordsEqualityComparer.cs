using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LexiTrainer.Db.Model;

public class WordsEqualityComparer : IEqualityComparer<Word>
{
    private static readonly WordsEqualityComparer _instance =
        new WordsEqualityComparer();

    private WordsEqualityComparer() { }

    public static WordsEqualityComparer Instance
    {
        get => _instance;
    }

    public bool Equals(Word x, Word y)
    {
        return x.Language.ToLower() == y.Language.ToLower() &&
               x.Original.ToLower() == y.Original.ToLower() &&
               x.Translation.ToLower() == y.Translation.ToLower();
    }

    public int GetHashCode([DisallowNull] Word word)
    {
        int hash = 17;
        hash = hash * 23 + word.Language?.ToLower().GetHashCode() ?? 0;
        hash = hash * 23 + word.Original?.ToLower().GetHashCode() ?? 0;
        hash = hash * 23 + word.Translation?.ToLower().GetHashCode() ?? 0;
        return hash;
    }
}
