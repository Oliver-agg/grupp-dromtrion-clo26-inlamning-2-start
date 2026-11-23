namespace Recept.Models;

/// <summary>
/// Ett recept. Minimal modell: bara det som README:n kräver.
/// </summary>
public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Servings { get; set; }
    public List<Ingredient> Ingredients { get; set; } = new();

    // Du behöver bygga ut detta mer.
    // Beställningen kräver kategori, en valfri anteckning och vilken kock som skapat receptet.
    // När ni lägger till fälten: uppdatera även UI/RecipeMenu.cs så att användaren kan fylla i dem.
}
