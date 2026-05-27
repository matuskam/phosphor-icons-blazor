namespace PhosphorIcons.Blazor;

/// <summary>
/// Renders a hidden SVG sprite of symbol definitions for sprite mode. No-ops when
/// <see cref="PhosphorIconsOptions.UseSprite"/> is <c>false</c>. Only icons registered
/// via <see cref="PhosphorIconsOptions.Definitions"/> appear in the sprite.
/// </summary>
public sealed class PhosphorIconSprite : ComponentBase
{
    [Inject] private IIconRegistry Registry { get; set; } = default!;
    [Inject] private PhosphorIconsRuntimeOptions Options { get; set; } = default!;

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Options.UseSprite)
        {
            return;
        }

        var prefix = Options.SpriteIdPrefix;
        var weights = _weightValues;

        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "style", "display: none;");
        builder.AddAttribute(3, "aria-hidden", "true");

        // Constant sequence numbers per source line. A runtime counter would defeat
        // Blazor's diff prefix-matching and can produce incorrect diffs.
        foreach (var def in Registry.All())
        {
            foreach (var weight in weights)
            {
                var weightSlug = WeightSlug(weight);
                builder.OpenElement(4, "symbol");
                builder.AddAttribute(5, "id", $"{prefix}{def.Name}-{weightSlug}");
                builder.AddAttribute(6, "viewBox", def.ViewBox);
                builder.AddMarkupContent(7, def.Paths.For(weight));
                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    // Hoisted to avoid Enum.GetValues allocation on the sprite render loop.
    private static readonly IconWeight[] _weightValues = Enum.GetValues<IconWeight>();

    // Cached lowercase slug per weight; avoids ToString allocations on the hot loop.
    private static readonly string[] _weightSlugs = new string[_weightValues.Length];

    static PhosphorIconSprite()
    {
        foreach (var w in _weightValues)
        {
            _weightSlugs[(int)w] = w.ToString().ToLowerInvariant();
        }
    }

    private static string WeightSlug(IconWeight weight) => _weightSlugs[(int)weight];
}
