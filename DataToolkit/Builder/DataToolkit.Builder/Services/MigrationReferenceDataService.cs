namespace DataToolkit.Builder.Services;

using DataToolkit.Builder.Models.Responses;
using DataToolkit.Builder.Services.Interfaces;
using System.Text;

public sealed class MigrationReferenceDataService
    : IMigrationReferenceDataService
{
    public async Task<string> GenerateHomologationScriptAsync(
        ReferenceDataMappingResult mapping,
        string outputPath)
    {
        Directory.CreateDirectory(outputPath);

        var tableName =
            $"HOMOLOGACION_{mapping.SourceTable.ToUpper()}";

        var fileName =
            Path.Combine(outputPath, $"{tableName}.sql");

        var sql = new StringBuilder();

        sql.AppendLine("-- =============================================");
        sql.AppendLine($"-- Tabla: {tableName}");
        sql.AppendLine($"-- Origen: {mapping.SourceTable}");
        sql.AppendLine($"-- Destino: {mapping.TargetTable}");
        sql.AppendLine("-- Generado por DataToolkit");
        sql.AppendLine("-- =============================================");
        sql.AppendLine();

        sql.AppendLine($"IF OBJECT_ID('dbo.{tableName}') IS NOT NULL");
        sql.AppendLine($"    DROP TABLE dbo.{tableName};");
        sql.AppendLine("GO");
        sql.AppendLine();

        sql.AppendLine($"CREATE TABLE dbo.{tableName}");
        sql.AppendLine("(");
        sql.AppendLine("    VALOR_ORIGEN         VARCHAR(100) NOT NULL,");
        sql.AppendLine("    DESCRIPCION_ORIGEN   VARCHAR(500) NULL,");
        sql.AppendLine();
        sql.AppendLine("    VALOR_DESTINO        VARCHAR(100) NULL,");
        sql.AppendLine("    DESCRIPCION_DESTINO  VARCHAR(500) NULL,");
        sql.AppendLine();
        sql.AppendLine("    CONFIANZA            DECIMAL(5,2) NOT NULL");
        sql.AppendLine(");");
        sql.AppendLine("GO");
        sql.AppendLine();

        foreach (var match in mapping.Matches)
        {
            sql.AppendLine($"INSERT INTO dbo.{tableName}");
            sql.AppendLine("(");
            sql.AppendLine("    VALOR_ORIGEN,");
            sql.AppendLine("    DESCRIPCION_ORIGEN,");
            sql.AppendLine("    VALOR_DESTINO,");
            sql.AppendLine("    DESCRIPCION_DESTINO,");
            sql.AppendLine("    CONFIANZA");
            sql.AppendLine(")");
            sql.AppendLine("VALUES");
            sql.AppendLine("(");
            sql.AppendLine($"    '{Escape(match.SourceValue)}',");
            sql.AppendLine($"    '{Escape(match.SourceDescription)}',");
            sql.AppendLine($"    '{Escape(match.TargetValue)}',");
            sql.AppendLine($"    '{Escape(match.TargetDescription)}',");
            sql.AppendLine($"    {match.Confidence}");
            sql.AppendLine(");");
            sql.AppendLine("GO");
            sql.AppendLine();
        }

        await File.WriteAllTextAsync(fileName, sql.ToString());

        return fileName;
    }

    private static string Escape(string? value)
    {
        return value?.Replace("'", "''") ?? string.Empty;
    }
}
