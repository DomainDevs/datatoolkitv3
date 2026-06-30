namespace DataToolkit.MigrationBuilder.Models;

public sealed class GenerateSqlRequest
{
    public List<string> WorkFiles { get; set; } = [];
    public bool IsHomologation { get; set; }

}
