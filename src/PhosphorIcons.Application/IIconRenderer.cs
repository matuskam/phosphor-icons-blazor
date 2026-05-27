namespace PhosphorIcons.Blazor;

/// <summary>
/// Produces the inner SVG content for a single icon. Implementations return
/// markup that the caller embeds inside an outer <c>svg</c> element; the
/// returned string must not include an outer <c>svg</c> wrapper of its own.
/// Per-instance attributes (size, color, mirror transform, title element,
/// pass-through HTML attributes) are applied by the caller to that outer
/// <c>svg</c> and are intentionally not surfaced through this interface.
/// </summary>
public interface IIconRenderer
{
    /// <summary>
    /// Render the inner content for <paramref name="request"/>. The returned
    /// markup is inserted as-is inside the caller's <c>svg</c>; typical
    /// outputs are one or more <c>path</c> elements (inline mode) or a single
    /// <c>use</c> reference to a sprite symbol (sprite mode).
    /// </summary>
    string Render(IconRenderRequest request);
}
