// AUTO-GENERATED FILE. Do not edit by hand.
// Regenerate with: dotnet run --project tools/PhosphorIcons.Generator
using PhosphorIcons.Blazor;

namespace PhosphorIcons;

/// <summary>Phosphor <c>dot-outline</c> icon.</summary>
public sealed class DotOutline : PhosphorIconBase
{
    /// <summary>Static icon definition.</summary>
    public static IconDefinition Definition { get; } = new(
        Name: "dot-outline",
        ViewBox: "0 0 256 256",
        Paths: new IconPaths(
            Thin: """<path d="M128,100a28,28,0,1,0,28,28A28,28,0,0,0,128,100Zm0,48a20,20,0,1,1,20-20A20,20,0,0,1,128,148Z"/>""",
            Light: """<path d="M128,98a30,30,0,1,0,30,30A30,30,0,0,0,128,98Zm0,48a18,18,0,1,1,18-18A18,18,0,0,1,128,146Z"/>""",
            Regular: """<path d="M128,96a32,32,0,1,0,32,32A32,32,0,0,0,128,96Zm0,48a16,16,0,1,1,16-16A16,16,0,0,1,128,144Z"/>""",
            Bold: """<path d="M128,96a32,32,0,1,0,32,32A32,32,0,0,0,128,96Zm0,40a8,8,0,1,1,8-8A8,8,0,0,1,128,136Z"/>""",
            Fill: """<path d="M156,128a28,28,0,1,1-28-28A28,28,0,0,1,156,128Z"/>""",
            Duotone: new IconDuotone(
                Background: """<path d="M152,128a24,24,0,1,1-24-24A24,24,0,0,1,152,128Z" opacity="0.2"/>""",
                Foreground: """<path d="M128,96a32,32,0,1,0,32,32A32,32,0,0,0,128,96Zm0,48a16,16,0,1,1,16-16A16,16,0,0,1,128,144Z"/>"""
            )
        )
    );

    /// <inheritdoc/>
    protected override IconDefinition Icon => Definition;
}
