using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Regler för kategorier: Namn, "Övrigt" som skyddad kategori och veckogränser
/// </summary>

public class CategoryService : ICategoryService
{
    private const string FallbackName = "Övrigt";
    private readonly ICategoryRepository _repository;
    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository; 
    }

    public List<Category> GetAll() => _repository.GetAll();

    public void Add(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Kategorin måste ha ett namn");
        }
        name = name.Trim();

        if(GetAll().Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Kategorin \"{name}\" finns redan");
        }
        _repository.Add(new Category { Name = name });
    }

    public void Delete(int categoryId)
    {
        Category category = GetExisting(categoryId);

        if (category.Name == FallbackName)
        {
            throw new ArgumentException($"Kategorin \"{FallbackName}\" kan inte tas bort.");
        }

        //TODO :: flytta kategorins recept till "Övrigt" . Kräver CategoryId på Recipe
        _repository.Delete(categoryId);
    }

    public void SetWeeklyLimit(int categoryId, int maxPerWeek)
    {
        if(maxPerWeek < 1)
        {
            throw new ArgumentException("Gränsen måste vara minst 1 gång per vecka.");
        }

        Category category = GetExisting(categoryId);
        category.MaxPerWeek = maxPerWeek;
        _repository.Update(category);
    }

    private Category GetExisting(int categoryId) => 
        _repository.Get(categoryId) 
        ?? throw new ArgumentException("Det finns ingen kategori med det numret");
}