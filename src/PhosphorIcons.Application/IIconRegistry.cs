namespace PhosphorIcons.Blazor;

/// <summary>
/// Resolves an icon's canonical name to its <see cref="IconDefinition"/>.
/// </summary>
public interface IIconRegistry
{
    /// <summary>
    /// Return the definition for the icon whose canonical name is <paramref name="name"/>, or <c>null</c> if no such icon is registered.
    /// </summary>
    IconDefinition? Get(string name);

    /// <summary>
    /// Enumerate every registered icon definition. The order of enumeration is implementation-defined.
    /// </summary>
    IReadOnlyCollection<IconDefinition> All();
}
