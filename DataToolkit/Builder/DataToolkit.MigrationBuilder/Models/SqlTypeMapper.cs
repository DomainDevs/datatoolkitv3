namespace DataToolkit.MigrationBuilder.Models;

/// <summary>
/// Maps SQL data types to normalized SQL Server native definitions.
/// This class centralizes type normalization so DDL generators never
/// emit invalid definitions such as INT(4) or SMALLDATETIME(4).
/// </summary>
public static class SqlTypeMapper
{
    public static string BuildType(
        string sqlType,
        int? length = null,
        int? precision = null,
        int? scale = null)
    {
        if (string.IsNullOrWhiteSpace(sqlType))
            return "NVARCHAR(MAX)";

        var type = sqlType.Trim().ToUpperInvariant();

        return type switch
        {
            "VARCHAR" or "VARCHAR2"
                => $"VARCHAR({ResolveLength(length)})",

            "NVARCHAR" or "NVARCHAR2"
                => $"NVARCHAR({ResolveLength(length)})",

            "CHAR"
                => $"CHAR({Math.Max(length ?? 1, 1)})",

            "NCHAR"
                => $"NCHAR({Math.Max(length ?? 1, 1)})",

            "TEXT" or "NTEXT" or "CLOB" or "NCLOB" or "LONGTEXT"
                => "NVARCHAR(MAX)",

            "BINARY"
                => $"BINARY({Math.Max(length ?? 1, 1)})",

            "VARBINARY" or "IMAGE" or "BLOB"
                => length is > 0 ? $"VARBINARY({length})" : "VARBINARY(MAX)",

            "DECIMAL" or "NUMERIC" or "NUMBER"
                => $"DECIMAL({precision ?? 18},{scale ?? 0})",

            "FLOAT" or "DOUBLE"
                => "FLOAT",

            "REAL"
                => "REAL",

            "BIT" or "BOOLEAN"
                => "BIT",

            "TINYINT"
                => "TINYINT",

            "SMALLINT"
                => "SMALLINT",

            "INT" or "INTEGER"
                => "INT",

            "BIGINT"
                => "BIGINT",

            "MONEY"
                => "MONEY",

            "SMALLMONEY"
                => "SMALLMONEY",

            "DATE"
                => "DATE",

            "TIME"
                => scale.HasValue
                    ? $"TIME({scale.Value})"
                    : "TIME",

            "DATETIME"
                => "DATETIME",

            "DATETIME2"
                => scale.HasValue
                    ? $"DATETIME2({scale.Value})"
                    : "DATETIME2",

            "SMALLDATETIME"
                => "SMALLDATETIME",

            "TIMESTAMP"
                => "DATETIME2",

            "UNIQUEIDENTIFIER" or "UUID"
                => "UNIQUEIDENTIFIER",

            "XML" or "XMLTYPE"
                => "XML",

            _ => "NVARCHAR(MAX)"
        };
    }

    private static string ResolveLength(int? length)
    {
        if (!length.HasValue || length.Value <= 0)
            return "MAX";

        return length.Value.ToString();
    }

    public static bool SupportsLength(string sqlType)
    {
        var t = sqlType.ToUpperInvariant();

        return t is "VARCHAR" or "VARCHAR2"
            or "NVARCHAR" or "NVARCHAR2"
            or "CHAR"
            or "NCHAR"
            or "BINARY"
            or "VARBINARY";
    }

    public static bool SupportsPrecision(string sqlType)
    {
        var t = sqlType.ToUpperInvariant();

        return t is "DECIMAL"
            or "NUMERIC"
            or "NUMBER"
            or "TIME"
            or "DATETIME2";
    }
}
