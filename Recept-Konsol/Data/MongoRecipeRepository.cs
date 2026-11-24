using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Recept.Models;
using Recept.Services;

namespace Recept.Data;

/// <summary>
/// Sparar recept och kockar i MongoDB. Följer samma kontrakt som låtsasdatabasen,
/// så menyn märker ingen skillnad.
/// </summary>
public class MongoRecipeRepository : IRecipeRepository
{
    private const string DatabaseName = "recept";
    private const int MaxChefs = 5;

    private readonly IMongoCollection<Recipe> _recipes;
    private readonly IMongoCollection<Chef> _chefs;
    private readonly IMongoCollection<BsonDocument> _counters;

    public MongoRecipeRepository(string connectionString)
    {
        MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

        var client = new MongoClient(settings);
        IMongoDatabase database = client.GetDatabase(DatabaseName);

        EnsureConnection(database);

        _recipes = database.GetCollection<Recipe>("recipes");
        _chefs = database.GetCollection<Chef>("chefs");
        _counters = database.GetCollection<BsonDocument>("counters");
    }

    public List<Recipe> GetAllRecipes() =>
        _recipes.Find(FilterDefinition<Recipe>.Empty).SortBy(r => r.Id).ToList();

    public Recipe? GetRecipe(int id) =>
        _recipes.Find(r => r.Id == id).FirstOrDefault();

    public void AddRecipe(Recipe recipe)
    {
        recipe.Id = GetNextId("recipes");
        _recipes.InsertOne(recipe);
    }

    public void DeleteRecipe(int id) =>
        _recipes.DeleteOne(r => r.Id == id);

    public List<Recipe> SearchByIngredient(string ingredientName)
    {
        // Regex.Escape gör att tecken som * och ( räknas som vanlig text,
        // så att användaren inte kan skicka in egna sökmönster (som SQL injection, fast för MongoDB).
        var pattern = new BsonRegularExpression(Regex.Escape(ingredientName), "i");
        FilterDefinition<Recipe> filter = Builders<Recipe>.Filter.Regex("Ingredients.Name", pattern);

        return _recipes.Find(filter).SortBy(r => r.Id).ToList();
    }

    public List<Chef> GetAllChefs() =>
        _chefs.Find(FilterDefinition<Chef>.Empty).SortBy(c => c.Id).ToList();

    public void AddChef(Chef chef)
    {
        // Beställningen: programmet ska klara 1–5 kockar.
        if (_chefs.CountDocuments(FilterDefinition<Chef>.Empty) >= MaxChefs)
        {
            throw new ArgumentException($"Programmet klarar högst {MaxChefs} kockar.");
        }

        chef.Id = GetNextId("chefs");
        _chefs.InsertOne(chef);
    }

    // Räknar upp ett nummer i databasen och lämnar ut det nya värdet,
    // så att varje recept och kock får ett eget heltal: 1, 2, 3 ...
    private int GetNextId(string counterName)
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
    private static void EnsureConnection(IMongoDatabase database)
    {
        try
        {
            database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
        }
        catch (Exception ex) when (ex is TimeoutException or MongoException)
        {
            throw new InvalidOperationException(
                "Kunde inte ansluta till databasen. Kontrollera internetanslutningen, " +
                "att din IP-adress finns i Atlas och att DB_CONNECTION är rätt.", ex);
        }
    }
}