// PhosphorIcons.Generator
//
// Reads vendored Phosphor SVGs from /assets/phosphor/icons/ and emits one .cs file
// per icon under /src/PhosphorIcons.Presentation.Blazor/Icons/, plus a bulk
// registration extension AddPhosphorIconsAll() at
// /src/PhosphorIcons.Presentation.Blazor/DependencyInjection/.
//
// Modes:
//   default : generate from vendored SVGs
//   --sync  : fetch latest Phosphor SVGs from upstream, then generate
//
// Run:
//   dotnet run --project tools/PhosphorIcons.Generator
//   dotnet run --project tools/PhosphorIcons.Generator -- --sync

using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

const string PhosphorRawBaseUrl = "https://raw.githubusercontent.com/phosphor-icons/core";
const string PhosphorApiBaseUrl = "https://api.github.com/repos/phosphor-icons/core";
const int MaxParallelFetches = 16;

// Pinned upstream ref. Override per-invocation with `--ref <tag-or-branch-or-sha>`.
const string DefaultPhosphorRef = "v2.0.8";

var repoRoot = FindRepoRoot();
var vendorDir = Path.Combine(repoRoot, "assets", "phosphor", "icons");
var versionFile = Path.Combine(repoRoot, "assets", "phosphor", "VERSION");
var iconsOutDir = Path.Combine(repoRoot, "src", "PhosphorIcons.Presentation.Blazor", "Icons");
var bulkOutDir = Path.Combine(repoRoot, "src", "PhosphorIcons.Presentation.Blazor", "DependencyInjection");
var bulkOutFile = Path.Combine(bulkOutDir, "PhosphorIconsAllServiceCollectionExtensions.cs");

RecoverInterruptedSwap(vendorDir);

var doSync = args.Contains("--sync");
var refIndex = Array.IndexOf(args, "--ref");
var phosphorRef = DefaultPhosphorRef;

if (refIndex >= 0)
{
    if (refIndex + 1 >= args.Length)
    {
        throw new ArgumentException("--ref requires a value (for example, --ref v2.0.8).");
    }
    var refValue = args[refIndex + 1];
    if (refValue.StartsWith("--", StringComparison.Ordinal))
    {
        throw new ArgumentException(
            $"--ref expects a tag, branch, or commit SHA; got another flag '{refValue}'. Did you forget the value?");
    }
    phosphorRef = refValue;

    if (!doSync)
    {
        Console.WriteLine("[warning] --ref has no effect without --sync. Add --sync to fetch from this ref.");
    }
}

if (doSync)
{
    Console.WriteLine($"[sync] Fetching Phosphor SVGs from upstream (ref: {phosphorRef}) ...");
    await SyncAsync(vendorDir, versionFile, phosphorRef);
    Console.WriteLine();
}

Console.WriteLine("[generate] Reading vendored SVGs ...");
var iconNames = DiscoverIcons(vendorDir);
Console.WriteLine($"[generate] Found {iconNames.Count} icons.");

Directory.CreateDirectory(iconsOutDir);
Directory.CreateDirectory(bulkOutDir);

// Clear stale generated files so removed-upstream icons don't linger.
foreach (var stale in Directory.EnumerateFiles(iconsOutDir, "*.cs"))
{
    File.Delete(stale);
}

var classNames = new List<string>(iconNames.Count);
var skipped = new List<(string Name, string Reason)>();

foreach (var name in iconNames)
{
    var className = ToClassName(name);
    try
    {
        var paths = ReadAllWeights(vendorDir, name);
        var src = RenderIconClass(name, className, paths);
        File.WriteAllText(Path.Combine(iconsOutDir, $"{className}.cs"), src);
        classNames.Add(className);
    }
    catch (Exception ex)
    {
        skipped.Add((name, ex.Message));
    }
}

classNames.Sort(StringComparer.Ordinal);
File.WriteAllText(bulkOutFile, RenderBulkRegistration(classNames));

Console.WriteLine($"[generate] Wrote {classNames.Count} icon classes + bulk registration.");
if (skipped.Count > 0)
{
    Console.WriteLine($"[generate] Skipped {skipped.Count} icons (missing or malformed weight files):");
    foreach (var (n, why) in skipped.Take(20))
    {
        Console.WriteLine($"  {n}: {why}");
    }
    if (skipped.Count > 20)
    {
        Console.WriteLine($"  ... and {skipped.Count - 20} more.");
    }
}


// ---------------- discovery & read ----------------

static string FindRepoRoot()
{
    // Try cwd first, then the .dll location, so the tool works from `dotnet run` and from a built .exe.
    foreach (var startDir in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
    {
        var dir = new DirectoryInfo(startDir);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "PhosphorIcons.Blazor.slnx")))
            {
                return dir.FullName;
            }
            dir = dir.Parent;
        }
    }
    throw new InvalidOperationException(
        "Could not locate repo root (PhosphorIcons.Blazor.slnx not found by walking upward from " +
        $"either {Directory.GetCurrentDirectory()} or {AppContext.BaseDirectory}).");
}

