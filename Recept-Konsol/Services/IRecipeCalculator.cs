using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Räknar om och formaterar mängder i ett recept.
/// </summary>
public interface IRecipeCalculator
{
    /// <summary>Returnerar ingredienserna omräknade till ett annat antal portioner (funktion 7).</summary>
    List<Ingredient> ScaleIngredients(Recipe recipe, int servings);

    /// <summary>Till exempel "1,5 dl", "2 st" eller "0,25 tsk" (önskemål 1).</summary>
    string FormatAmount(decimal amount, string unit);
}
