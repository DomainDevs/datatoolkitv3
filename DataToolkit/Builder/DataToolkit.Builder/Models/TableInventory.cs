namespace DataToolkit.Builder.Models;

public class TableInventory
{
    public string Schema { get; set; } = "";
    public string Name { get; set; } = "";
    public List<ColumnInventory> Columns { get; set; } = new();
}
