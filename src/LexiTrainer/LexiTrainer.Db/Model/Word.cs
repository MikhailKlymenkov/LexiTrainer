using System.Collections.Generic;

namespace LexiTrainer.Db.Model;

public class Word : IdenticalObject
{
    public string UserName { get; set; }

    public string Language { get; set; }

    public string Theme { get; set; }

    public string Original { get; set; }

    public string Translation { get; set; }

    public string Description { get; set; }

    public int LearningCount { get; set; }
}
