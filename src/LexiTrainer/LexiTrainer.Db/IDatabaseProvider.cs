using LexiTrainer.Db.Model;
using System.Collections.Generic;

namespace LexiTrainer.Db
{
    public interface IDatabaseProvider
    {
        bool Delete<T>(int id) where T : IdenticalObject;

        T Get<T>(int id) where T : IdenticalObject;

        IEnumerable<T> GetAll<T>() where T : IdenticalObject;

        void Insert<T>(T obj) where T : IdenticalObject;

        void InsertCollection<T>(IEnumerable<T> collection) where T : IdenticalObject;

        bool Update<T>(T obj) where T : IdenticalObject;

        int UpsertCollection<T>(IEnumerable<T> collection) where T : IdenticalObject;
    }
}