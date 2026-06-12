using DataToolkit.Library.Fluent.Sql;

namespace DataToolkit.Library.Engine.Fluent.Sql;

internal sealed record SqlSkip(int Value)
    : SqlNode;