static List<string> DiscoverIcons(string vendorDir)
{
    var regularDir = Path.Combine(vendorDir, "regular");
    if (!Directory.Exists(regularDir))
    {
        throw new InvalidOperationException(
            $"Vendored regular weight not found at {regularDir}. " +
            "Run with --sync to fetch upstream, or vendor the SVGs manually first.");
    }

    return Directory.EnumerateFiles(regularDir, "*.svg")
        .Select(f => Path.GetFileNameWithoutExtension(f)!)
        .OrderBy(n => n, StringComparer.Ordinal)
        .ToList();
}

static IconPathsRaw ReadAllWeights(string vendorDir, string name)
{
    string Read(string weight, string filename)
    {
        var path = Path.Combine(vendorDir, weight, filename);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Missing {weight} weight file: {filename}", path);
        }
        return ExtractInner(File.ReadAllText(path));
    }

    var thin = Read("thin", $"{name}-thin.svg");
    var light = Read("light", $"{name}-light.svg");
    var regular = Read("regular", $"{name}.svg");
    var bold = Read("bold", $"{name}-bold.svg");
    var fill = Read("fill", $"{name}-fill.svg");
    var duotoneInner = Read("duotone", $"{name}-duotone.svg");
    var (bg, fg) = SplitDuotone(duotoneInner);

    return new IconPathsRaw(thin, light, regular, bold, fill, bg, fg);
}

static string ExtractInner(string svg)
{
    var m = Regex.Match(svg, @"<svg[^>]*>(.+?)</svg>", RegexOptions.Singleline);
    if (!m.Success)
    {
        throw new InvalidDataException("SVG file did not contain a parseable <svg>...</svg> wrapper.");
    }
    return m.Groups[1].Value.Trim();
}

static (string Background, string Foreground) SplitDuotone(string inner)
{
    // Match each top-level SVG element. Backreference plus negative lookahead so a child
    // element with a different name doesn't terminate the match prematurely.
    var elements = Regex.Matches(
        inner,
        @"<(\w+)\b[^>]*?(?:/>|>(?:(?!</\1\s*>).)*</\1\s*>)",
        RegexOptions.Singleline);

    var bgBuilder = new StringBuilder();
    var fgBuilder = new StringBuilder();
    foreach (Match m in elements)
    {
        // Classify by the opening tag only so attribute values inside child elements can't be misread.
        var openTagEnd = m.Value.IndexOf('>');
        var openingTag = openTagEnd > 0 ? m.Value.Substring(0, openTagEnd + 1) : m.Value;
        if (openingTag.Contains("opacity=\"0.2\""))
        {
            bgBuilder.Append(m.Value);
        }
        else
        {
            fgBuilder.Append(m.Value);
        }
    }

    var bg = bgBuilder.ToString();
    var fg = fgBuilder.ToString();

    // Empty Background is valid for "none" variants. Empty Foreground is not.
    if (string.IsNullOrEmpty(fg))
    {
        throw new InvalidDataException(
            "Duotone SVG did not contain a full-opacity foreground element.");
    }
    return (bg, fg);
}


// ---------------- name conversion ----------------

static string ToClassName(string kebab)
{
    var ti = CultureInfo.InvariantCulture.TextInfo;
    var pascal = string.Concat(kebab.Split('-').Select(part => ti.ToTitleCase(part)));
    // C# identifiers cannot start with a digit.
    if (pascal.Length > 0 && char.IsDigit(pascal[0]))
    {
        pascal = "_" + pascal;
    }
    return pascal;
}


// ---------------- codegen ----------------

static string RenderIconClass(string iconName, string className, IconPathsRaw paths)
{
    // Use 3-quote raw string for content with double quotes; use "" for empty.
    // Six consecutive quotes is not a valid empty raw string in C#.
    static string EmitLiteral(string content)
        => content.Length == 0 ? "\"\"" : $"\"\"\"{content}\"\"\"";

    // Outer raw-string uses 4-quote delimiters so the generated 3-quote raw-string literals inside are literal.
    return $$""""
// AUTO-GENERATED FILE. Do not edit by hand.
// Regenerate with: dotnet run --project tools/PhosphorIcons.Generator
using PhosphorIcons.Blazor;

namespace PhosphorIcons;

/// <summary>Phosphor <c>{{iconName}}</c> icon.</summary>
public sealed class {{className}} : PhosphorIconBase
{
    /// <summary>Static icon definition.</summary>
    public static IconDefinition Definition { get; } = new(
        Name: "{{iconName}}",
        ViewBox: "0 0 256 256",
        Paths: new IconPaths(
            Thin: {{EmitLiteral(paths.Thin)}},
            Light: {{EmitLiteral(paths.Light)}},
            Regular: {{EmitLiteral(paths.Regular)}},
            Bold: {{EmitLiteral(paths.Bold)}},
            Fill: {{EmitLiteral(paths.Fill)}},
            Duotone: new IconDuotone(
                Background: {{EmitLiteral(paths.DuotoneBackground)}},
                Foreground: {{EmitLiteral(paths.DuotoneForeground)}}
            )
        )
    );

    /// <inheritdoc/>
    protected override IconDefinition Icon => Definition;
}

"""";
}

