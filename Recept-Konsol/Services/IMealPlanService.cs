using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Veckoplan och inköpslista. En vecka börjar alltid på måndag (önskemål 2).
/// </summary>
public interface IMealPlanService
{
    /// <summary>
    /// Lägger in ett recept på ett datum. Returnerar de varningar användaren ska se,
    /// till exempel "Efterrätt är på 80 % av gränsen". Tom lista betyder inga varningar.
    /// </summary>
    List<string> PlanMeal(int recipeId, DateOnly date);

    List<PlannedMeal> GetWeekPlan(int year, int weekNumber);

    /// <summary>Samma ingrediens med samma enhet summeras till en rad.</summary>
    List<ShoppingListItem> GetShoppingList(int year, int weekNumber);
}
