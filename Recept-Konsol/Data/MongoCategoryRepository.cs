using MongoDB.Driver;
using Recept.Models;
using Recept.Services;

namespace Recept.Data;

/// <summary>
/// Sparar kategorier i MongoDB. Bara lagring: reglerna ligger i CategoryService.
/// </summary>
public class MongoCategoryRepository : ICategoryRepository
{
    private readonly MongoContext _context;
    private readonly IMongoCollection<Category> _categories;

    public MongoCategoryRepository(MongoContext context)
    {
        _context = context;
        _categories = context.GetCollection<Category>("categories");
    }

    public List<Category> GetAll() =>
        _categories.Find(FilterDefinition<Category>.Empty).SortBy(c => c.Id).ToList();

    public Category? Get(int id) =>
        _categories.Find(c => c.Id == id).FirstOrDefault();

    public void Add(Category category)
    {
        category.Id = _context.GetNextId("categories");
        _categories.InsertOne(category);
    }

    public void Update(Category category) =>
        _categories.ReplaceOne(c => c.Id == category.Id, category);

    public void Delete(int id) =>
        _categories.DeleteOne(c => c.Id == id);
}
