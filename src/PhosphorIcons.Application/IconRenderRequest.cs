namespace PhosphorIcons.Blazor;

/// <summary>
/// Inputs to <see cref="IIconRenderer.Render"/>. Carries only what the renderer
/// needs to produce the inner SVG content for one icon at one weight. The
/// outer <c>svg</c> element and its attributes (size, color, mirror, title,
/// pass-through HTML attributes) are applied by the caller, not the renderer.
/// </summary>
/// <param name="Definition">The icon to render.</param>
/// <param name="Weight">The selected weight.</param>
public sealed record IconRenderRequest(
    IconDefinition Definition,
    IconWeight Weight);
