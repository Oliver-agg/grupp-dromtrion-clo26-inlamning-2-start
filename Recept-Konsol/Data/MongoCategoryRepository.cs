using MongoDB.Driver;
using Recept.Models;
using Recept.Services;

namespace Recept.Data;

public class MongoCategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<Category> _categories;
    public MongoCategoryRepository(string connectionString)
    {
        var client = new MongoClient(connectionString);
        _categories = client.GetDatabase("recept").GetCollection<Category>("categories");
    }

    public List<Category> GetAll() => _categories.Find(FilterDefinition<Category>.Empty).SortBy(c => c.Id).ToList();

    public Category? Get(int id) => _categories.Find(c => c.Id == id).FirstOrDefault();

    public void Add(Category category)
    {
        Category? last = _categories.Find(FilterDefinition<Category>.Empty).SortByDescending(c => c.Id).Limit(1).FirstOrDefault();
        category.Id = (last?.Id ?? 0) + 1;
        _categories.InsertOne(category);
    }

    public void Update(Category category) => _categories.ReplaceOne(c => c.Id == category.Id, category);

    public void Delete(int id) => _categories.DeleteOne(c => c.Id == id);
}