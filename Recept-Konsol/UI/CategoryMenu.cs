using Recept.Models;
using Recept.Services;

namespace Recept.UI;

/// <summary>
/// Menyval för kategorier och veckogränser. Logiken ligger i ICategoryService.
/// </summary>
public class CategoryMenu
{
    private readonly ICategoryService _categories;

    public CategoryMenu(ICategoryService categories)
    {
        _categories = categories;
    }

    public void Run()
    {
        new Menu("Kategorier")
            .Add("1", "Lista kategorier", ListCategories)
            .Add("2", "Lägg till kategori", AddCategory)
            .Add("3", "Ta bort kategori", DeleteCategory)
            .Add("4", "Sätt gräns per vecka", SetWeeklyLimit)
            .Run();
    }

    private void ListCategories()
    {
        foreach (Category category in _categories.GetAll())
        {
            string limit = category.MaxPerWeek is int max ? $"(högst {max} per vecka)" : "";
            Console.WriteLine($"{category.Id}. {category.Name}{limit}");
        }
    }

    private void AddCategory()
    {
        // Ingen kontroll av tom text här: den regeln hör hemma i CategoryService.
        string name = ConsoleInput.ReadText("Namn på kategorin");
        _categories.Add(name);
        Console.WriteLine($"Kategorin \"{name}\" är tillagd.");
    }

    private void DeleteCategory()
    {
        ListCategories();
        int id = ConsoleInput.ReadInt("Kategorins nummer", min: 1);

        if (!ConsoleInput.Confirm("Recepten i kategorin flyttas till \"Övrigt\". Fortsätta?"))
        {
            Console.WriteLine("Inget togs bort.");
            return;
        }

        _categories.Delete(id);
        Console.WriteLine("Kategorin är borttagen.");
    }

    private void SetWeeklyLimit()
    {
        ListCategories();
        int id = ConsoleInput.ReadInt("Kategorins nummer", min: 1);
        int max = ConsoleInput.ReadInt("Högst antal gånger per vecka", min: 1, max: 21);

        _categories.SetWeeklyLimit(id, max);
        Console.WriteLine("Gränsen är sparad.");
    }

}
