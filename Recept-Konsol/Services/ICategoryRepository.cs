using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Lagring av kategorier. Regler ligger i CategoryService, inte här.
/// </summary>

public interface ICategoryRepository
{
    List<Category> GetAll();
    Category? Get(int id);
    void Add(Category category);
    void Update(Category category);
    void Delete(int id);
}