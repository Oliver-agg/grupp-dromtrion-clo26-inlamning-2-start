namespace Recept.Models;

/// <summary>
/// En rad i inköpslistan. Samma ingrediens med samma enhet ska vara summerad till en rad.
/// </summary>
public class ShoppingListItem
{
    public string Name { get; set; } = "";
    public decimal Amount { get; set; }
    public string Unit { get; set; } = "";
}
