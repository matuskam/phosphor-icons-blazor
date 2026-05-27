namespace PhosphorIcons.Blazor;

/// <summary>
/// <see cref="IIconRenderer"/> for sprite mode. Emits a <c>use</c> element referencing a symbol
/// with ID <c>{prefix}{iconName}-{weightSlug}</c>.
/// </summary>
public sealed class SpriteSvgRenderer : IIconRenderer
{
    // Cached lowercase slug per weight; avoids per-render allocation.
    private static readonly string[] _weightSlugs;

    static SpriteSvgRenderer()
    {
        var values = Enum.GetValues(typeof(IconWeight));
        _weightSlugs = new string[values.Length];
        foreach (IconWeight w in values)
        {
            _weightSlugs[(int)w] = w.ToString().ToLowerInvariant();
        }
    }

    private readonly string _prefix;

    /// <param name="idPrefix">
    /// Must match the <see cref="PhosphorIconsOptions.SpriteIdPrefix"/> used by the sprite component.
    /// </param>
    public SpriteSvgRenderer(string idPrefix = "ph-")
    {
        if (idPrefix is null) throw new ArgumentNullException(nameof(idPrefix));
        _prefix = idPrefix;
    }

    /// <inheritdoc/>
    public string Render(IconRenderRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return $@"<use href=""#{_prefix}{request.Definition.Name}-{_weightSlugs[(int)request.Weight]}""/>";
    }
}
