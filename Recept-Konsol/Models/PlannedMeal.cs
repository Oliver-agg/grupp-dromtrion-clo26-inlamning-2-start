namespace Recept.Models;

/// <summary>
/// Ett recept som är inplanerat en viss dag i veckoplanen.
/// </summary>
public class PlannedMeal
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int RecipeId { get; set; }

    // Du behöver bygga ut detta mer.
    // Fundera på om ni vill spara receptets namn här, eller slå upp det via RecipeId.
}
