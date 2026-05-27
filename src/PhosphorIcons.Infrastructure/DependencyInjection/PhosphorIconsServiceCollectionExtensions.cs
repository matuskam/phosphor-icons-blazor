using PhosphorIcons.Blazor;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// DI registration helpers for PhosphorIcons.Blazor.
/// </summary>
public static class PhosphorIconsServiceCollectionExtensions
{
    /// <summary>
    /// Register the <see cref="IIconRenderer"/> and <see cref="IIconRegistry"/> services
    /// required by PhosphorIcons.Blazor. Populate <c>PhosphorIconsOptions.Definitions</c>
    /// in the configure callback to enable dynamic name lookup.
    /// </summary>
    public static IServiceCollection AddPhosphorIcons(
        this IServiceCollection services,
        Action<PhosphorIconsOptions>? configure = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        var options = new PhosphorIconsOptions();
        configure?.Invoke(options);

        var definitionsSnapshot = options.Definitions.ToList();
        var runtimeOptions = new PhosphorIconsRuntimeOptions(options.UseSprite, options.SpriteIdPrefix);

        services.AddSingleton(runtimeOptions);

        if (runtimeOptions.UseSprite)
        {
            services.AddSingleton<IIconRenderer>(_ => new SpriteSvgRenderer(runtimeOptions.SpriteIdPrefix));
        }
        else
        {
            services.AddSingleton<IIconRenderer, InlineSvgRenderer>();
        }

        services.AddSingleton<IIconRegistry>(_ => new EmbeddedIconRegistry(definitionsSnapshot));

        return services;
    }
}
