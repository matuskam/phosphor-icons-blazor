namespace PhosphorIcons.Blazor;

/// <summary>
/// Default <see cref="IIconRegistry"/>. In-memory dictionary populated at construction.
/// </summary>
public sealed class EmbeddedIconRegistry : IIconRegistry
{
    private readonly Dictionary<string, IconDefinition> _byName;

    /// <summary>
    /// Build a registry from the given definitions. Names must be unique.
    /// </summary>
    public EmbeddedIconRegistry(IEnumerable<IconDefinition> definitions)
    {
        if (definitions is null) throw new ArgumentNullException(nameof(definitions));

        _byName = definitions.ToDictionary(d => d.Name, StringComparer.Ordinal);
    }

    /// <inheritdoc/>
    public IconDefinition? Get(string name)
    {
        if (name is null) throw new ArgumentNullException(nameof(name));
        return _byName.TryGetValue(name, out var definition) ? definition : null;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<IconDefinition> All() => _byName.Values;
}
