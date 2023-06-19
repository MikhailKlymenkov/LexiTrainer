namespace LexiTrainer.Db.Model;

public class User : IdenticalObject
{
    public string Name { get; set; }

    public string Password { get; set; }

    public bool IsCurrent { get; set;}
}
