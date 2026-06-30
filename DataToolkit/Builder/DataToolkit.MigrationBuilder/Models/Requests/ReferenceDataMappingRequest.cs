using DataToolkit.MigrationBuilder.Models.Connections;
using System.ComponentModel;

namespace DataToolkit.MigrationBuilder.Models.Requests;

public sealed class ReferenceDataMappingRequest
{
    public ConnectionRequest SourceConnectionString { get; set; }

    public ConnectionRequest TargetConnectionString { get; set; }

    [DefaultValue("dbo")]
    public string? Schema { get; set; }

    public string SourceTable { get; set; } = string.Empty;

    public string TargetTable { get; set; } = string.Empty;
}