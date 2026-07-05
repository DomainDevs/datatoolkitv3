using DataToolkit.MigrationBuilder.Models.Medatata2;

namespace DataToolkit.MigrationBuilder.Services.Metadata2;

/// <summary>
/// Motor encargado de combinar y comparar metadata entre origen y destino.
/// NOTA: Esqueleto inicial para evolución.
/// </summary>
public sealed class MetadataMergeEngine
{
    public DatabaseMetadata Merge(
        DatabaseMetadata source,
        DatabaseMetadata target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        // TODO:
        // - Comparar tablas
        // - Comparar columnas
        // - Detectar tipos incompatibles
        // - Detectar PK/FK
        // - Marcar columnas nuevas
        // - Preparar modelo para DDL
        // - Preparar modelo para WorkFiles
        // - Preparar modelo para Homologación

        return target;
    }
}