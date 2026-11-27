using System.Globalization;
using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Veckoplan och inköpslista. Sparar via IMealPlanRepository och hämtar recept via IRecipeRepository.
/// En vecka börjar alltid på måndag (önskemål 2).
/// </summary>
public class MealPlanService : IMealPlanService
{
    private readonly IMealPlanRepository _mealPlans;
    private readonly IRecipeRepository _recipes;
    private readonly CategoryLimitChecker _limitChecker;

    public MealPlanService(IMealPlanRepository mealPlans, IRecipeRepository recipes, CategoryLimitChecker limitChecker)
    {
        _mealPlans = mealPlans;
        _recipes = recipes;
        _limitChecker = limitChecker;
    }

    // Sparar måltiden. Varningar vid 80 % och 100 % av kategorins gräns
    public List<string> PlanMeal(int recipeId, DateOnly date)
    {
        if (_recipes.GetRecipe(recipeId) is null)
        {
            throw new ArgumentException($"Det finns inget recept med nummer {recipeId}.");
        }

        _mealPlans.AddPlannedMeal(new PlannedMeal { Date = date, RecipeId = recipeId });

        return _limitChecker.CheckWarnings(date);
    }

    public List<PlannedMeal> GetWeekPlan(int year, int weekNumber)
    {
        (DateOnly monday, DateOnly sunday) = GetWeekRange(year, weekNumber);
        return _mealPlans.GetPlannedMeals(monday, sunday);
    }

    // Räknar fram inköpslistan ur veckoplanen. Samma ingrediens med samma enhet
    // summeras till en rad. Listan sparas inte, så den visar alltid aktuell data (beslut 9).
    public List<ShoppingListItem> GetShoppingList(int year, int weekNumber)
    {
        var items = new List<ShoppingListItem>();

        foreach (PlannedMeal meal in GetWeekPlan(year, weekNumber))
        {
            Recipe? recipe = _recipes.GetRecipe(meal.RecipeId);
            if (recipe is null)
            {
                // Receptet har tagits bort efter att måltiden planerades.
                continue;
            }

            foreach (Ingredient ingredient in recipe.Ingredients)
            {
                AddToList(items, ingredient);
            }
        }

        return items.OrderBy(i => i.Name).ToList();
    }

    // Lägger till ingrediensen, eller ökar mängden om samma namn och enhet redan finns.
    // Stora och små bokstäver räknas som samma, så "Mjölk" och "mjölk" blir en rad.
    private static void AddToList(List<ShoppingListItem> items, Ingredient ingredient)
    {
        ShoppingListItem? existing = items.FirstOrDefault(i =>
            string.Equals(i.Name, ingredient.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(i.Unit, ingredient.Unit, StringComparison.OrdinalIgnoreCase));

        if (existing is null)
        {
            items.Add(new ShoppingListItem
            {
                Name = ingredient.Name,
                Amount = ingredient.Amount,
                Unit = ingredient.Unit,
            });
        }
        else
        {
            existing.Amount += ingredient.Amount;
        }
    }

    // Räknar fram måndag och söndag för en vecka. ISOWeek räknar veckor som börjar på måndag.
    private static (DateOnly Monday, DateOnly Sunday) GetWeekRange(int year, int weekNumber)
    {
        if (weekNumber < 1 || weekNumber > ISOWeek.GetWeeksInYear(year))
        {
            throw new ArgumentException($"År {year} har ingen vecka {weekNumber}.");
        }

        DateOnly monday = DateOnly.FromDateTime(ISOWeek.ToDateTime(year, weekNumber, DayOfWeek.Monday));
        return (monday, monday.AddDays(6));
    }
}