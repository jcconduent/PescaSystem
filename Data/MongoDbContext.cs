using MongoDB.Driver;
using PescaSystem.Models;

namespace PescaSystem.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext()
        {
            var client = new MongoClient("mongodb+srv://jc:elmismo@cluster0.bveyato.mongodb.net/pesca?retryWrites=true&w=majority");
            _database = client.GetDatabase("pesca");
        }

        public IMongoCollection<PescaLog> PescaLogs => _database.GetCollection<PescaLog>("PescaLogs");
    }
}

