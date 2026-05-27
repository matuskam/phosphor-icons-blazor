namespace PhosphorIcons.Blazor;

/// <summary>
/// Default <see cref="IIconRenderer"/>. Returns the inner SVG markup for the requested weight.
/// </summary>
public sealed class InlineSvgRenderer : IIconRenderer
{
    /// <inheritdoc/>
    public string Render(IconRenderRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return request.Definition.Paths.For(request.Weight);
    }
}
