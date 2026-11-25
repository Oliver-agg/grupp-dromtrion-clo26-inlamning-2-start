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

    public MealPlanService(IMealPlanRepository mealPlans, IRecipeRepository recipes)
    {
        _mealPlans = mealPlans;
        _recipes = recipes;
    }

    // Sparar måltiden. Varningar vid 80 % och 100 % av kategorins gräns
    // byggs av Daniella och läggs till här (beslut 9).
    public List<string> PlanMeal(int recipeId, DateOnly date)
    {
        if (_recipes.GetRecipe(recipeId) is null)
        {
            throw new ArgumentException($"Det finns inget recept med nummer {recipeId}.");
        }

        _mealPlans.AddPlannedMeal(new PlannedMeal { Date = date, RecipeId = recipeId });

        return new List<string>();
    }

    public List<PlannedMeal> GetWeekPlan(int year, int weekNumber)
    {
        (DateOnly monday, DateOnly sunday) = GetWeekRange(year, weekNumber);
        return _mealPlans.GetPlannedMeals(monday, sunday);
    }

    // DEMO: byts ut i nästa steg mot en lista som räknas fram ur veckoplanen.
    public List<ShoppingListItem> GetShoppingList(int year, int weekNumber) =>
    [
        new ShoppingListItem { Name = "vetemjöl", Amount = 5m, Unit = "dl" },
        new ShoppingListItem { Name = "mjölk", Amount = 12m, Unit = "dl" },
        new ShoppingListItem { Name = "ägg", Amount = 6m, Unit = "st" },
        new ShoppingListItem { Name = "salt", Amount = 0.5m, Unit = "tsk" },
        new ShoppingListItem { Name = "krossade tomater", Amount = 400m, Unit = "g" },
        new ShoppingListItem { Name = "gul lök", Amount = 1m, Unit = "st" },
        new ShoppingListItem { Name = "grönsaksbuljong", Amount = 3m, Unit = "dl" },
    ];

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