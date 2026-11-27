using Recept.Models;
using Recept.Services;

namespace Recept.UI;

/// <summary>
/// Menyval för recept och kockar. Klassen sköter bara inmatning och utskrift;
/// all lagring går via IRecipeRepository och all räkning via IRecipeCalculator.
/// </summary>
public class RecipeMenu
{
    private readonly IRecipeRepository _repository;
    private readonly ICategoryService _categories;
    private readonly IRecipeCalculator _calculator;

    public RecipeMenu(IRecipeRepository repository, ICategoryService categories, IRecipeCalculator calculator)
    {
        _repository = repository;
        _categories = categories;
        _calculator = calculator;
    }

    public void ListRecipes()
    {
        List<Recipe> recipes = _repository.GetAllRecipes();
        if (recipes.Count == 0)
        {
            Console.WriteLine("Det finns inga recept än.");
            return;
        }

        foreach (Recipe recipe in recipes)
        {
            Console.WriteLine($"{recipe.Id}. {recipe.Name} ({recipe.Servings} portioner)");
        }
    }

    public void AddRecipe()
    {

        //Beslut 4: ett recept måste ha en kock, så det finns inget att välja utan kockar.

        List<Chef> chefs = _repository.GetAllChefs();
        if (chefs.Count ==0)
        {
            ConsoleInput.WriteWarning("Det finns inga kockar. Lägg till en kock (menyval 6) först.");
            return;
        }

        RecipeBuilder builder = new RecipeBuilder()
            .WithName(ConsoleInput.ReadRequiredText("Namn"))
            .WithServings(ConsoleInput.ReadInt("Antal portioner", min: 1, max: 100))
            .InCategory(AskForCategoryId())
            .ByChef(AskForChefId(chefs))
            .WithNote(ConsoleInput.ReadText("Anteckning (valfri)"));

        ReadIngredients(builder);

        Recipe recipe = builder.Build();
        _repository.AddRecipe(recipe);
        Console.WriteLine($"Receptet \"{recipe.Name}\" är sparat.");
    }

    public void ShowRecipe()
    {
        Recipe? recipe = AskForRecipe();
        if (recipe is null)
        {
            return;
        }

        PrintRecipe(recipe, recipe.Ingredients, recipe.Servings);
    }

    public void ShowRecipeForServings()
    {
        Recipe? recipe = AskForRecipe();
        if (recipe is null)
        {
            return;
        }

        int servings = ConsoleInput.ReadInt("Antal portioner", min: 1, max: 100);
        List<Ingredient> scaled = _calculator.ScaleIngredients(recipe, servings);
        PrintRecipe(recipe, scaled, servings);
    }

    public void DeleteRecipe()
    {
        Recipe? recipe = AskForRecipe();
        if (recipe is null)
        {
            return;
        }

        // Önskemål 5: bekräfta innan något tas bort.
        if (!ConsoleInput.Confirm($"Är du säker på att du vill ta bort \"{recipe.Name}\"?"))
        {
            Console.WriteLine("Inget togs bort.");
            return;
        }

        _repository.DeleteRecipe(recipe.Id);
        Console.WriteLine($"\"{recipe.Name}\" är borttaget.");
    }

    public void SearchByIngredient()
    {
        string ingredient = ConsoleInput.ReadRequiredText("Sök på ingrediens");
        List<Recipe> hits = _repository.SearchByIngredient(ingredient);

        if (hits.Count == 0)
        {
            Console.WriteLine("Inga recept hittades.");
            return;
        }

        foreach (Recipe recipe in hits)
        {
            Console.WriteLine($"{recipe.Id}. {recipe.Name}");
        }
    }

    public void ListChefs()
    {
        List<Chef> chefs = _repository.GetAllChefs();
        if (chefs.Count == 0)
        {
            Console.WriteLine("Det finns inga kockar än!");
            return;
        }

        foreach (Chef chef in chefs)
        {
            Console.WriteLine($"{chef.Id} - {chef.FirstName}");
        }
    }

    public void AddChef()
    {
        var chef = new Chef
        {
            FirstName = ConsoleInput.ReadRequiredText("Förnamn")
        };

        _repository.AddChef(chef);
        Console.WriteLine($"Kocken {chef.FirstName} är tillagd.");
    }


    private int AskForCategoryId()
    {
        List<Category> categories = _categories.GetAll();

        Console.WriteLine();
        Console.WriteLine("Välj kategori: ");

        foreach (Category category in categories)
        {
            Console.WriteLine($"{category.Id} - {category.Name}");
        }

        while (true)
        {
            int id = ConsoleInput.ReadInt("Kategorins nummer, min: 1");
            if (categories.Any(c => c.Id == id))
            {
                return id;
            }

            ConsoleInput.WriteWarning($"Det finns ingen kategori med {id}");
        }
    }

    private static int AskForChefId(List<Chef> chefs)

    {
        Console.WriteLine();
        Console.WriteLine("Välj kock: ");
        
        foreach (Chef chef in chefs)
        {
            Console.WriteLine($"{chef.Id} - {chef.FirstName}");
        }

        while (true)
        {
            int id = ConsoleInput.ReadInt("Kockens nummer", min: 1);
            if (chefs.Any(c => c.Id == id))
            {
                return id;
            }

            ConsoleInput.WriteWarning($"Det finns ingen kock med nummer {id}");
        }
    }


    //Mängd och enhet kontrolleras av RecipeBuilder. Är något fel visas meddelandet 
    //och användaren får skriva in ingrediensen igen.
    private static void ReadIngredients(RecipeBuilder builder)
    {
        string units = string.Join(", ", RecipeBuilder.AllowedUnits);

        Console.WriteLine("Ingredienser. Lämna namnet tomt när du är klar.");
        while (true)
        {
            string name = ConsoleInput.ReadText(" Ingrediens");
            if (name.Length == 0)
            {
                return;
            }

            decimal amount = ConsoleInput.ReadDecimal(" Mängd");
            string unit = ConsoleInput.ReadRequiredText($" Enhet ({units})");

            try
            {
                builder.WithIngredient(name, amount, unit);
            }
            catch (ArgumentException ex)
            {
                ConsoleInput.WriteWarning(ex.Message);
            }
        }
    }

    private string GetChefName(int? chefId) =>
        _repository.GetAllChefs().FirstOrDefault(c => c.Id == chefId)?.FirstName ?? "(okänd kock)";


    private Recipe? AskForRecipe()
    {
        ListRecipes();
        int id = ConsoleInput.ReadInt("Receptets nummer", min: 1);
        Recipe? recipe = _repository.GetRecipe(id);

        if (recipe is null)
        {
            ConsoleInput.WriteWarning($"Det finns inget recept med nummer {id}.");
        }
        return recipe;
    }

    private void PrintRecipe(Recipe recipe, List<Ingredient> ingredients, int servings)
    {
        Console.WriteLine();
        Console.WriteLine($"{recipe.Name}, {servings} portioner");
        foreach (Ingredient ingredient in ingredients)
        {
            Console.WriteLine($"  {_calculator.FormatAmount(ingredient.Amount, ingredient.Unit)} {ingredient.Name}");
        }

        Console.WriteLine();
        string categoryName = _categories.GetAll()
            .FirstOrDefault(c => c.Id == recipe.CategoryId)?.Name ?? "(ingen kategori)";
        Console.WriteLine($"Kategori: {categoryName}");
        Console.WriteLine($"Kock: {GetChefName(recipe.ChefId)}");

        if (!string.IsNullOrWhiteSpace(recipe.Note))
        {
            Console.WriteLine($"Anteckning: {recipe.Note}");
        }

    }
}
