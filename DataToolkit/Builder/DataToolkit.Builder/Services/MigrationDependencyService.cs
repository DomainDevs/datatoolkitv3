namespace DataToolkit.Builder.Services;

public sealed class MigrationDependencyService
{
    public List<TableDependency> GetDependencies(
        string tableName,
        List<TableMetadata> metadata,
        int dependencyLevel = 1)
    {
        if (dependencyLevel < 1)
            dependencyLevel = 1;

        var result = new List<TableDependency>();

        LoadDependencies(
            tableName,
            metadata,
            result,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            1,
            dependencyLevel);

        return result
            .OrderBy(x => x.Level)
            .ThenBy(x => x.Table)
            .ThenBy(x => x.ForeignTable)
            .ToList();
    }

    private void LoadDependencies(
        string tableName,
        List<TableMetadata> metadata,
        List<TableDependency> result,
        HashSet<string> visited,
        int currentLevel,
        int maxLevel)
    {
        if (currentLevel > maxLevel)
            return;

        if (!visited.Add(tableName))
            return;

        var table = metadata.FirstOrDefault(t =>
            string.Equals(
                t.Name,
                tableName,
                StringComparison.OrdinalIgnoreCase));

        if (table == null)
            return;

        foreach (var column in table.Columns)
        {
            if (string.IsNullOrWhiteSpace(column.ForeignTable))
                continue;

            result.Add(new TableDependency
            {
                Table = table.Name,
                Column = column.Name,
                ForeignTable = column.ForeignTable!,
                ForeignColumn = column.ForeignColumn ?? string.Empty,
                Level = currentLevel
            });

            LoadDependencies(
                column.ForeignTable!,
                metadata,
                result,
                visited,
                currentLevel + 1,
                maxLevel);
        }
    }
}