static string RenderBulkRegistration(List<string> classNames)
{
    var sb = new StringBuilder();

    sb.AppendLine("// AUTO-GENERATED FILE. Do not edit by hand.");
    sb.AppendLine("// Regenerate with: dotnet run --project tools/PhosphorIcons.Generator");
    sb.AppendLine("using System;");
    sb.AppendLine("using PhosphorIcons.Blazor;");
    sb.AppendLine();
    sb.AppendLine("namespace Microsoft.Extensions.DependencyInjection;");
    sb.AppendLine();
    sb.AppendLine("/// <summary>Bulk-registration extension methods for PhosphorIcons.Blazor.</summary>");
    sb.AppendLine("public static class PhosphorIconsAllServiceCollectionExtensions");
    sb.AppendLine("{");
    sb.AppendLine("    /// <summary>Registers PhosphorIcons.Blazor services and seeds <c>Definitions</c> with every generated icon.</summary>");
    sb.AppendLine("    public static IServiceCollection AddPhosphorIconsAll(");
    sb.AppendLine("        this IServiceCollection services,");
    sb.AppendLine("        Action<PhosphorIconsOptions>? configure = null)");
    sb.AppendLine("    {");
    sb.AppendLine("        return services.AddPhosphorIcons(opts =>");
    sb.AppendLine("        {");
    foreach (var cn in classNames)
    {
        sb.AppendLine($"            opts.Definitions.Add(PhosphorIcons.{cn}.Definition);");
    }
    sb.AppendLine("            configure?.Invoke(opts);");
    sb.AppendLine("        });");
    sb.AppendLine("    }");
    sb.AppendLine("}");

    return sb.ToString();
}


// ---------------- recovery ----------------

// Recover from a hard-kill during the previous sync's rename-rename-delete
// swap. Safe to call on every invocation; no-ops when no leftover backup
// exists. Designed for NTFS, where Directory.Move is a single atomic rename.
// Behavior on other filesystems (FAT, exFAT, ReFS, ext4, APFS, ...) is not
// verified and may differ.
static void RecoverInterruptedSwap(string vendorDir)
{
    var backupDir = vendorDir + ".old";
    if (!Directory.Exists(backupDir)) return;

    if (Directory.Exists(vendorDir))
    {
        // Prior swap completed (step 2) but the cleanup delete (step 3) did not.
        // The current vendor is the new content; the backup is obsolete.
        Directory.Delete(backupDir, recursive: true);
        Console.WriteLine($"[recover] Removed leftover backup at {backupDir}.");
    }
    else
    {
        // Prior swap was interrupted between step 1 (move vendor -> backup) and
        // step 2 (move staging -> vendor). Restore the previous vendor tree.
        Directory.Move(backupDir, vendorDir);
        Console.WriteLine($"[recover] Restored vendor tree from {backupDir}.");
    }
}


// ---------------- sync ----------------

