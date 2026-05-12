using ERP.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddErpInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CatalogueOptions>(configuration.GetSection("Catalogue:Satisfactory"));
        services.Configure<FactoryStateOptions>(configuration.GetSection("FactoryState:Satisfactory"));
        services.AddSingleton<UserCatalogueConfig>();
        services.AddSingleton<ICatalogProvider, DocsCatalogProvider>();
        services.AddSingleton<IFactoryStateProvider, SatisfactorySaveNetFactoryStateProvider>();
        return services;
    }
}
