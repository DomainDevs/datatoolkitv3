namespace DataToolkit.MigrationBuilder.Models;

public sealed class HomologationResult
{
    public string SourceTable { get; set; } = string.Empty;

    public string TargetTable { get; set; } = string.Empty;

    public List<ReferenceDataMatch> Matches { get; set; } = [];

    public string SourceCodeColumn { get; set; } = string.Empty;

    public string SourceDescriptionColumn { get; set; } = string.Empty;

    public string TargetCodeColumn { get; set; } = string.Empty;

    public string TargetDescriptionColumn { get; set; } = string.Empty;

}