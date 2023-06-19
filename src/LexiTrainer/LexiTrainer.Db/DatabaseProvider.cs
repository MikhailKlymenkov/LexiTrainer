using LexiTrainer.Db.Model;
using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace LexiTrainer.Db;

public class DatabaseProvider : IDatabaseProvider
{
    // TODO: Move connection string to appsettings.json
    private const string ConnectionString = "Filename=data.db";

    public void Insert<T>(T obj) where T : IdenticalObject
    {
        using var db = new LiteDatabase(ConnectionString);

        var collection = db.GetCollection<T>();
        collection.Insert(obj);
    }

    public void InsertCollection<T>(IEnumerable<T> collection) where T : IdenticalObject
    {
        using var db = new LiteDatabase(ConnectionString);

        var dbCollection = db.GetCollection<T>();
        dbCollection.InsertBulk(collection);
    }

    public T Get<T>(int id) where T : IdenticalObject
    {
        using var db = new LiteDatabase(ConnectionString);

        var collection = db.GetCollection<T>();
        return collection.FindById(id);
    }

    public IEnumerable<T> GetAll<T>() where T : IdenticalObject
    {
        using var db = new LiteDatabase(ConnectionString);

        var collection = db.GetCollection<T>();
        return collection.FindAll().ToList();
    }

    public bool Update<T>(T obj) where T : IdenticalObject
    {
        using var db = new LiteDatabase(ConnectionString);

        var collection = db.GetCollection<T>();
        return collection.Update(obj);
    }

    public bool Delete<T>(int id) where T : IdenticalObject
    {
        using var db = new LiteDatabase(ConnectionString);

        var collection = db.GetCollection<T>();
        return collection.Delete(id);
    }

    public int UpsertCollection<T>(IEnumerable<T> collection) where T : IdenticalObject
    {
        using var db = new LiteDatabase(ConnectionString);

        var dbCollection = db.GetCollection<T>();
        return dbCollection.Upsert(collection);
    }
}