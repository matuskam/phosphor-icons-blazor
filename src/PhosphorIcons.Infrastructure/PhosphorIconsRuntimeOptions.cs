namespace PhosphorIcons.Blazor;

/// <summary>
/// Immutable runtime snapshot of <see cref="PhosphorIconsOptions"/> values
/// consumed during rendering. Registered as a singleton by
/// <c>AddPhosphorIcons</c> and captured at registration time. The mutable
/// <see cref="PhosphorIconsOptions"/> is not registered, so post-configure
/// mutation cannot desync the sprite component from the renderer.
/// </summary>
public sealed class PhosphorIconsRuntimeOptions
{
    /// <param name="useSprite">
    /// True when <see cref="SpriteSvgRenderer"/> is the active <see cref="IIconRenderer"/>.
    /// </param>
    /// <param name="spriteIdPrefix">
    /// Prefix applied to every sprite symbol id. Must match the value passed
    /// to the active <see cref="SpriteSvgRenderer"/> so symbol ids and
    /// <c>use</c> hrefs agree.
    /// </param>
    public PhosphorIconsRuntimeOptions(bool useSprite, string spriteIdPrefix)
    {
        if (spriteIdPrefix is null) throw new ArgumentNullException(nameof(spriteIdPrefix));
        UseSprite = useSprite;
        SpriteIdPrefix = spriteIdPrefix;
    }

    /// <summary>
    /// True when <see cref="SpriteSvgRenderer"/> is the active
    /// <see cref="IIconRenderer"/>. <c>PhosphorIconSprite</c> uses this
    /// to short-circuit rendering when sprite mode is off.
    /// </summary>
    public bool UseSprite { get; }

    /// <summary>
    /// Prefix applied to every sprite symbol id. Symbol ids emitted by
    /// <c>PhosphorIconSprite</c> follow the pattern
    /// <c>{SpriteIdPrefix}{iconName}-{weight}</c>.
    /// </summary>
    public string SpriteIdPrefix { get; }
}
