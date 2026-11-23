using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Kategorier och veckogränser per kategori.
/// </summary>
public interface ICategoryService
{
    List<Category> GetAll();
    void Add(string name);

    /// <summary>Tar bort en kategori. Recept i kategorin flyttas till "Övrigt" (önskemål 3).</summary>
    void Delete(int categoryId);

    /// <summary>Hur många gånger per vecka kategorin högst får förekomma i veckoplanen.</summary>
    void SetWeeklyLimit(int categoryId, int maxPerWeek);
}
