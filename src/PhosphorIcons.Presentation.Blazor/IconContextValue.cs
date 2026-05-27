namespace PhosphorIcons.Blazor;

/// <summary>
/// Snapshot of icon defaults cascaded by an <see cref="IconContext"/>. A <c>null</c>
/// field means no default is supplied at this layer.
/// </summary>
public sealed record IconContextValue
{
    /// <summary>Default weight for descendants that do not set their own.</summary>
    public IconWeight? Weight { get; init; }

    /// <summary>Default size for descendants that do not set their own.</summary>
    public string? Size { get; init; }

    /// <summary>Default color for descendants that do not set their own.</summary>
    public string? Color { get; init; }

    /// <summary>Default mirrored flag for descendants that do not set their own.</summary>
    public bool? Mirrored { get; init; }

    /// <summary>
    /// Default HTML attributes for descendants. Per-icon attributes merge on a per-key basis
    /// with icon-level values winning. The <c>style</c> attribute does not deep-merge.
    /// </summary>
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; init; }
}
