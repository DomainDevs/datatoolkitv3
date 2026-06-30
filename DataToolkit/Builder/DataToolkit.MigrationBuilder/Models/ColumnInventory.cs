using DataToolkit.MigrationBuilder.Models.Metadata;

namespace DataToolkit.MigrationBuilder.Models;

public class ColumnInventory
{
    public string Name { get; set; } = "";
    public ColumnMetadata? SourceValue { get; set; }
    public ColumnMetadata? TargetValue { get; set; }
    public string DifferenceType { get; set; } = "";
}
