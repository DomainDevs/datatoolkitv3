namespace DataToolkit.MigrationBuilder.Services.Migration.Homologation;

using DataToolkit.MigrationBuilder.Helpers;
using DataToolkit.MigrationBuilder.Models;
using DataToolkit.MigrationBuilder.Models.Requests;
using DataToolkit.MigrationBuilder.Services.Migration.Homologation.Interfaces;
using Microsoft.Data.SqlClient;

public sealed class HomologationDiscoveryService
    : IHomologationDiscoveryService
{
    public async Task<HomologationResult> DiscoverAsync(
        ReferenceDataMappingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sourceCs =
            new SqlConnectionStringBuilder
            {
                DataSource =
                    request.SourceConnectionString.Server,

                InitialCatalog =
                    request.SourceConnectionString.Database,

                UserID =
                    request.SourceConnectionString.User,

                Password =
                    request.SourceConnectionString.Password,

                TrustServerCertificate = true,
                Encrypt = false
            };

        var targetCs =
            new SqlConnectionStringBuilder
            {
                DataSource =
                    request.TargetConnectionString.Server,

                InitialCatalog =
                    request.TargetConnectionString.Database,

                UserID =
                    request.TargetConnectionString.User,

                Password =
                    request.TargetConnectionString.Password,

                TrustServerCertificate = true,
                Encrypt = false
            };

        await using var sourceConnection =
            new SqlConnection(
                sourceCs.ConnectionString);

        await using var targetConnection =
            new SqlConnection(
                targetCs.ConnectionString);

        await sourceConnection.OpenAsync();
        await targetConnection.OpenAsync();

        var schema =
            string.IsNullOrWhiteSpace(request.Schema)
                ? "dbo"
                : request.Schema;

        var sourceColumns =
            await DetectColumnsAsync(
                sourceConnection,
                schema,
                request.SourceTable);

        var targetColumns =
            await DetectColumnsAsync(
                targetConnection,
                schema,
                request.TargetTable);

        var sourceRows =
            await ReadRowsAsync(
                sourceConnection,
                schema,
                request.SourceTable,
                sourceColumns.CodeColumn,
                sourceColumns.DescriptionColumn);

        var targetRows =
            await ReadRowsAsync(
                targetConnection,
                schema,
                request.TargetTable,
                targetColumns.CodeColumn,
                targetColumns.DescriptionColumn);

        var matches =
            BuildMatches(
                sourceRows,
                targetRows);

        return new HomologationResult
        {
            SourceTable = request.SourceTable,
            TargetTable = request.TargetTable,

            SourceCodeColumn =
                sourceColumns.CodeColumn,

            SourceDescriptionColumn =
                sourceColumns.DescriptionColumn,

            TargetCodeColumn =
                targetColumns.CodeColumn,

            TargetDescriptionColumn =
                targetColumns.DescriptionColumn,

            Matches = matches
        };
    }

    private async Task<ColumnInfo> DetectColumnsAsync(
        SqlConnection connection,
        string schema,
        string tableName)
    {
        var columns =
            new List<string>();

        const string sql =
        """
        SELECT COLUMN_NAME
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = @Schema
          AND TABLE_NAME = @TableName
        ORDER BY ORDINAL_POSITION
        """;

        using var cmd =
            new SqlCommand(
                sql,
                connection);

        cmd.Parameters.AddWithValue(
            "@Schema",
            schema);

        cmd.Parameters.AddWithValue(
            "@TableName",
            tableName);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            columns.Add(
                reader.GetString(0));
        }

        if (columns.Count < 2)
        {
            throw new InvalidOperationException(
                $"No se pudieron detectar columnas suficientes para la tabla '{schema}.{tableName}'.");
        }

        var detectedCode =
            columns.FirstOrDefault(x =>
                x.Equals(
                    "CODIGO",
                    StringComparison.OrdinalIgnoreCase))
            ??
            columns.FirstOrDefault(x =>
                x.Contains(
                    "CODIGO",
                    StringComparison.OrdinalIgnoreCase))
            ??
            columns.FirstOrDefault(x =>
                x.Contains(
                    "ID",
                    StringComparison.OrdinalIgnoreCase))
            ??
            columns.First();

        if (string.IsNullOrWhiteSpace(detectedCode))
        {
            throw new InvalidOperationException(
                $"No se pudo detectar la columna código para la tabla '{schema}.{tableName}'.");
        }

        var detectedDescription =
            columns.FirstOrDefault(x =>
                x.Contains(
                    "DESCRIP",
                    StringComparison.OrdinalIgnoreCase))
            ??
            columns.FirstOrDefault(x =>
                x.Contains(
                    "NOMBRE",
                    StringComparison.OrdinalIgnoreCase))
            ??
            columns.Last();

        if (string.IsNullOrWhiteSpace(detectedDescription))
        {
            throw new InvalidOperationException(
                $"No se pudo detectar la columna descripción para la tabla '{schema}.{tableName}'.");
        }

        return new ColumnInfo
        {
            CodeColumn = detectedCode,
            DescriptionColumn = detectedDescription
        };
    }

    private async Task<List<ReferenceRow>> ReadRowsAsync(
        SqlConnection connection,
        string schema,
        string tableName,
        string codeColumn,
        string descriptionColumn)
    {
        var result =
            new List<ReferenceRow>();

        var sql =
        $"""
        SELECT
            [{codeColumn}],
            [{descriptionColumn}]
        FROM [{schema}].[{tableName}]
        """;

        using var cmd =
            new SqlCommand(
                sql,
                connection);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(
                new ReferenceRow
                {
                    Code =
                        reader[0]?.ToString()
                        ?? string.Empty,

                    Description =
                        reader[1]?.ToString()
                        ?? string.Empty
                });
        }

        return result;
    }

    private List<ReferenceDataMatch> BuildMatches(
        List<ReferenceRow> sourceRows,
        List<ReferenceRow> targetRows)
    {
        var result =
            new List<ReferenceDataMatch>();

        foreach (var source in sourceRows)
        {
            var bestMatch =
                targetRows
                    .Select(target => new
                    {
                        Target = target,
                        Score = CalculateSimilarity(
                            source,
                            target)
                    })
                    .OrderByDescending(x => x.Score)
                    .FirstOrDefault();

            if (bestMatch is null ||
                bestMatch.Score == 0)
            {
                result.Add(
                    new ReferenceDataMatch
                    {
                        SourceValue = source.Code,
                        SourceDescription = source.Description,
                        Confidence = 0,
                        Status = MappingStatus.Unmapped,
                        Comment = "No match found"
                    });

                continue;
            }

            result.Add(
                new ReferenceDataMatch
                {
                    SourceValue = source.Code,
                    SourceDescription = source.Description,

                    TargetValue = bestMatch.Target.Code,
                    TargetDescription = bestMatch.Target.Description,

                    Confidence = bestMatch.Score,

                    Status =
                        bestMatch.Score >= 90
                            ? MappingStatus.Auto
                            : MappingStatus.Review,

                    Comment =
                        bestMatch.Score >= 90
                            ? "Automatic match"
                            : "Review suggested"
                });
        }

        return result;
    }

    private static decimal CalculateSimilarity(
        ReferenceRow source,
        ReferenceRow target)
    {
        if (source.Code.Equals(
            target.Code,
            StringComparison.OrdinalIgnoreCase))
        {
            return 100;
        }

        if (source.Description.Equals(
            target.Description,
            StringComparison.OrdinalIgnoreCase))
        {
            return 95;
        }

        if (target.Description.Contains(
            source.Description,
            StringComparison.OrdinalIgnoreCase))
        {
            return 80;
        }

        if (source.Description.Contains(
            target.Description,
            StringComparison.OrdinalIgnoreCase))
        {
            return 80;
        }

        return 0;
    }

    private sealed class ColumnInfo
    {
        public string CodeColumn { get; set; }
            = string.Empty;

        public string DescriptionColumn { get; set; }
            = string.Empty;
    }

    private sealed class ReferenceRow
    {
        public string Code { get; set; }
            = string.Empty;

        public string Description { get; set; }
            = string.Empty;
    }
}