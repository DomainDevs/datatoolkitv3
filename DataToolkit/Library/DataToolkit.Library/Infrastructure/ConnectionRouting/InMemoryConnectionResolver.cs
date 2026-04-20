using DataToolkit.Library.Connections;
using System;
using System.Collections.Generic;

namespace DataToolkit.Library.Infrastructure.ConnectionRouting;

public class InMemoryConnectionResolver : IConnectionResolver
{
    private readonly Dictionary<string, (string connectionString, DatabaseProvider provider)> _config;

    public InMemoryConnectionResolver(Dictionary<string, (string connectionString, DatabaseProvider provider)> config)
    {
        if (config == null || config.Count == 0)
            throw new ArgumentException("Debe existir al menos una configuración.");

        _config = config;
    }

    public (string connectionString, DatabaseProvider provider) Resolve(string alias)
    {
        if (!_config.TryGetValue(alias, out var entry))
            throw new InvalidOperationException($"No existe el alias: {alias}");

        return entry;
    }
}