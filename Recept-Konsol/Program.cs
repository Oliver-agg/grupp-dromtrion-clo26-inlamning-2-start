// Startpunkt för Recept (konsol). Här kopplas alla delar ihop, och sedan startar menyn.
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Configuration;
using Recept.Services;
using Recept.UI;

// Svenska tecken och svenskt talformat (2,5 i stället för 2.5).
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("sv-SE");
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("sv-SE");

// Connectionstring läses från user-secrets, aldrig från koden.
var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string? connectionString = config["DB_CONNECTION"];


// Byt den här raden när ni har en riktig databas, till exempel:
//   IRecipeRepository repository = new MongoRecipeRepository(connectionString);
// Er databasklass ska skapa databasen och seeda exempeldata vid första start, om den är tom.
// Här passar till exempel mönstret Factory: en klass som väljer och skapar rätt repository
// utifrån konfigurationen, så att Program.cs inte behöver känna till varje databasklass.
IRecipeRepository repository = new InMemoryRecipeRepository();

// De här tre ska ni bygga. Tills dess visar menyn "Inte implementerat ännu".
ICategoryService categories = new CategoryService();
IMealPlanService mealPlan = new MealPlanService();
IRecipeCalculator calculator = new RecipeCalculator();

var recipeMenu = new RecipeMenu(repository, calculator);
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
