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
    private const int MaxChefs = 5;

    private readonly MongoContext _context;
    private readonly IMongoCollection<Recipe> _recipes;
    private readonly IMongoCollection<Chef> _chefs;

    public MongoRecipeRepository(MongoContext context)
    {
        _context = context;
        _recipes = context.GetCollection<Recipe>("recipes");
        _chefs = context.GetCollection<Chef>("chefs");
    }

    public List<Recipe> GetAllRecipes() =>
        _recipes.Find(FilterDefinition<Recipe>.Empty).SortBy(r => r.Id).ToList();

    public Recipe? GetRecipe(int id) =>
        _recipes.Find(r => r.Id == id).FirstOrDefault();

    public void AddRecipe(Recipe recipe)
    {
        recipe.Id = _context.GetNextId("recipes");
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

    // Flyttar alla recept från en kategori till en annan i ett enda steg (önskemål 3).
    public void MoveRecipesToCategory(int fromCategoryId, int toCategoryId) =>
        _recipes.UpdateMany(
            r => r.CategoryId == fromCategoryId,
            Builders<Recipe>.Update.Set(r => r.CategoryId, toCategoryId));

    public List<Chef> GetAllChefs() =>
        _chefs.Find(FilterDefinition<Chef>.Empty).SortBy(c => c.Id).ToList();

    public void AddChef(Chef chef)
    {
        // Beställningen: programmet ska klara 1–5 kockar.
        if (_chefs.CountDocuments(FilterDefinition<Chef>.Empty) >= MaxChefs)
        {
            throw new ArgumentException($"Programmet klarar högst {MaxChefs} kockar.");
        }

        chef.Id = _context.GetNextId("chefs");
        _chefs.InsertOne(chef);
    }
}
