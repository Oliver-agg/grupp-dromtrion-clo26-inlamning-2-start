namespace Recept.Models;

/// <summary>
/// En receptkategori, till exempel "Middag" eller "Efterrätt".
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public int? MaxPerWeek { get; set; }

    // Du behöver bygga ut detta mer.
    // Beställningen kräver en gräns för hur många gånger per vecka kategorin får förekomma,
    // och att "Övrigt" aldrig kan tas bort.
}
