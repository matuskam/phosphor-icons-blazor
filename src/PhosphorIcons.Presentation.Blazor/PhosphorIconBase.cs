using System.Text.RegularExpressions;

namespace PhosphorIcons.Blazor;

/// <summary>
/// Base class for per-icon Phosphor components. Subclasses override <see cref="Icon"/>
/// to supply their <see cref="IconDefinition"/>. Parameter resolution: explicit value,
/// then cascading <see cref="IconContext"/>, then hard-coded default.
/// </summary>
public abstract class PhosphorIconBase : ComponentBase
{
    /// <summary>Weight (stroke or fill variant). Defaults to <see cref="IconWeight.Regular"/>.</summary>
    [Parameter] public IconWeight? Weight { get; set; }

    /// <summary>Width and height as a CSS length. Defaults to <c>"1em"</c>.</summary>
    [Parameter] public string? Size { get; set; }

    /// <summary>SVG <c>fill</c> color. Defaults to <c>"currentColor"</c>.</summary>
    [Parameter] public string? Color { get; set; }

    /// <summary>Flip horizontally.</summary>
    [Parameter] public bool? Mirrored { get; set; }

    /// <summary>Accessible name. When non-<c>null</c>, a child <c>title</c> element is emitted inside the SVG.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Pass-through HTML attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Cascading defaults from an ancestor <see cref="IconContext"/>.</summary>
    [CascadingParameter] protected IconContextValue? Context { get; set; }

    /// <summary>The icon to render.</summary>
    protected abstract IconDefinition Icon { get; }

    [Inject] private IIconRenderer Renderer { get; set; } = default!;

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var weight = Weight ?? Context?.Weight ?? IconWeight.Regular;
        var size = Size ?? Context?.Size ?? "1em";
        var color = Color ?? Context?.Color ?? "currentColor";
        var mirrored = Mirrored ?? Context?.Mirrored ?? false;

        var attributes = MergeAttributes(Context?.AdditionalAttributes, AdditionalAttributes, mirrored);

        var request = new IconRenderRequest(Icon, weight);

        builder.OpenElement(0, "svg");

        // Splat first so explicit attributes below win on key collision.
        if (attributes is not null)
        {
            builder.AddMultipleAttributes(1, attributes);
        }

        builder.AddAttribute(2, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(3, "viewBox", Icon.ViewBox);
        builder.AddAttribute(4, "width", size);
        builder.AddAttribute(5, "height", size);
        builder.AddAttribute(6, "fill", color);

        if (!string.IsNullOrEmpty(Title))
        {
            builder.OpenElement(7, "title");
            builder.AddContent(8, Title);
            builder.CloseElement();
        }

        builder.AddMarkupContent(9, Renderer.Render(request));

        builder.CloseElement();
    }

    private static IReadOnlyDictionary<string, object>? MergeAttributes(
        IReadOnlyDictionary<string, object>? context,
        IReadOnlyDictionary<string, object>? own,
        bool mirrored)
    {
        var hasContext = context is not null && context.Count > 0;
        var hasOwn = own is not null && own.Count > 0;

        if (!hasContext && !hasOwn && !mirrored)
        {
            return null;
        }

        var merged = new Dictionary<string, object>();
        if (hasContext)
        {
            foreach (var kvp in context!) merged[kvp.Key] = kvp.Value;
        }
        if (hasOwn)
        {
            foreach (var kvp in own!) merged[kvp.Key] = kvp.Value;
        }

        if (mirrored)
        {
            var existingStyle = merged.TryGetValue("style", out var existing)
                ? existing?.ToString()
                : null;
            merged["style"] = ComposeMirroredStyle(existingStyle);
        }

        return merged;
    }

    /// <summary>
    /// Compose <c>scaleX(-1)</c> into an existing <c>style</c> string. The mirror is appended
    /// as the rightmost (innermost) transform so consumer-supplied transforms wrap it.
    /// </summary>
    private static string ComposeMirroredStyle(string? existingStyle)
    {
        const string MirrorTransform = "scaleX(-1)";

        if (string.IsNullOrEmpty(existingStyle))
        {
            return $"transform: {MirrorTransform};";
        }

        // Splice scaleX(-1) at the end of any existing transform declaration's value.
        // CSS applies the rightmost transform first, so rightmost means innermost.
        var match = Regex.Match(
            existingStyle,
            @"(transform\s*:\s*)([^;]*?)(\s*;|\s*$)",
            RegexOptions.IgnoreCase);

        if (match.Success)
        {
            var keyword = match.Groups[1].Value;
            var existingValue = match.Groups[2].Value.Trim();
            var terminator = match.Groups[3].Value;
            var combined = string.IsNullOrEmpty(existingValue)
                ? MirrorTransform
                : $"{existingValue} {MirrorTransform}";
            return existingStyle.Substring(0, match.Index)
                + $"{keyword}{combined}{terminator}"
                + existingStyle.Substring(match.Index + match.Length);
        }

        return $"transform: {MirrorTransform}; {existingStyle}";
    }
}
