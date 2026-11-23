using Recept.Models;

namespace Recept.Services;

/// <summary>
/// Ska byggas av er. Menyn anropar redan de här metoderna och visar
/// "Inte implementerat ännu" så länge de kastar NotImplementedException.
/// </summary>
public class MealPlanService : IMealPlanService
{
    // Du behöver bygga ut detta mer: varningar vid 80 % och 100 % av kategorins gräns,
    // och samma 80 %-varning får inte visas igen förrän veckan byts (önskemål 4).
    // Här passar till exempel mönstret Observer.
    public List<string> PlanMeal(int recipeId, DateOnly date) => throw new NotImplementedException();

    // Tips: System.Globalization.ISOWeek räknar veckonummer med måndag som första dag.
    public List<PlannedMeal> GetWeekPlan(int year, int weekNumber) => throw new NotImplementedException();

    public List<ShoppingListItem> GetShoppingList(int year, int weekNumber) => throw new NotImplementedException();
}
