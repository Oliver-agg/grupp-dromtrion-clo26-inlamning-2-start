namespace Recept.Models;

/// <summary>
/// En varning som redan har visats för en kategori en viss vecka.
/// Sparas så att samma varning inte visas igen förrän veckan byts (önskemål 4),
/// inte ens efter att programmet startats om.
/// </summary>
public class CategoryWarning
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int Year { get; set; }
    public int Week { get; set; }

    // 80 eller 100 (procent av kategorins gräns).
    public int Level { get; set; }
}
