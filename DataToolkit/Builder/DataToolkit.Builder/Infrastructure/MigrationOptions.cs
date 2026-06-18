namespace DataToolkit.Builder.Configuration;

public sealed class MigrationOptions
{
    public const string SectionName = "Migration";

    public string WorkFilePath { get; set; } = "";

    public string SqlOutputPath { get; set; } = "";

    public string DefaultSchema { get; set; } = "dbo";
}