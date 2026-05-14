using ERP.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Satisfactory.Save;

namespace ERP.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddErpInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CatalogueOptions>(configuration.GetSection("Catalogue:Satisfactory"));
        services.Configure<FactoryStateOptions>(configuration.GetSection("FactoryState:Satisfactory"));
        services.AddSingleton<UserCatalogueConfig>();
        services.AddSingleton<ICatalogProvider, DocsCatalogProvider>();
        // Manual node overrides — user-local JSON loaded once at startup and
        // mutated through the /factory/node-override API. Same singleton is
        // shared with the SaveFileReader so re-parses pick up new entries.
        services.AddSingleton<ManualNodeOverrides>(_ => ManualNodeOverrides.LoadOrCreate(
            ManualNodeOverridesPath.Resolve(configuration["FactoryState:Satisfactory:OverridesPath"])));
        services.AddSingleton<IFactoryStateProvider, SatisfactorySaveNetFactoryStateProvider>();
        return services;
    }
}
