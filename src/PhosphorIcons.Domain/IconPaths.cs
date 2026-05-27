namespace PhosphorIcons.Blazor;

/// <summary>
/// The inner SVG markup for each of an icon's six weights. The renderer supplies the outer svg wrapper.
/// </summary>
public sealed record IconPaths(
    string Thin,
    string Light,
    string Regular,
    string Bold,
    string Fill,
    IconDuotone Duotone)
{
    // Benign race: concurrent writes produce an equal string and reference-type field writes are atomic in .NET.
    private string? _duotoneCombined;

    /// <summary>
    /// Return the inner SVG markup for a single weight. Duotone returns background concatenated with foreground and is cached after first call.
    /// </summary>
    public string For(IconWeight weight) => weight switch
    {
        IconWeight.Thin => Thin,
        IconWeight.Light => Light,
        IconWeight.Regular => Regular,
        IconWeight.Bold => Bold,
        IconWeight.Fill => Fill,
        IconWeight.Duotone => _duotoneCombined ??= Duotone.Background + Duotone.Foreground,
        _ => throw new ArgumentOutOfRangeException(nameof(weight), weight, $"Unknown IconWeight value: {weight}.")
    };
}
