// AUTO-GENERATED FILE. Do not edit by hand.
// Regenerate with: dotnet run --project tools/PhosphorIcons.Generator
using PhosphorIcons.Blazor;

namespace PhosphorIcons;

/// <summary>Phosphor <c>arrow-down</c> icon.</summary>
public sealed class ArrowDown : PhosphorIconBase
{
    /// <summary>Static icon definition.</summary>
    public static IconDefinition Definition { get; } = new(
        Name: "arrow-down",
        ViewBox: "0 0 256 256",
        Paths: new IconPaths(
            Thin: """<path d="M202.83,146.83l-72,72a4,4,0,0,1-5.66,0l-72-72a4,4,0,0,1,5.66-5.66L124,206.34V40a4,4,0,0,1,8,0V206.34l65.17-65.17a4,4,0,0,1,5.66,5.66Z"/>""",
            Light: """<path d="M204.24,148.24l-72,72a6,6,0,0,1-8.48,0l-72-72a6,6,0,0,1,8.48-8.48L122,201.51V40a6,6,0,0,1,12,0V201.51l61.76-61.75a6,6,0,0,1,8.48,8.48Z"/>""",
            Regular: """<path d="M205.66,149.66l-72,72a8,8,0,0,1-11.32,0l-72-72a8,8,0,0,1,11.32-11.32L120,196.69V40a8,8,0,0,1,16,0V196.69l58.34-58.35a8,8,0,0,1,11.32,11.32Z"/>""",
            Bold: """<path d="M208.49,152.49l-72,72a12,12,0,0,1-17,0l-72-72a12,12,0,0,1,17-17L116,187V40a12,12,0,0,1,24,0V187l51.51-51.52a12,12,0,0,1,17,17Z"/>""",
            Fill: """<path d="M205.66,149.66l-72,72a8,8,0,0,1-11.32,0l-72-72A8,8,0,0,1,56,136h64V40a8,8,0,0,1,16,0v96h64a8,8,0,0,1,5.66,13.66Z"/>""",
            Duotone: new IconDuotone(
                Background: """<path d="M200,144l-72,72L56,144Z" opacity="0.2"/>""",
                Foreground: """<path d="M207.39,140.94A8,8,0,0,0,200,136H136V40a8,8,0,0,0-16,0v96H56a8,8,0,0,0-5.66,13.66l72,72a8,8,0,0,0,11.32,0l72-72A8,8,0,0,0,207.39,140.94ZM128,204.69,75.31,152H180.69Z"/>"""
            )
        )
    );

    /// <inheritdoc/>
    protected override IconDefinition Icon => Definition;
}
