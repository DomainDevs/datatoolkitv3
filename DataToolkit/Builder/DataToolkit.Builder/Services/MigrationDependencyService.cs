namespace DataToolkit.Builder.Services;

public class MigrationDependencyService
{
    public List<TableDependency> GetDependencies(
        string tableName,
        List<TableMetadata> metadata)
    {
        var result = new List<TableDependency>();

        var table =
            metadata.FirstOrDefault(t =>
                string.Equals(
                    t.Name,
                    tableName,
                    StringComparison.OrdinalIgnoreCase));

        if (table == null)
            return result;

        foreach (var column in table.Columns)
        {
            if (string.IsNullOrWhiteSpace(column.ForeignTable))
                continue;

            result.Add(new TableDependency
            {
                Table = table.Name,
                Column = column.Name,
                ForeignTable = column.ForeignTable!,
                ForeignColumn = column.ForeignColumn ?? ""
            });
        }

        return result;
    }
}