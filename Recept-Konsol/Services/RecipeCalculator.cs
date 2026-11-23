using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Ska byggas av er.
/// </summary>
public class RecipeCalculator : IRecipeCalculator
{
    public List<Ingredient> ScaleIngredients(Recipe recipe, int servings) => throw new NotImplementedException();

    // Den här fungerar redan, men fel: den skriver "2,50 dl" i stället för "2,5 dl".
    // Du behöver bygga ut detta mer, så att den följer önskemål 1.
    public string FormatAmount(decimal amount, string unit) => $"{amount:F2} {unit}";
}
