using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Ska byggas av er. Menyn anropar redan de här metoderna och visar
/// "Inte implementerat ännu" så länge de kastar NotImplementedException.
/// </summary>
public class CategoryService : ICategoryService
{
    // Du behöver bygga ut detta mer: standardlistan är
    // Frukost, Lunch, Middag, Efterrätt, Bakning och Övrigt.
    public List<Category> GetAll() => throw new NotImplementedException();

    // Tom kategori ska ge ett tydligt felmeddelande. Kasta ArgumentException, så visar menyn texten.
    public void Add(string name) => throw new NotImplementedException();

    public void Delete(int categoryId) => throw new NotImplementedException();

    public void SetWeeklyLimit(int categoryId, int maxPerWeek) => throw new NotImplementedException();
}