static async Task SyncAsync(string vendorDir, string versionFile, string phosphorRef)
{
    using var http = new HttpClient();
    http.DefaultRequestHeaders.UserAgent.ParseAdd("phosphor-icons-blazor-generator/0.1");

    // Resolve the ref to a concrete commit SHA. /commits/{ref} auto-dereferences annotated tags.
    var commitJson = await http.GetStringAsync($"{PhosphorApiBaseUrl}/commits/{phosphorRef}");
    var commitDoc = JsonDocument.Parse(commitJson);
    var sha = commitDoc.RootElement.GetProperty("sha").GetString()!;
    Console.WriteLine($"  ref {phosphorRef} resolved to commit {sha[..12]}");

    // Discover all icon names in one request via the Git Tree API.
    var treeJson = await http.GetStringAsync($"{PhosphorApiBaseUrl}/git/trees/{sha}?recursive=1");
    using var treeDoc = JsonDocument.Parse(treeJson);
    var entries = treeDoc.RootElement.GetProperty("tree").EnumerateArray();

    var iconNames = new List<string>();
    foreach (var entry in entries)
    {
        var path = entry.GetProperty("path").GetString();
        if (path is null) continue;
        if (path.StartsWith("assets/regular/", StringComparison.Ordinal) && path.EndsWith(".svg", StringComparison.Ordinal))
        {
            iconNames.Add(Path.GetFileNameWithoutExtension(path));
        }
    }
    iconNames.Sort(StringComparer.Ordinal);
    Console.WriteLine($"  discovered {iconNames.Count} icons");

    // Refuse to proceed if the API returned zero icons; otherwise we'd wipe the vendor tree.
    if (iconNames.Count == 0)
    {
        throw new InvalidOperationException(
            $"Discovered 0 icons at ref {phosphorRef}. The upstream repo layout may have changed, " +
            "or the ref may be invalid. The existing vendor tree is unchanged.");
    }

    // Stage downloads in a sibling dir; swap into place only on full success.
    var stagingDir = vendorDir + ".staging";
    if (Directory.Exists(stagingDir)) Directory.Delete(stagingDir, recursive: true);
    foreach (var w in new[] { "thin", "light", "regular", "bold", "fill", "duotone" })
    {
        Directory.CreateDirectory(Path.Combine(stagingDir, w));
    }

    var weightSuffixes = new (string Weight, string Suffix)[]
    {
        ("thin", "-thin"), ("light", "-light"), ("regular", ""),
        ("bold", "-bold"), ("fill", "-fill"), ("duotone", "-duotone"),
    };

    var totalFetches = iconNames.Count * weightSuffixes.Length;
    var fetchCount = 0;
    var failures = 0;
    using var sem = new SemaphoreSlim(MaxParallelFetches);
    var tasks = new List<Task>(totalFetches);

    foreach (var name in iconNames)
    {
        foreach (var (weight, suffix) in weightSuffixes)
        {
            tasks.Add(Task.Run(async () =>
            {
                await sem.WaitAsync();
                try
                {
                    var url = $"{PhosphorRawBaseUrl}/{sha}/assets/{weight}/{name}{suffix}.svg";
                    string svg;
                    try
                    {
                        svg = await http.GetStringAsync(url);
                    }
                    catch (HttpRequestException ex)
                    {
                        Interlocked.Increment(ref failures);
                        Console.WriteLine($"  FAIL {weight}/{name}{suffix}.svg -- {ex.Message}");
                        return;
                    }
                    await File.WriteAllTextAsync(Path.Combine(stagingDir, weight, $"{name}{suffix}.svg"), svg);
                    var n = Interlocked.Increment(ref fetchCount);
                    if (n % 500 == 0) Console.WriteLine($"  fetched {n}/{totalFetches}");
                }
                finally { sem.Release(); }
            }));
        }
    }
    await Task.WhenAll(tasks);

    // Abort partial syncs. Drop staging on any failure; the vendor tree stays intact.
    if (failures > 0)
    {
        try { Directory.Delete(stagingDir, recursive: true); } catch { /* best-effort cleanup */ }
        throw new InvalidOperationException(
            $"Sync aborted: {failures}/{totalFetches} fetches failed. The existing vendor tree is unchanged. " +
            "Common causes: network drop, GitHub rate limit (HTTP 403), or upstream restructure. " +
            "Inspect the per-file FAIL lines above and retry.");
    }

    // Vendor-tree swap via rename-rename-delete. At every observable moment at
    // least one complete copy of the tree exists on disk (vendor, vendor.old,
    // or both), so a hard-kill never leaves the repo without a usable tree;
    // RecoverInterruptedSwap reconciles any leftover backup on the next run.
    // Designed for NTFS, where Directory.Move is a single atomic rename.
    // Behavior on other filesystems (FAT, exFAT, ReFS, ext4, APFS, ...) is not
    // verified and may differ.
    var backupDir = vendorDir + ".old";
    if (Directory.Exists(backupDir)) Directory.Delete(backupDir, recursive: true);
    if (Directory.Exists(vendorDir)) Directory.Move(vendorDir, backupDir);
    Directory.Move(stagingDir, vendorDir);
    try { Directory.Delete(backupDir, recursive: true); } catch { /* leave for next-run recovery */ }

    // Write VERSION only after the successful swap.
    var versionContent =
        $"phosphor-icons/core @ {phosphorRef}{Environment.NewLine}" +
        $"Commit: {sha}{Environment.NewLine}" +
        $"Fetched: {DateTime.UtcNow:yyyy-MM-dd}{Environment.NewLine}" +
        $"Icons: {iconNames.Count}{Environment.NewLine}";
    Directory.CreateDirectory(Path.GetDirectoryName(versionFile)!);
    await File.WriteAllTextAsync(versionFile, versionContent);

    Console.WriteLine($"  vendored {fetchCount}/{totalFetches} SVGs successfully");
}


// ---------------- types ----------------

internal sealed record IconPathsRaw(
    string Thin,
    string Light,
    string Regular,
    string Bold,
    string Fill,
    string DuotoneBackground,
    string DuotoneForeground);
