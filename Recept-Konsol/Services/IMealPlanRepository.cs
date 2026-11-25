using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Lagring av veckoplanen och av varningar som redan visats.
/// Regler ligger i MealPlanService, inte här.
/// </summary>
public interface IMealPlanRepository
{
    /// <summary>Alla planerade måltider mellan två datum, båda inräknade.</summary>
    List<PlannedMeal> GetPlannedMeals(DateOnly from, DateOnly to);
    void AddPlannedMeal(PlannedMeal meal);
    void DeletePlannedMeal(int id);

    /// <summary>Har varningen (80 eller 100) redan visats för kategorin den här veckan?</summary>
    bool HasWarning(int categoryId, int year, int week, int level);
    void AddWarning(CategoryWarning warning);
}
