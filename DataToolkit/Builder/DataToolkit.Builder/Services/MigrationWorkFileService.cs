using System.Text.Json;
using DataToolkit.Builder.Helpers;
using DataToolkit.Builder.Models;

namespace DataToolkit.Builder.Services;

public sealed class MigrationWorkFileService
{
    public async Task GenerateMigrationWorkFilesAsync(
        List<TableMetadata> sourceMetadata,
        List<TableMetadata> targetMetadata,
        string outputPath)
    {
        Directory.CreateDirectory(outputPath);

        foreach (var sourceTable in sourceMetadata)
        {
            var targetTable =
                targetMetadata.FirstOrDefault(t =>
                    t.Schema.Equals(
                        sourceTable.Schema,
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    t.Name.Equals(
                        sourceTable.Name,
                        StringComparison.OrdinalIgnoreCase));

            if (targetTable is null)
                continue;

            var workFile = new MigrationWorkFile
            {
                Schema = sourceTable.Schema,
                Table = sourceTable.Name,
                SourceTable = sourceTable.Name,
                TargetTable = targetTable.Name
            };

            // =====================================================
            // SOURCE -> TARGET
            // =====================================================

            foreach (var sourceColumn in sourceTable.Columns)
            {
                var targetColumn =
                    targetTable.Columns.FirstOrDefault(c =>
                        c.Name.Equals(
                            sourceColumn.Name,
                            StringComparison.OrdinalIgnoreCase));

                // -------------------------------------------------
                // DIRECT / CONVERT
                // -------------------------------------------------

                if (targetColumn is not null)
                {
                    workFile.Columns.Add(new ColumnMapping
                    {
                        SourceColumn = sourceColumn.Name,
                        TargetColumn = targetColumn.Name,
                        Rule = DetermineRule(
                            sourceColumn,
                            targetColumn),
                        Required = !targetColumn.IsNullable
                    });

                    continue;
                }

                // -------------------------------------------------
                // REVIEW (heurística simple)
                // -------------------------------------------------

                var similarColumn =
                    targetTable.Columns.FirstOrDefault(c =>
                        c.Name.Contains(
                            sourceColumn.Name,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        sourceColumn.Name.Contains(
                            c.Name,
                            StringComparison.OrdinalIgnoreCase));

                if (similarColumn is not null)
                {
                    workFile.Columns.Add(new ColumnMapping
                    {
                        SourceColumn = sourceColumn.Name,
                        TargetColumn = similarColumn.Name,
                        Rule = MappingRules.Review,
                        Required = !similarColumn.IsNullable
                    });

                    continue;
                }

                // -------------------------------------------------
                // UNMAPPED
                // -------------------------------------------------

                workFile.Columns.Add(new ColumnMapping
                {
                    SourceColumn = sourceColumn.Name,
                    TargetColumn = null,
                    Rule = MappingRules.Unmapped,
                    Required = false
                });
            }

            // =====================================================
            // TARGET ONLY -> DEFAULT
            // =====================================================

            foreach (var targetColumn in targetTable.Columns)
            {
                bool alreadyMapped =
                    workFile.Columns.Any(c =>
                        c.TargetColumn?.Equals(
                            targetColumn.Name,
                            StringComparison.OrdinalIgnoreCase) == true);

                if (alreadyMapped)
                    continue;

                workFile.Columns.Add(new ColumnMapping
                {
                    SourceColumn = null,
                    TargetColumn = targetColumn.Name,
                    Rule = MappingRules.Default,
                    Required = !targetColumn.IsNullable
                });
            }

            var fileName =
                $"WF_{sourceTable.Schema}.{sourceTable.Name}.json";

            var filePath =
                Path.Combine(
                    outputPath,
                    fileName);

            var json =
                JsonSerializer.Serialize(
                    workFile,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

            await File.WriteAllTextAsync(
                filePath,
                json);
        }
    }

    private static string DetermineRule(
        ColumnMetadata sourceColumn,
        ColumnMetadata targetColumn)
    {
        if (!sourceColumn.SqlType.Equals(
                targetColumn.SqlType,
                StringComparison.OrdinalIgnoreCase))
        {
            return MappingRules.Convert;
        }

        return MappingRules.Direct;
    }
}