namespace DataToolkit.Builder.Models.Requests;

public sealed class ReferenceDataMappingRequest
{
    public string SourceConnectionString { get; set; } = string.Empty;

    public string TargetConnectionString { get; set; } = string.Empty;

    public string SourceTable { get; set; } = string.Empty;

    public string TargetTable { get; set; } = string.Empty;
}
