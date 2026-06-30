namespace DataToolkit.MigrationBuilder.Models.Responses;

public sealed class ReferenceDataMappingResult
{
    public string SourceTable { get; set; } = string.Empty;

    public string TargetTable { get; set; } = string.Empty;

    public List<ReferenceDataMatch> Matches { get; set; } = [];
}