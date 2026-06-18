using DataToolkit.Builder.Configuration;
using DataToolkit.Builder.Helpers;
using Microsoft.Extensions.Options;
using System.Text.Json;

public sealed class MigrationSqlGeneratorService
{
    private readonly MigrationOptions _options;

    public MigrationSqlGeneratorService(
        IOptions<MigrationOptions> options)
    {
        _options = options.Value;
    }

    public async Task GenerateSqlScriptsAsync(
        GenerateSqlRequest request)
    {
        Directory.CreateDirectory(
            _options.SqlOutputPath);

        var workFiles = ResolveWorkFiles(request);

        foreach (var workFilePath in workFiles)
        {
            var json =
                await File.ReadAllTextAsync(
                    workFilePath);

            var workFile =
                JsonSerializer.Deserialize<MigrationWorkFile>(
                    json);

            if (workFile is null)
                continue;

            var sql =
                BuildInsertSelect(workFile);

            var fileName =
                Path.GetFileNameWithoutExtension(
                    workFilePath);

            fileName =
                fileName.Replace("WF_", "SQL_");

            var sqlFile =
                Path.Combine(
                    _options.SqlOutputPath,
                    $"{fileName}.sql");

            await File.WriteAllTextAsync(
                sqlFile,
                sql);
        }
    }

    private List<string> ResolveWorkFiles(
        GenerateSqlRequest request)
    {
        if (request.WorkFiles.Count == 0)
        {
            return Directory
                .GetFiles(
                    _options.WorkFilePath,
                    "WF_*.json")
                .ToList();
        }

        return request.WorkFiles
            .Select(x =>
                Path.Combine(
                    _options.WorkFilePath,
                    x))
            .ToList();
    }

    private static string BuildInsertSelect(
        MigrationWorkFile workFile)
    {
        var validColumns =
            workFile.Columns
                .Where(c =>
                    c.Rule != MappingRules.Unmapped
                    &&
                    c.SourceColumn is not null
                    &&
                    c.TargetColumn is not null)
                .ToList();

        var targetColumns =
            string.Join(
                "," + Environment.NewLine,
                validColumns.Select(c =>
                    $"    [{c.TargetColumn}]"));

        var sourceColumns =
            string.Join(
                "," + Environment.NewLine,
                validColumns.Select(c =>
                    $"    [{c.SourceColumn}]"));

        return
$"""
INSERT INTO [{workFile.Schema}].[{workFile.TargetTable}]
(
{targetColumns}
)
SELECT
{sourceColumns}
FROM [{workFile.Schema}].[{workFile.SourceTable}];
""";
    }
}