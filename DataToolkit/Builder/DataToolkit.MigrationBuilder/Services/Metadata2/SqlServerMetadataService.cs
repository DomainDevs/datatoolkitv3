using DataToolkit.MigrationBuilder.Configuration;
using DataToolkit.MigrationBuilder.Helpers;
using DataToolkit.MigrationBuilder.Infrastructure;
using DataToolkit.MigrationBuilder.Models.Medatata2;
using Microsoft.Extensions.Options;
using System.Text;

namespace DataToolkit.MigrationBuilder.Services.Metadata2;

/// <summary>
/// Extrae metadata y genera el DDL base para origen y destino.
/// </summary>
public sealed class SqlServerMetadataService
{
    private readonly MigrationConfiguration _configuration;

    public SqlServerMetadataService(
        IOptions<MigrationConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public async Task<string> GenerateDdlAsync(
        DatabaseMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        var ddl = new StringBuilder();

        foreach (var table in metadata.Tables)
        {
            ddl.AppendLine($"CREATE TABLE [{table.Schema}].[{table.Name}]");
            ddl.AppendLine("(");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                var c = table.Columns[i];

                ddl.Append($"    [{c.Name}] {c.SqlType}");

                // Tipos que usan longitud (varchar, nvarchar, char, nchar, binary, varbinary)
                if (UsesLength(c.SqlType) && c.MaxLength.HasValue)
                {
                    ddl.Append(c.MaxLength.Value == -1
                        ? "(MAX)"
                        : $"({c.MaxLength.Value})");
                }
                // Tipos que usan precisión y escala (decimal, numeric)
                else if (UsesPrecisionAndScale(c.SqlType) && c.Precision.HasValue)
                {
                    ddl.Append($"({c.Precision.Value}");

                    if (c.Scale.HasValue)
                        ddl.Append($",{c.Scale.Value}");

                    ddl.Append(")");
                }
                // Tipos que usan solo precisión (datetime2, datetimeoffset, time)
                else if (UsesPrecision(c.SqlType) && c.Precision.HasValue)
                {
                    ddl.Append($"({c.Precision.Value})");
                }

                if (c.IsIdentity)
                    ddl.Append(" IDENTITY(1,1)");

                ddl.Append(c.IsNullable ? " NULL" : " NOT NULL");

                if (!string.IsNullOrWhiteSpace(c.DefaultValue))
                    ddl.Append($" DEFAULT {c.DefaultValue}");

                if (i < table.Columns.Count - 1 || table.Columns.Exists(x => x.IsPrimaryKey))
                    ddl.Append(",");

                ddl.AppendLine();
            }

            var pk = table.Columns.Where(x => x.IsPrimaryKey).ToList();

            if (pk.Count > 0)
            {
                ddl.AppendLine($"    CONSTRAINT [PK_{table.Name}] PRIMARY KEY (");
                ddl.AppendLine("        " + string.Join(", ", pk.Select(x => $"[{x.Name}]")));
                ddl.AppendLine("    )");
            }

            ddl.AppendLine(");");
            ddl.AppendLine();
        }

        await Task.CompletedTask;
        return ddl.ToString();
    }


    private static bool UsesLength(string sqlType)
    {
        return sqlType.Equals("varchar", StringComparison.OrdinalIgnoreCase)
            || sqlType.Equals("nvarchar", StringComparison.OrdinalIgnoreCase)
            || sqlType.Equals("char", StringComparison.OrdinalIgnoreCase)
            || sqlType.Equals("nchar", StringComparison.OrdinalIgnoreCase)
            || sqlType.Equals("binary", StringComparison.OrdinalIgnoreCase)
            || sqlType.Equals("varbinary", StringComparison.OrdinalIgnoreCase);
    }

    private static bool UsesPrecisionAndScale(string sqlType)
    {
        return sqlType.Equals("decimal", StringComparison.OrdinalIgnoreCase)
            || sqlType.Equals("numeric", StringComparison.OrdinalIgnoreCase);
    }

    private static bool UsesPrecision(string sqlType)
    {
        return sqlType.Equals("datetime2", StringComparison.OrdinalIgnoreCase)
            || sqlType.Equals("datetimeoffset", StringComparison.OrdinalIgnoreCase)
            || sqlType.Equals("time", StringComparison.OrdinalIgnoreCase);
    }
}
