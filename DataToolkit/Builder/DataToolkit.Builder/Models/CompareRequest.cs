using System.ComponentModel;

namespace DataToolkit.Builder.Models;

public sealed class CompareRequest
{
    /// <summary>
    /// Base de datos origen.
    /// </summary>
    public ConnectionRequest SourceConnectionString { get; set; }

    /// <summary> 
    /// Base de datos destino. 
    /// </summary>
    public ConnectionRequest TargetConnectionString { get; set; }

    /// <summary> 
    /// Esquema a comparar. 
    /// Null = todos los esquemas. 
    /// </summary>
    [DefaultValue("dbo")]
    public string? Schema { get; set; } 
    /// <summary>
    /// Tablas específicas a comparar. 
    /// Null o vacío = todas las tablas. 
    /// </summary>
    public List<string> Tables { get; set; } = [];
}