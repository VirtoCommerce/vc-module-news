using GraphQL.MicrosoftDI;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.News.ExperienceApi.Services;
using VirtoCommerce.Xapi.Core.Extensions;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.ExperienceApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExperienceApi(this IServiceCollection serviceCollection)
    {
        _ = new GraphQLBuilder(serviceCollection, builder =>
        {
            builder.AddSchema(serviceCollection, typeof(XapiAssemblyMarker));
        });

        serviceCollection.AddSingleton<ScopedSchemaFactory<XapiAssemblyMarker>>();
        serviceCollection.AddSingleton<INewsArticleLocalizationService, NewsArticleLocalizationService>();
        serviceCollection.AddSingleton<INewsArticleSeoService, NewsArticleSeoService>();
        serviceCollection.AddSingleton<INewsArticleUserGroupsService, NewsArticleUserGroupsService>();

        return serviceCollection;
    }
}
