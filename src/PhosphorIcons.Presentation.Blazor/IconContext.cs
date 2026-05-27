namespace PhosphorIcons.Blazor;

/// <summary>
/// Cascades default icon parameters to a subtree. Nested contexts inherit outer defaults
/// and override only the parameters they set.
/// </summary>
public class IconContext : ComponentBase
{
    /// <summary>Default weight for descendants.</summary>
    [Parameter] public IconWeight? Weight { get; set; }

    /// <summary>Default size for descendants.</summary>
    [Parameter] public string? Size { get; set; }

    /// <summary>Default color for descendants.</summary>
    [Parameter] public string? Color { get; set; }

    /// <summary>Default mirrored flag for descendants.</summary>
    [Parameter] public bool? Mirrored { get; set; }

    /// <summary>Default HTML attributes for descendants.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Subtree that receives the cascading defaults.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Parent context when this <see cref="IconContext"/> is nested.</summary>
    [CascadingParameter] private IconContextValue? Parent { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var merged = new IconContextValue
        {
            Weight = Weight ?? Parent?.Weight,
            Size = Size ?? Parent?.Size,
            Color = Color ?? Parent?.Color,
            Mirrored = Mirrored ?? Parent?.Mirrored,
            AdditionalAttributes = MergeAttributes(Parent?.AdditionalAttributes, AdditionalAttributes)
        };

        builder.OpenComponent<CascadingValue<IconContextValue>>(0);
        builder.AddAttribute(1, nameof(CascadingValue<IconContextValue>.Value), merged);
        builder.AddAttribute(2, nameof(CascadingValue<IconContextValue>.ChildContent), ChildContent);
        builder.CloseComponent();
    }

    private static IReadOnlyDictionary<string, object>? MergeAttributes(
        IReadOnlyDictionary<string, object>? parent,
        IReadOnlyDictionary<string, object>? own)
    {
        if (parent is null) return own;
        if (own is null) return parent;

        var merged = new Dictionary<string, object>(parent);
        foreach (var kvp in own)
        {
            merged[kvp.Key] = kvp.Value;
        }
        return merged;
    }
}
