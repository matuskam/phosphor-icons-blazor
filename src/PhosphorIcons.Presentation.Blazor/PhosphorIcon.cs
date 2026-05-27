namespace PhosphorIcons.Blazor;

/// <summary>
/// Dynamic icon component. Resolves an <see cref="IconDefinition"/> from the
/// <see cref="IIconRegistry"/> by name at runtime. Throws on a null, empty, or unregistered name.
/// </summary>
public sealed class PhosphorIcon : PhosphorIconBase
{
    /// <summary>
    /// Canonical Phosphor icon name (lowercase kebab-case). Required. Throws
    /// <see cref="InvalidOperationException"/> when null, empty, or unregistered.
    /// </summary>
    [Parameter] public string? Name { get; set; }

    [Inject] private IIconRegistry Registry { get; set; } = default!;

    private IconDefinition? _resolved;

    /// <inheritdoc/>
    protected override IconDefinition Icon => _resolved!;

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (string.IsNullOrEmpty(Name))
        {
            throw new InvalidOperationException(
                "PhosphorIcon.Name is required. Set it to a canonical Phosphor icon name " +
                "(lowercase, kebab-case, for example \"house\" or \"user-circle\"). " +
                "If the name is bound to data that may briefly be null or empty, guard the " +
                "call site with @if (!string.IsNullOrEmpty(name)) { <PhosphorIcon Name=\"@name\" /> }.");
        }

        _resolved = Registry.Get(Name);

        if (_resolved is null)
        {
            throw new InvalidOperationException(
                $"No icon registered with name '{Name}'. " +
                "Register the icon definition via AddPhosphorIconsAll() or PhosphorIconsOptions.Definitions, " +
                "and verify the name matches the canonical Phosphor identifier (lowercase, kebab-case).");
        }
    }
}
