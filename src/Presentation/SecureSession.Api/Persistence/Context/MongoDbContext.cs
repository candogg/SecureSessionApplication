using MongoDB.Driver;

namespace SecureSession.Api.Persistence.Context
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class MongoDbContext(IMongoClient client, string databaseName)
    {
        public IMongoClient Client { get; } = client;
        public IMongoDatabase Database { get; } = client.GetDatabase(databaseName);

        public IMongoCollection<T> GetCollection<T>(string name)
            => Database.GetCollection<T>(name);
    }
}
