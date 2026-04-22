namespace DataToolkit.Library.Fluent.Sql;

public sealed record SqlWhere(SqlNode Expression) : SqlNode;