using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Recept.Models;


//Builder (designmönster): bygger ett recept steg för steg
//Alla regler för ett recept och deras ingredienser kontrolleras här på ett ställe
//Felaktiga värden ger en ArgumentException med ett med. på svenska, som menyn visar istället för att programmet kraschar.

namespace Recept.Services;

public class RecipeBuilder
//De enda tillåtna enheterna, så att inköpslistan kan summera rätt
{
    public static readonly IReadOnlyList<string> AllowedUnits =
       new[] { "dl", "l", "msk", "tsk", "krm", "g", "kg", "st" };

    private readonly List<Ingredient> _ingredients = new();
    private string _name = "";
    private int _servings;
    private int _categoryId;
    private int? _chefId;
    private string _note = "";

    //Receptnamn
    public RecipeBuilder WithName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Receptet måste ha ett namn.");
        }

        _name = name.Trim();
        return this;
    }

    //Portioner
    public RecipeBuilder WithServings(int servings)
    {
        if (servings < 1)
        {
            throw new ArgumentException("Antal portioner måste vara minst 1.");
        }

        _servings = servings;
        return this;
    }

    //Recept kategory
    public RecipeBuilder InCategory(int categoryId)
    {
        if (categoryId < 1)
        {
            throw new ArgumentException("Receptet måste ha en kategori.");
        }

        _categoryId = categoryId;
        return this;
    }

    //Kock
    public RecipeBuilder ByChef(int chefId)
    {
        if (chefId < 1)
        {
            throw new ArgumentException("Receptet måste ha en kock.");
        }

        _chefId = chefId;
        return this;
    }

    //Anteckning
    public RecipeBuilder WithNote(string note)
    {
        _note = note?.Trim() ?? "";
        return this;
    }

    //Lägger till en ingrediens 
    public RecipeBuilder WithIngredient(string name, decimal amount, string unit)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ingrediensen måste ha ett namn.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Mängden måste vara större än 0.");
        }

        string normalizedUnit = (unit ?? "").Trim().ToLowerInvariant();
        if (!AllowedUnits.Contains(normalizedUnit))
        {
            throw new ArgumentException($"Enheten  \"{unit}\" finns inte. Välj bland: {string.Join(", ", AllowedUnits)}."); 
        }

        _ingredients.Add(new Ingredient { Name = name.Trim(), Amount = amount, Unit = normalizedUnit });
        return this;
    }


    //Skapar receptet. Name, portioner, kategori och måste vara ifyllda (beslut 4)
    public Recipe Build()
    {
        if (_name.Length == 0)
        {
            throw new ArgumentException("Receptet måste ha ett namn.");
        }

        if (_servings < 1)
        {
            throw new ArgumentException("Antal portioner måste vara minst 1.");
        }

        if (_categoryId < 1)
        {
            throw new ArgumentException("Receptet måste ha en kategori");
        }

        if (_chefId is null)
        {
            throw new ArgumentException("Recepetet måste ha en kock.");
        }

        return new Recipe
        {
            Name = _name,
            Servings = _servings,
            CategoryId = _categoryId,
            ChefId = _chefId,
            Note = _note,
            Ingredients = new List<Ingredient>(_ingredients),
        };
    }
}


