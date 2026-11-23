namespace Recept.UI;

/// <summary>
/// En återanvändbar konsolmeny. Varje val är en text och en metod som körs.
/// Menyn fångar fel från metoderna, så att programmet aldrig kraschar på ett menyval.
/// </summary>
public class Menu
{
    private readonly string _title;
    private readonly string _exitLabel;
    private readonly List<(string Key, string Label, Action Action)> _items = new();

    public Menu(string title, string exitLabel = "Tillbaka")
    {
        _title = title;
        _exitLabel = exitLabel;
    }

    public Menu Add(string key, string label, Action action)
    {
        _items.Add((key, label, action));
        return this;
    }

    /// <summary>Visar menyn tills användaren väljer 0.</summary>
    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"=== {_title} ===");
            foreach (var item in _items)
            {
                Console.WriteLine($"{item.Key}. {item.Label}");
            }
            Console.WriteLine($"0. {_exitLabel}");

            string choice = ConsoleInput.ReadText("Val");
            if (choice == "0")
            {
                return;
            }

            var selected = _items.FirstOrDefault(i => i.Key == choice);
            if (selected.Action is null)
            {
                Console.WriteLine("Okänt val, försök igen.");
                continue;
            }

            RunSafely(selected.Action);
        }
    }

    private static void RunSafely(Action action)
    {
        try
        {
            action();
        }
        catch (NotImplementedException)
        {
            ConsoleInput.WriteWarning("Inte implementerat ännu. Den här funktionen ska ni bygga.");
        }
        catch (ArgumentException ex)
        {
            // Felaktig inmatning som upptäcks i era services: kasta ArgumentException med ett
            // meddelande på svenska, så visas det här i stället för att programmet kraschar.
            ConsoleInput.WriteWarning(ex.Message);
        }
        catch (Exception ex)
        {
            ConsoleInput.WriteWarning($"Något gick fel: {ex.Message}");
        }
    }
}
