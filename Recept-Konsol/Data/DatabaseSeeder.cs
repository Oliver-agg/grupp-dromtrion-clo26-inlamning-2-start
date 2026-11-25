using Recept.Services;

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
}