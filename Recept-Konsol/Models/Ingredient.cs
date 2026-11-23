namespace Recept.Models;

/// <summary>
/// En ingrediens i ett recept, till exempel "2,5 dl vetemjöl".
/// </summary>
public class Ingredient
{
    public string Name { get; set; } = "";
    public decimal Amount { get; set; }
    public string Unit { get; set; } = "";

    // Du behöver bygga ut detta mer.
    // Väljer ni en relationsdatabas behöver ingrediensen troligen ett Id och en koppling till sitt recept.
}
