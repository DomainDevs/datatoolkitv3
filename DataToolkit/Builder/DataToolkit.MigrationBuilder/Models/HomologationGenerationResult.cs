namespace DataToolkit.MigrationBuilder.Models;

public sealed class HomologationGenerationResult
{
    public string ScriptPath { get; set; }
        = string.Empty;

    public HomologationResult Result { get; set; }
        = new();
}
