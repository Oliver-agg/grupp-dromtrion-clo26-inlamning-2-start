using System.Globalization;

namespace Recept.UI;

/// <summary>
/// Läser och kontrollerar inmatning. Varje metod frågar igen tills svaret går att tolka,
/// så felaktig inmatning ger ett meddelande i stället för en krasch.
/// </summary>
public static class ConsoleInput
{
    private static readonly CultureInfo Swedish = CultureInfo.GetCultureInfo("sv-SE");

    public static string ReadText(string prompt)
    {
        Console.Write($"{prompt}: ");
        return (Console.ReadLine() ?? "").Trim();
    }

    public static string ReadRequiredText(string prompt)
    {
        while (true)
        {
            string text = ReadText(prompt);
            if (text.Length > 0)
            {
                return text;
            }
            WriteWarning("Fältet får inte vara tomt.");
        }
    }

    public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            string text = ReadText(prompt);
            if (int.TryParse(text, out int value) && value >= min && value <= max)
            {
                return value;
            }
            WriteWarning($"Skriv ett heltal mellan {min} och {max}.");
        }
    }

    public static decimal ReadDecimal(string prompt, decimal min = decimal.MinValue)
    {
        while (true)
        {
            // Både "2,5" och "2.5" godkänns.
            string text = ReadText(prompt).Replace('.', ',');
            if (decimal.TryParse(text, NumberStyles.Number, Swedish, out decimal value) && value >= min)
            {
                return value;
            }
            WriteWarning("Skriv ett tal, till exempel 2,5.");
        }
    }

    public static DateOnly ReadDate(string prompt)
    {
        while (true)
        {
            string text = ReadText($"{prompt} (ÅÅÅÅ-MM-DD)");
            if (DateOnly.TryParseExact(text, "yyyy-MM-dd", Swedish, DateTimeStyles.None, out DateOnly date))
            {
                return date;
            }
            WriteWarning("Okänt datum. Skriv till exempel 2026-11-15.");
        }
    }

    /// <summary>Frågar "(j/n)" och returnerar true bara för "j".</summary>
    public static bool Confirm(string question)
    {
        while (true)
        {
            string answer = ReadText($"{question} (j/n)").ToLowerInvariant();
            if (answer == "j") return true;
            if (answer == "n") return false;
            WriteWarning("Svara j eller n.");
        }
    }

    public static void WriteWarning(string message)
    {
        ConsoleColor previous = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"! {message}");
        Console.ForegroundColor = previous;
    }
}
