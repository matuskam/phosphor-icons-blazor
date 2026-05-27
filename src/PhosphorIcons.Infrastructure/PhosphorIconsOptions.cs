namespace PhosphorIcons.Blazor;

/// <summary>
/// Configuration for the PhosphorIcons.Blazor library.
/// </summary>
public sealed class PhosphorIconsOptions
{
    /// <summary>
    /// Icon definitions registered with the <see cref="IIconRegistry"/>. The dynamic
    /// <c>PhosphorIcon</c> component resolves names against this collection, and
    /// <c>PhosphorIconSprite</c> emits one symbol per definition and weight.
    /// </summary>
    public IList<IconDefinition> Definitions { get; } = new List<IconDefinition>();

    /// <summary>
    /// When <c>true</c>, <c>AddPhosphorIcons</c> registers <see cref="SpriteSvgRenderer"/>
    /// instead of <see cref="InlineSvgRenderer"/>. Sprite mode requires
    /// <c>PhosphorIconSprite</c> to be rendered once on the page.
    /// </summary>
    public bool UseSprite { get; set; }

    /// <summary>
    /// Prefix applied to every sprite symbol ID. Default <c>"ph-"</c>. Symbol IDs follow
    /// the pattern <c>{SpriteIdPrefix}{iconName}-{weight}</c>.
    /// </summary>
    public string SpriteIdPrefix { get; set; } = "ph-";
}
