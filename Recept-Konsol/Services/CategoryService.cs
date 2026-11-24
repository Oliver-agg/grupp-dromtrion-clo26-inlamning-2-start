using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Ska byggas av er. GetAll visar demodata så att sidan går att titta på.
/// Övriga metoder kastar NotImplementedException, och menyn visar då "Inte implementerat ännu".
/// </summary>
public class CategoryService : ICategoryService
{
    // DEMO: en fast lista som inte sparas någonstans. Ersätt med kategorier från databasen.
    // Standardlistan ska alltid finnas: Frukost, Lunch, Middag, Efterrätt, Bakning och Övrigt.
    public List<Category> GetAll() =>
    [
        new Category { Id = 1, Name = "Frukost" },
        new Category { Id = 2, Name = "Lunch" },
        new Category { Id = 3, Name = "Middag" },
        new Category { Id = 4, Name = "Efterrätt" },
        new Category { Id = 5, Name = "Bakning" },
        new Category { Id = 6, Name = "Övrigt" },
    ];

    // Tom kategori ska ge ett tydligt felmeddelande. Kasta ArgumentException, så visar menyn texten.
    public void Add(string name) => throw new NotImplementedException();

    public void Delete(int categoryId) => throw new NotImplementedException();

    public void SetWeeklyLimit(int categoryId, int maxPerWeek) => throw new NotImplementedException();
}
