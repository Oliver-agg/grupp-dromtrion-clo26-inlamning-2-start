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
        var recipe = new Recipe
        {
            Name = ConsoleInput.ReadRequiredText("Namn"),
            Servings = ConsoleInput.ReadInt("Antal portioner", min: 1, max: 100),
        };


        Console.WriteLine();
        Console.WriteLine("Välj kategori: ");

        foreach (Category category in _categories.GetAll())
        {
            Console.WriteLine($"{category.Id}. {category.Name}");
        }

        recipe.CategoryId = ConsoleInput.ReadInt(
            "Kategorins nummer",
            min: 1);

        recipe.Note = ConsoleInput.ReadText(
            "Anteckning (valfri)");

        Console.WriteLine("Ingredienser. Lämna namnet tomt när du är klar.");
        while (true)
        {
            string name = ConsoleInput.ReadText("  Ingrediens");
            if (name.Length == 0)
            {
                break;
            }

            recipe.Ingredients.Add(new Ingredient
            {
                Name = name,
                Amount = ConsoleInput.ReadDecimal("  Mängd", min: 0),
                Unit = ConsoleInput.ReadRequiredText("  Enhet (dl, st, g ...)"),
            });
        }

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
        foreach (Chef chef in _repository.GetAllChefs())
        {
            Console.WriteLine($"{chef.Id}. {chef.FirstName} {chef.LastName}");
        }
    }

    public void AddChef()
    {
        // Fälten följer modellen Chef. Tar ni bort ett fält ur modellen, ta bort raden här också.
        var chef = new Chef
        {
            FirstName = ConsoleInput.ReadRequiredText("Förnamn"),
            LastName = ConsoleInput.ReadText("Efternamn"),
            BirthDate = ConsoleInput.ReadDate("Födelsedatum"),
            HairColor = ConsoleInput.ReadText("Hårfärg"),
            HeightCm = ConsoleInput.ReadInt("Längd i cm", min: 0, max: 300),
            Religion = ConsoleInput.ReadText("Religion"),
        };

        _repository.AddChef(chef);
        Console.WriteLine($"Kocken {chef.FirstName} är tillagd.");
    }

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

        if (!string.IsNullOrWhiteSpace(recipe.Note))
        {
            Console.WriteLine($"Anteckning: {recipe.Note}");
        }

    }
}
