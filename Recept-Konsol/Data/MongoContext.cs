using MongoDB.Bson;
using MongoDB.Driver;

namespace Recept.Data;

/// <summary>
/// Gemensam anslutning till MongoDB för alla repositories.
/// Här finns också räknaren som ger varje sak ett eget nummer (1, 2, 3 ...),
/// så att anslutningen och numreringen bara finns på ett ställe.
/// </summary>
public class MongoContext
{
    private const string DatabaseName = "recept";

    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<BsonDocument> _counters;

    public MongoContext(string connectionString)
    {
        MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);

        // Ge upp efter 5 sekunder i stället för standardens 30, så att programmet inte verkar hänga sig.
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

        var client = new MongoClient(settings);
        _database = client.GetDatabase(DatabaseName);

        EnsureConnection();

        _counters = _database.GetCollection<BsonDocument>("counters");
    }

    public IMongoCollection<T> GetCollection<T>(string name) =>
        _database.GetCollection<T>(name);

    // Räknar upp ett nummer i databasen och lämnar ut det nya värdet.
    // Sker i ett enda steg i databasen, så två anrop kan aldrig få samma nummer.
    public int GetNextId(string counterName)
    {
        FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", counterName);
        UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Inc("value", 1);
        var options = new FindOneAndUpdateOptions<BsonDocument>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After,
        };

        BsonDocument counter = _counters.FindOneAndUpdate(filter, update, options);
        return counter["value"].AsInt32;
    }

    // Kontrollerar direkt vid start att databasen svarar,
    // så att användaren får ett tydligt meddelande i stället för en krasch senare.
    private void EnsureConnection()
    {
        try
        {
            _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
        }
        catch (Exception ex) when (ex is TimeoutException or MongoException)
        {
            throw new InvalidOperationException(
                "Kunde inte ansluta till databasen. Kontrollera internetanslutningen, " +
                "att din IP-adress finns i Atlas och att DB_CONNECTION är rätt.", ex);
        }
    }
}
