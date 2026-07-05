namespace DataToolkit.MigrationBuilder.Services.Metadata.Internal;

/// <summary>
/// Consultas SQL utilizadas por el motor de extracción de metadata.
/// Todas las consultas al catálogo de SQL Server deben vivir aquí.
/// </summary>
internal static class SqlQueries
{
    #region Tables

    public const string Tables =
    """
    SELECT
        t.object_id,
        s.name  AS SchemaName,
        t.name  AS TableName
    FROM sys.tables t
        INNER JOIN sys.schemas s
            ON s.schema_id = t.schema_id
    WHERE
        (@IncludeSystemTables = 1
         OR t.is_ms_shipped = 0)
    ORDER BY
        s.name,
        t.name;
    """;

    #endregion

    #region Columns

    public const string Columns =
    """
    SELECT
        t.object_id,

        c.column_id,

        c.name,

        ty.name AS SqlType,

        c.max_length,

        c.precision,

        c.scale,

        c.is_nullable,

        c.is_identity,

        c.is_computed,

        c.is_rowguidcol,

        cc.definition AS ComputedDefinition,

        dc.definition AS DefaultValue,

        c.collation_name

    FROM sys.tables t

        INNER JOIN sys.columns c
            ON t.object_id = c.object_id

        INNER JOIN sys.types ty
            ON c.user_type_id = ty.user_type_id

        LEFT JOIN sys.computed_columns cc
            ON cc.object_id = c.object_id
           AND cc.column_id = c.column_id

        LEFT JOIN sys.default_constraints dc
            ON dc.parent_object_id = c.object_id
           AND dc.parent_column_id = c.column_id

    ORDER BY
        t.object_id,
        c.column_id;
    """;

    #endregion

    #region Primary Keys

    public const string PrimaryKeys =
    """
    SELECT

        kc.parent_object_id,

        c.column_id,

        kc.name,

        c.name AS ColumnName

    FROM sys.key_constraints kc

        INNER JOIN sys.index_columns ic

            ON kc.parent_object_id = ic.object_id
           AND kc.unique_index_id = ic.index_id

        INNER JOIN sys.columns c

            ON ic.object_id = c.object_id
           AND ic.column_id = c.column_id

    WHERE kc.type = 'PK';
    """;

    #endregion

    #region Foreign Keys

    public const string ForeignKeys =
    """
    SELECT

        fk.parent_object_id,

        pc.column_id,

        fk.name,

        rt.name AS ReferencedTable,

        rc.name AS ReferencedColumn,

        fk.delete_referential_action_desc,

        fk.update_referential_action_desc,

        fk.is_disabled,

        fk.is_not_trusted

    FROM sys.foreign_keys fk

        INNER JOIN sys.foreign_key_columns fkc

            ON fk.object_id = fkc.constraint_object_id

        INNER JOIN sys.columns pc

            ON pc.object_id = fkc.parent_object_id
           AND pc.column_id = fkc.parent_column_id

        INNER JOIN sys.tables rt

            ON rt.object_id = fkc.referenced_object_id

        INNER JOIN sys.columns rc

            ON rc.object_id = fkc.referenced_object_id
           AND rc.column_id = fkc.referenced_column_id;
    """;

    #endregion

    #region Indexes

    public const string Indexes =
    """
    SELECT

        i.object_id,

        ic.column_id,

        i.name,

        i.is_unique

    FROM sys.indexes i

        INNER JOIN sys.index_columns ic

            ON i.object_id = ic.object_id
           AND i.index_id = ic.index_id

    WHERE

        i.is_primary_key = 0
        AND i.is_hypothetical = 0;
    """;

    #endregion

    #region Extended Properties

    public const string ExtendedProperties =
    """
    SELECT

        ep.major_id,

        ep.minor_id,

        CAST(ep.value AS nvarchar(max)) AS Description

    FROM sys.extended_properties ep

    WHERE

        ep.name = 'MS_Description';
    """;

    #endregion
}