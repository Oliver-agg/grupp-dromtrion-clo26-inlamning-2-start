using System.Globalization;
using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Räknar om mängder till ett antal portioner och skriver dem snyggt (önskemål 1).
/// </summary>
public class RecipeCalculator : IRecipeCalculator
{
    private const int MaxDecimals = 2;
    private static readonly CultureInfo Swedish = CultureInfo.GetCultureInfo("sv-SE");

    //2,5 dl för 4 portioner blir 1,25 fl för 2 portioner. Originalreceptet ändras inte,
    //och mängderna avrundas inte här utan först när de visas (FormatAmount).
    public List<Ingredient> ScaleIngredients(Recipe recipe, int servings)
    {
        if (servings < 1)
        {
            throw new ArgumentException("Antal portioner måste vara minst 1.");
        }

        if (recipe.Servings < 1)
        {
            throw new ArgumentException(
                $"Receptet \"{recipe.Name}\" har ogiltigt antal portioner och kan inte räknas om.");
        }

        return recipe.Ingredients
            .Select(i => new Ingredient
            {
                Name = i.Name,
                Amount = i.Amount * servings / recipe.Servings,
                Unit = i.Unit,
            })
            .ToList();
    }

    //Högst två decimaler, utan onödiga nollar och med decimalkomma:
    public string FormatAmount(decimal amount, string unit)
    {
        decimal rounded = Math.Round(amount, MaxDecimals, MidpointRounding.AwayFromZero);
        return $"{rounded.ToString("0.##", Swedish)} {unit}".TrimEnd();
    }
}
