namespace DataToolkit.Builder.Models;

public sealed class TableDependency
{
    public string Table { get; set; } = "";

    public string Column { get; set; } = "";

    public string ForeignTable { get; set; } = "";

    public string ForeignColumn { get; set; } = "";
}
