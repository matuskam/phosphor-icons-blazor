// AUTO-GENERATED FILE. Do not edit by hand.
// Regenerate with: dotnet run --project tools/PhosphorIcons.Generator
using PhosphorIcons.Blazor;

namespace PhosphorIcons;

/// <summary>Phosphor <c>cell-signal-none</c> icon.</summary>
public sealed class CellSignalNone : PhosphorIconBase
{
    /// <summary>Static icon definition.</summary>
    public static IconDefinition Definition { get; } = new(
        Name: "cell-signal-none",
        ViewBox: "0 0 256 256",
        Paths: new IconPaths(
            Thin: """<path d="M44,192v8a4,4,0,0,1-8,0v-8a4,4,0,0,1,8,0Z"/>""",
            Light: """<path d="M46,192v8a6,6,0,0,1-12,0v-8a6,6,0,0,1,12,0Z"/>""",
            Regular: """<path d="M48,192v8a8,8,0,0,1-16,0v-8a8,8,0,0,1,16,0Z"/>""",
            Bold: """<path d="M52,192v8a12,12,0,0,1-24,0v-8a12,12,0,0,1,24,0Z"/>""",
            Fill: """<path d="M198.12,25.23a16,16,0,0,0-17.44,3.46l-160,160A16,16,0,0,0,32,216H192a16,16,0,0,0,16-16V40A15.94,15.94,0,0,0,198.12,25.23ZM192,200H32L192,40Z"/>""",
            Duotone: new IconDuotone(
                Background: "",
                Foreground: """<path d="M198.12,25.23a16,16,0,0,0-17.43,3.47l-160,160A16,16,0,0,0,32,216H192a16,16,0,0,0,16-16V40A16,16,0,0,0,198.12,25.23ZM192,200H32L192,40Z"/>"""
            )
        )
    );

    /// <inheritdoc/>
    protected override IconDefinition Icon => Definition;
}
