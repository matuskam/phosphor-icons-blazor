namespace PhosphorIcons.Blazor;

/// <summary>
/// The two-layer SVG markup for a duotone icon. <see cref="Background"/> renders first, <see cref="Foreground"/> on top.
/// </summary>
/// <param name="Background">Lower-layer SVG markup. Upstream Phosphor bakes opacity="0.2" into this path.</param>
/// <param name="Foreground">Upper-layer SVG markup rendered at full opacity.</param>
public sealed record IconDuotone(string Background, string Foreground);
