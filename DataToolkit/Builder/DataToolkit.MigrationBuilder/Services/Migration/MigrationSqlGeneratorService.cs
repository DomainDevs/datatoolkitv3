using DataToolkit.MigrationBuilder.Configuration;
using DataToolkit.MigrationBuilder.Helpers;
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

    private List<string> ResolveWorkFiles(GenerateSqlRequest request)
    {
        List<string> workFiles = new();

        if (request.WorkFiles.Count == 0)
        {
            workFiles.AddRange(
                Directory.GetFiles(
                    _options.WorkFilePath,
                    "WF_*.json"));
        }
        else
        {
            foreach (string workFile in request.WorkFiles)
            {
                workFiles.Add(
                    Path.Combine(
                        _options.WorkFilePath,
                        "WF_dbo."+workFile+ ".json"));
            }
        }

        return workFiles;
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
INSERT INTO [{workFile.Schema}].[WF_{workFile.TargetTable}]
(
{targetColumns}
)
SELECT
{sourceColumns}
FROM [{workFile.Schema}].[{workFile.SourceTable}];
""";
    }
}