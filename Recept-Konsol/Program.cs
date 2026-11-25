// Startpunkt för Recept (konsol). Här kopplas alla delar ihop, och sedan startar menyn.
using System.Globalization;
using System.Text;
using Recept.Data;
using Recept.Services;
using Recept.UI;

// Svenska tecken och svenskt talformat (2,5 i stället för 2.5).
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("sv-SE");
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("sv-SE");

// Factory (designmönster) skapar allt som pratar med databasen. Connectionstringen läses
// från user-secrets inne i Factory. Saknas den, eller svarar inte databasen,
// visas ett meddelande på svenska i stället för att programmet kraschar.
IRecipeRepository repository;
ICategoryService categories;
try
{
    repository = RepositoryFactory.Create();
    categories = RepositoryFactory.CreateCategoryService();
}
catch (InvalidOperationException ex)
{
    ConsoleInput.WriteWarning(ex.Message);
    return;
}

// Exempeldata vid första start, om databasen är tom.
DatabaseSeeder.SeedIfEmpty(repository, new InMemoryRecipeRepository());

// De här två ska ni bygga. Tills dess visar menyn "Inte implementerat ännu".
// MealPlanService får sina repositories via konstruktorn när den byggs,
// till exempel med RepositoryFactory.CreateMealPlanRepository().
IMealPlanService mealPlan = new MealPlanService();
IRecipeCalculator calculator = new RecipeCalculator();

var recipeMenu = new RecipeMenu(repository, categories, calculator);
var categoryMenu = new CategoryMenu(categories);
var mealPlanMenu = new MealPlanMenu(mealPlan, repository, calculator);

Console.WriteLine("Välkommen till Recept!");

new Menu("Recept", exitLabel: "Avsluta")
    .Add("1", "Lista recept", recipeMenu.ListRecipes)
    .Add("2", "Lägg till recept", recipeMenu.AddRecipe)
    .Add("3", "Visa ett recept", recipeMenu.ShowRecipe)
    .Add("4", "Ta bort recept", recipeMenu.DeleteRecipe)
    .Add("5", "Sök recept på ingrediens", recipeMenu.SearchByIngredient)
    .Add("6", "Lägg till kock", recipeMenu.AddChef)
    .Add("7", "Lista kockar", recipeMenu.ListChefs)
    .Add("8", "Visa recept för annat antal portioner", recipeMenu.ShowRecipeForServings)
    .Add("9", "Kategorier och gränser", categoryMenu.Run)
    .Add("10", "Veckoplan och inköpslista", mealPlanMenu.Run)
    .Run();

Console.WriteLine("Hej då!");
