using MongoDB.Driver;
using Recept.Models;
using Recept.Services;

namespace Recept.Data;

/// <summary>
/// Sparar veckoplanen och vilka varningar som redan har visats.
/// Bara lagring: reglerna ligger i MealPlanService och i varningslogiken.
/// </summary>
public class MongoMealPlanRepository : IMealPlanRepository
{
    private readonly MongoContext _context;
    private readonly IMongoCollection<PlannedMeal> _meals;
    private readonly IMongoCollection<CategoryWarning> _warnings;

    public MongoMealPlanRepository(MongoContext context)
    {
        _context = context;
        _meals = context.GetCollection<PlannedMeal>("plannedMeals");
        _warnings = context.GetCollection<CategoryWarning>("categoryWarnings");
    }

    // Hämtar alla planerade måltider och filtrerar fram dem mellan två datum (båda inräknade).
    // Ett hushåll har få måltider, så det går snabbt och fungerar oavsett hur datumet sparas.
    public List<PlannedMeal> GetPlannedMeals(DateOnly from, DateOnly to) =>
        _meals.Find(FilterDefinition<PlannedMeal>.Empty)
            .ToList()
            .Where(m => m.Date >= from && m.Date <= to)
            .OrderBy(m => m.Date)
            .ThenBy(m => m.Id)
            .ToList();

    public void AddPlannedMeal(PlannedMeal meal)
    {
        meal.Id = _context.GetNextId("plannedMeals");
        _meals.InsertOne(meal);
    }

    public void DeletePlannedMeal(int id) =>
        _meals.DeleteOne(m => m.Id == id);

    public bool HasWarning(int categoryId, int year, int week, int level) =>
        _warnings.Find(w =>
            w.CategoryId == categoryId &&
            w.Year == year &&
            w.Week == week &&
            w.Level == level).Any();

    public void AddWarning(CategoryWarning warning)
    {
        warning.Id = _context.GetNextId("categoryWarnings");
        _warnings.InsertOne(warning);
    }
}
