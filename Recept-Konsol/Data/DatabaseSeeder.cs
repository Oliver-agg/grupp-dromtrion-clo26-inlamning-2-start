using Recept.Services;
using Recept.Models;

namespace Recept.Data;

public static class DatabaseSeeder
{
    public static void SeedIfEmpty(IRecipeRepository target, IRecipeRepository source)
    {
        if (target.GetAllChefs().Count == 0)
        {
            foreach (var chef in source.GetAllChefs())
            {
                target.AddChef(chef);
            }
        }

        if (target.GetAllRecipes().Count == 0)
        {
            foreach (var recipe in source.GetAllRecipes())
            {
                target.AddRecipe(recipe);
            }
        }
    }

    public static void SeedCategoriesIfEmpty(ICategoryRepository repository)
    {
        if (repository.GetAll().Count > 0)
        {
            return;
        }

        foreach(string name in new[] { "Frukost", "Lunch", "Middag", "Efterrätt", "Bakning", "Övrigt"})
        {
            repository.Add(new Category { Name = name });
        }
    }
}