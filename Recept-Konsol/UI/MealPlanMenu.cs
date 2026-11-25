using Recept.Models;
using Recept.Services;

namespace Recept.UI;

/// <summary>
/// Menyval för veckoplan och inköpslista. Logiken ligger i IMealPlanService.
/// </summary>
public class MealPlanMenu
{
    private readonly IMealPlanService _mealPlan;
    private readonly IRecipeRepository _repository;
    private readonly IRecipeCalculator _calculator;

    public MealPlanMenu(IMealPlanService mealPlan, IRecipeRepository repository, IRecipeCalculator calculator)
    {
        _mealPlan = mealPlan;
        _repository = repository;
        _calculator = calculator;
    }

    public void Run()
    {
        new Menu("Veckoplan")
            .Add("1", "Planera en måltid", PlanMeal)
            .Add("2", "Visa veckoplan", ShowWeekPlan)
            .Add("3", "Visa inköpslista", ShowShoppingList)
            .Run();
    }

    private void PlanMeal()
    {
        foreach (Recipe recipe in _repository.GetAllRecipes())
        {
            Console.WriteLine($"{recipe.Id}. {recipe.Name}");
        }

        int recipeId = ConsoleInput.ReadInt("Receptets nummer", min: 1);

        // Kontrollera receptet direkt, så att användaren slipper skriva ett datum i onödan.
        if (_repository.GetRecipe(recipeId) is null)
        {
            ConsoleInput.WriteWarning($"Det finns inget recept med nummer {recipeId}.");
            return;
        }

        DateOnly date = ConsoleInput.ReadDate("Datum");

        List<string> warnings = _mealPlan.PlanMeal(recipeId, date);
        Console.WriteLine("Måltiden är inplanerad.");

        foreach (string warning in warnings)
        {
            ConsoleInput.WriteWarning(warning);
        }
    }

    private void ShowWeekPlan()
    {
        (int year, int week) = AskForWeek();
        List<PlannedMeal> meals = _mealPlan.GetWeekPlan(year, week);

        Console.WriteLine($"Vecka {week}, {year}");
        foreach (PlannedMeal meal in meals.OrderBy(m => m.Date))
        {
            string recipeName = _repository.GetRecipe(meal.RecipeId)?.Name ?? "(okänt recept)";
            Console.WriteLine($"  {meal.Date:dddd d MMMM}: {recipeName}");
        }
    }

    private void ShowShoppingList()
    {
        (int year, int week) = AskForWeek();
        List<ShoppingListItem> items = _mealPlan.GetShoppingList(year, week);

        Console.WriteLine($"Inköpslista för vecka {week}, {year}");
        foreach (ShoppingListItem item in items)
        {
            Console.WriteLine($"  {_calculator.FormatAmount(item.Amount, item.Unit)} {item.Name}");
        }
    }

    private static (int Year, int Week) AskForWeek()
    {
        int year = ConsoleInput.ReadInt("År", min: 2000, max: 2100);
        int week = ConsoleInput.ReadInt("Veckonummer", min: 1, max: 53);
        return (year, week);
    }
}
