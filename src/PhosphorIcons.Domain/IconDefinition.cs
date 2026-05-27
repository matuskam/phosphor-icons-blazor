namespace PhosphorIcons.Blazor;

/// <summary>
/// The full definition of a single icon: canonical name, SVG viewBox, and inner SVG markup for each weight.
/// </summary>
/// <param name="Name">Canonical lowercase-kebab-case name matching the upstream Phosphor identifier.</param>
/// <param name="ViewBox">The SVG viewBox attribute value.</param>
/// <param name="Paths">Inner SVG markup for each of the six weights.</param>
public sealed record IconDefinition(string Name, string ViewBox, IconPaths Paths);
