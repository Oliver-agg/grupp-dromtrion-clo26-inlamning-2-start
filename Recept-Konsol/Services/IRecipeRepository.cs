using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Kontraktet mellan gränssnittet och datalagret. Menyn känner bara till det här interfacet,
/// så ni kan byta InMemoryRecipeRepository mot en riktig databas utan att röra menyn.
/// </summary>
public interface IRecipeRepository
{
    List<Recipe> GetAllRecipes();
    Recipe? GetRecipe(int id);
    void AddRecipe(Recipe recipe);
    void DeleteRecipe(int id);
    List<Recipe> SearchByIngredient(string ingredientName);

    /// <summary>Flyttar alla recept i en kategori till en annan, till exempel till "Övrigt" (önskemål 3).</summary>
    void MoveRecipesToCategory(int fromCategoryId, int toCategoryId);

    List<Chef> GetAllChefs();
    void AddChef(Chef chef);

    // Du behöver bygga ut detta mer: metoder för kategorier och veckoplan när ni sparar dem i databasen.
}
