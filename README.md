# phosphor-icons-blazor

Phosphor Icons for Blazor. 1,248 icons across 6 weights (thin, light, regular, bold, fill, duotone) as native Razor components. Pinned to upstream [phosphor-icons/core v2.0.8](https://github.com/phosphor-icons/core/releases/tag/v2.0.8).

## Requirements

- .NET 8.0 or .NET 10.0
- Any Blazor hosting model (Server, WebAssembly, Hybrid, Static SSR)

## Install

While the package is pre-publication, reference the project directly from a local clone:

```xml
<ProjectReference Include="..\..\phosphor-icons-blazor\src\PhosphorIcons.Presentation.Blazor\PhosphorIcons.Presentation.Blazor.csproj" />
```

## Quick start

In `Program.cs`:

```csharp
builder.Services.AddPhosphorIconsAll();
```

`AddPhosphorIconsAll()` registers every icon with the runtime registry so both per-icon and dynamic-by-name usage work. To register only what you need, see [Registration variants](#registration-variants).

In a `.razor` file:

```razor
@using PhosphorIcons.Blazor

@* Per-icon: compile-time names, IntelliSense *@
<PhosphorIcons.House Weight="IconWeight.Bold" Size="32" Color="red" />
<PhosphorIcons.User Mirrored="true" />

@* Dynamic: bind the name at runtime *@
@foreach (var name in iconNames)
{
    <PhosphorIcon Name="@name" Weight="IconWeight.Bold" />
}
```

## Why are per-icon tags fully qualified?

The `PhosphorIcons` namespace contains 1,248 type names including `File`, `Path`, `User`, and `Image`. Importing it unqualified (`@using PhosphorIcons`) would shadow `System.IO.File` and similar BCL types throughout the file. To avoid that, per-icon components are always written `<PhosphorIcons.House />` and the namespace is not imported.

The dynamic `<PhosphorIcon>` component lives in `PhosphorIcons.Blazor` and is unqualified because that namespace does not collide with anything in the BCL.

## Parameters

Every icon (per-icon and dynamic) accepts the same parameter set:

| Parameter | Type | Default | Notes |
|---|---|---|---|
| `Weight` | `IconWeight?` | `Regular` | `Thin`, `Light`, `Regular`, `Bold`, `Fill`, `Duotone` |
| `Size` | `string?` | `"1em"` | Any CSS length: `"24"`, `"24px"`, `"1em"`, `"100%"` |
| `Color` | `string?` | `"currentColor"` | Any CSS color. Maps to the SVG's `fill` attribute; with the default, icons inherit text color through `currentColor` |
| `Mirrored` | `bool?` | `false` | Flips horizontally via `transform: scaleX(-1)` |
| `Title` | `string?` | `null` | When set, emits a `<title>` inside the SVG for accessibility |
| splat | HTML attributes | none | `class`, `style`, `aria-*`, `data-*`, event handlers; all pass through to the outer `<svg>` |

## Cascading defaults

Wrap a subtree in `<IconContext>` to set defaults that descendants inherit. Per-icon explicit values override the context on a per-parameter basis.

```razor
<IconContext Weight="IconWeight.Bold" Color="#dc2626" Size="32">
    <PhosphorIcons.House />                        @* Bold, red, 32 *@
    <PhosphorIcons.User />                         @* Bold, red, 32 *@
    <PhosphorIcons.File Color="#2563eb" />         @* Bold, blue, 32 (color override wins) *@
</IconContext>
```

Nested contexts merge: an inner context inherits values from the outer for any parameter it does not set itself, and overrides only the parameters it explicitly sets.

```razor
<IconContext Weight="IconWeight.Bold" Color="red">
    <PhosphorIcons.House />              @* Bold, red *@
    <IconContext Color="blue">
        <PhosphorIcons.User />           @* Bold, blue (outer weight is inherited, inner color wins) *@
    </IconContext>
</IconContext>
```

## Accessibility

Set `Title` to give an icon a semantic name. The component emits an inline `<title>` element that screen readers announce as the icon's accessible name:

```razor
<PhosphorIcons.House Title="Open home page" />
```

For purely decorative icons, omit `Title` and pass `aria-hidden="true"` via the splat so assistive technology skips the element:

```razor
<PhosphorIcons.Star aria-hidden="true" />
```

## Sprite mode

Inline mode (the default) emits every icon as full SVG markup at the call site. Sprite mode emits a single hidden `<svg>` containing `<symbol>` definitions once per page, and rewrites each icon usage as a small `<use href="#...">` reference. Trades sprite payload size for thinner per-icon DOM; useful when the same page renders many copies of the same icons.

```csharp
// Program.cs
builder.Services.AddPhosphorIconsAll(opts => opts.UseSprite = true);
```

```razor
@* MainLayout.razor: once, near the top of the layout *@
<PhosphorIconSprite />
```

Per-icon and dynamic markup is identical between modes. Switching is purely a DI configuration choice.

## Registration variants

| Call | Registers | When to use |
|---|---|---|
| `AddPhosphorIconsAll()` | Every icon (1,248) | Default. Both per-icon and dynamic-by-name work. |
| `AddPhosphorIcons(opts => opts.Definitions.Add(...))` | Only icons you list | Smaller registry. Per-icon components still work (they hold their own definition). Dynamic `<PhosphorIcon Name="...">` only resolves names you added. |
| `AddPhosphorIcons()` | Nothing | Per-icon components only. Dynamic name lookup will throw. |