using LexiTrainer.Db;
using LexiTrainer.Db.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LexiTrainer.Services;

public class DictionaryService : IDictionaryService
{
    private readonly IDatabaseProvider _dbProvider;
    private readonly Session _session;

    public DictionaryService(IDatabaseProvider dbProvider, Session session)
    {
        _dbProvider = dbProvider;
        _session = session;
    }

    public bool Import(string fileName)
    {
        var words = new List<Word>();
        var userWords = _dbProvider.GetAll<Word>().Where(x => x.UserName == _session.User.Name);
        using var reader = new StreamReader(fileName);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var columns = line.Split(';');

            // TODO: Add corresponding exception.
            if (columns.Length < 4 || columns.Length > 5)
                throw new Exception();

            var word = new Word
            {
                UserName = _session.User.Name,
                Language = columns[0],
                Theme = columns[1],
                Original = columns[2],
                Translation = columns[3],
                Description = columns.Length > 4 ? columns[4] : null
            };

            if (!word.IsValid())
            {
                // TODO: Add corresponding exception.
                throw new Exception();
            }

            if (!userWords.Contains(word, WordsEqualityComparer.Instance))
                words.Add(word);
        }

        if (words.Count > 0)
        {
            _dbProvider.InsertCollection(words);
            return true;
        }

        return false;
    }

    public void Export(string fileName, IEnumerable<Word> words)
    {
        using var writer = new StreamWriter(fileName);
        foreach (var word in words)
        {
            var line = string.IsNullOrEmpty(word.Description) ?
                $"{word.Language};{word.Theme};{word.Original};{word.Translation}" :
                $"{word.Language};{word.Theme};{word.Original};{word.Translation};{word.Description}";
            writer.WriteLine(line);
        }
    }
}
