using LexiTrainer.Db.Model;
using System.Collections.Generic;

namespace LexiTrainer.Services;

public interface IDictionaryService
{
    bool Import(string fileName);

    void Export(string fileName, IEnumerable<Word> words);
}
