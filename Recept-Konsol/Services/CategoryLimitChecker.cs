using Recept.Models;
using System.Globalization;

namespace Recept.Services;


/// <summary>
/// Kontrollerar veckogränser och hanterar varningar vid 80% och 100% 
/// Sparar visade varningar så att de inte visas igen samma vecka.
/// </summary>
public class CategoryLimitChecker
{
    private readonly ICategoryRepository _categories;
    private readonly IRecipeRepository _recipes;
    private readonly IMealPlanRepository _mealPlans;

    public CategoryLimitChecker(
        ICategoryRepository categories,
        IRecipeRepository recipes,
        IMealPlanRepository mealPlans)
    {
        _categories = categories;
        _recipes = recipes;
        _mealPlans = mealPlans;
    }

    public List<string> CheckWarnings(DateOnly date)
    {
        var messages = new List<string>();

        // Hitta rätt ISO vecka (måndag till söndag).
        DateTime dateTime = date.ToDateTime(TimeOnly.MinValue);
        int year = ISOWeek.GetYear(dateTime);
        int week = ISOWeek.GetWeekOfYear(dateTime);

        DateOnly monday = DateOnly.FromDateTime(ISOWeek.ToDateTime(year, week, DayOfWeek.Monday));

        DateOnly sunday = monday.AddDays(6);

        // Hämta veckans planerade måltider.
        List<PlannedMeal> plannedMeals = _mealPlans.GetPlannedMeals(monday, sunday);

        // Kontrollera alla kategorier som har en veckogräns.
        foreach (Category category in _categories.GetAll())
        {
            if(category.MaxPerWeek is not int limit)
            {
                continue;
            }

            // Räkna hur många planerade recept som tillhör kategorin.
            int count = plannedMeals.Count(meal => _recipes.GetRecipe(meal.RecipeId)?.CategoryId == category.Id);

            // 100%: gränsen är nådd.
            if(count >= limit)
            {
                if(!_mealPlans.HasWarning(category.Id, year, week, 100))
                {
                    _mealPlans.AddWarning(new CategoryWarning
                    {
                        CategoryId = category.Id,
                        Year = year,
                        Week = week,
                        Level = 100
                    });
                    messages.Add($"{category.Name} har nått 100% av gränsen.");
                }
            }

            // 80%: Gränsen är nästan nådd.
            else
            {
                int warningLimit = (int)Math.Ceiling(limit * 0.8m);

                if (count >= warningLimit && !_mealPlans.HasWarning(category.Id, year, week, 80))
                {
                    _mealPlans.AddWarning(new CategoryWarning
                    {
                        CategoryId = category.Id,
                        Year = year,
                        Week = week,
                        Level = 80
                    });
                    messages.Add($"{category.Name} är på 80% av gränsen.");
                }
            }
        }
        return messages;
    }
}