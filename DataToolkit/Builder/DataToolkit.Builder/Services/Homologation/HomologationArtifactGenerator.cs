namespace DataToolkit.Builder.Services.Homologation;

using DataToolkit.Builder.Helpers;
using DataToolkit.Builder.Models;
using DataToolkit.Builder.Services.Homologation.Interfaces;
using System.Text;

public sealed class HomologationArtifactGenerator
    : IHomologationArtifactGenerator
{
    public async Task<string> GenerateHomologationScriptAsync(
        HomologationResult mapping,
        string outputPath)
    {
        ArgumentNullException.ThrowIfNull(mapping);

        Directory.CreateDirectory(outputPath);

        var tableName =
            $"HOMOLOGACION_{mapping.SourceTable.ToUpper()}";

        var fileName =
            Path.Combine(
                outputPath,
                $"{tableName}.sql");

        var sql =
            new StringBuilder();

        sql.AppendLine("-- =============================================");
        sql.AppendLine("-- HOMOLOGACION GENERADA POR DATATOOLKIT");
        sql.AppendLine("-- =============================================");
        sql.AppendLine($"-- Tabla Origen  : {mapping.SourceTable}");
        sql.AppendLine($"-- Tabla Destino : {mapping.TargetTable}");
        sql.AppendLine($"-- Fecha         : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sql.AppendLine("-- =============================================");
        sql.AppendLine();

        GenerateTable(
            sql,
            tableName);

        GenerateSuggestedMappings(
            sql,
            tableName,
            mapping);

        GenerateSummary(
            sql,
            mapping);

        GenerateMigrationTemplate(
            sql,
            tableName,
            mapping);

        await File.WriteAllTextAsync(
            fileName,
            sql.ToString());

        return fileName;
    }

    private static void GenerateTable(
        StringBuilder sql,
        string tableName)
    {
        sql.AppendLine("-- =============================================");
        sql.AppendLine("-- TABLA HOMOLOGACION");
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

        sql.AppendLine("    CONFIANZA            DECIMAL(5,2) NOT NULL,");
        sql.AppendLine("    ESTADO               VARCHAR(20) NOT NULL,");
        sql.AppendLine("    OBSERVACION          VARCHAR(500) NULL");

        sql.AppendLine(");");
        sql.AppendLine("GO");
        sql.AppendLine();
    }

    private static void GenerateSuggestedMappings(
        StringBuilder sql,
        string tableName,
        HomologationResult mapping)
    {
        sql.AppendLine("-- =============================================");
        sql.AppendLine("-- SUGERENCIAS DE HOMOLOGACION");
        sql.AppendLine("-- =============================================");
        sql.AppendLine();

        foreach (var match in mapping.Matches
                     .Where(x =>
                         x.Status != MappingStatus.Unmapped))
        {
            sql.AppendLine($"INSERT INTO dbo.{tableName}");
            sql.AppendLine("(");
            sql.AppendLine("    VALOR_ORIGEN,");
            sql.AppendLine("    DESCRIPCION_ORIGEN,");
            sql.AppendLine("    VALOR_DESTINO,");
            sql.AppendLine("    DESCRIPCION_DESTINO,");
            sql.AppendLine("    CONFIANZA,");
            sql.AppendLine("    ESTADO,");
            sql.AppendLine("    OBSERVACION");
            sql.AppendLine(")");
            sql.AppendLine("VALUES");
            sql.AppendLine("(");

            sql.AppendLine(
                $"    '{Escape(match.SourceValue)}',");

            sql.AppendLine(
                $"    '{Escape(match.SourceDescription)}',");

            sql.AppendLine(
                $"    '{Escape(match.TargetValue)}',");

            sql.AppendLine(
                $"    '{Escape(match.TargetDescription)}',");

            sql.AppendLine(
                $"    {match.Confidence},");

            sql.AppendLine(
                $"    '{match.Status}',");

            sql.AppendLine(
                $"    '{Escape(match.Comment)}'");

            sql.AppendLine(");");
            sql.AppendLine("GO");
            sql.AppendLine();
        }
    }

    private static void GenerateSummary(
        StringBuilder sql,
        HomologationResult mapping)
    {
        sql.AppendLine();
        sql.AppendLine("-- =============================================");
        sql.AppendLine("-- RESUMEN");
        sql.AppendLine("-- =============================================");

        foreach (var match in mapping.Matches)
        {
            sql.AppendLine(
                $"-- [{match.Status}] " +
                $"{match.SourceValue} -> {match.TargetValue} " +
                $"({match.Confidence}%)");
        }

        sql.AppendLine();
    }

    private static void GenerateMigrationTemplate(
        StringBuilder sql,
        string tableName,
        HomologationResult mapping)
    {
        sql.AppendLine("-- =============================================");
        sql.AppendLine("-- SCRIPT DE MIGRACION SUGERIDO");
        sql.AppendLine("-- =============================================");
        sql.AppendLine();

        sql.AppendLine("-- Ajustar nombres de tablas destino");
        sql.AppendLine();

        sql.AppendLine("SELECT");
        sql.AppendLine($"    O.[{mapping.SourceCodeColumn}],");
        sql.AppendLine($"    H.VALOR_DESTINO");
        sql.AppendLine($"FROM dbo.[{mapping.SourceTable}] O");
        sql.AppendLine($"LEFT JOIN dbo.[{tableName}] H");
        sql.AppendLine(
            $"    ON H.VALOR_ORIGEN = O.[{mapping.SourceCodeColumn}]");
        sql.AppendLine(";");
        sql.AppendLine();
    }

    private static string Escape(
        string? value)
    {
        return value?.Replace(
                   "'",
                   "''")
               ?? string.Empty;
    }
}