using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Ska byggas av er. Båda metoderna fungerar redan, men fel, så att ni ser vad som ska rättas.
/// </summary>
public class RecipeCalculator : IRecipeCalculator
{
    // DEMO: räknar inte om, utan returnerar originalmängderna.
    // Du behöver bygga ut detta mer: 2,5 dl för 4 portioner ska bli 1,25 dl för 2 portioner.
    public List<Ingredient> ScaleIngredients(Recipe recipe, int servings) => recipe.Ingredients;

    // Den här skriver "2,50 dl" i stället för "2,5 dl".
    // Du behöver bygga ut detta mer, så att den följer önskemål 1.
    public string FormatAmount(decimal amount, string unit) => $"{amount:F2} {unit}";
}
