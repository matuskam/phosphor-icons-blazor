// AUTO-GENERATED FILE. Do not edit by hand.
// Regenerate with: dotnet run --project tools/PhosphorIcons.Generator
using PhosphorIcons.Blazor;

namespace PhosphorIcons;

/// <summary>Phosphor <c>dot</c> icon.</summary>
public sealed class Dot : PhosphorIconBase
{
    /// <summary>Static icon definition.</summary>
    public static IconDefinition Definition { get; } = new(
        Name: "dot",
        ViewBox: "0 0 256 256",
        Paths: new IconPaths(
            Thin: """<path d="M136,128a8,8,0,1,1-8-8A8,8,0,0,1,136,128Z"/>""",
            Light: """<path d="M138,128a10,10,0,1,1-10-10A10,10,0,0,1,138,128Z"/>""",
            Regular: """<path d="M140,128a12,12,0,1,1-12-12A12,12,0,0,1,140,128Z"/>""",
            Bold: """<path d="M144,128a16,16,0,1,1-16-16A16,16,0,0,1,144,128Z"/>""",
            Fill: """<path d="M128,80a48,48,0,1,0,48,48A48,48,0,0,0,128,80Zm0,60a12,12,0,1,1,12-12A12,12,0,0,1,128,140Z"/>""",
            Duotone: new IconDuotone(
                Background: """<path d="M176,128a48,48,0,1,1-48-48A48,48,0,0,1,176,128Z" opacity="0.2"/>""",
                Foreground: """<path d="M140,128a12,12,0,1,1-12-12A12,12,0,0,1,140,128Z"/>"""
            )
        )
    );

    /// <inheritdoc/>
    protected override IconDefinition Icon => Definition;
}
