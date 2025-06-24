using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.News.Core;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.Data.MySql;
using VirtoCommerce.News.Data.PostgreSql;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.News.Data.Services;
using VirtoCommerce.News.Data.SqlServer;
using VirtoCommerce.News.ExperienceApi;
using VirtoCommerce.News.ExperienceApi.Authorization;
using VirtoCommerce.News.ExperienceApi.Extensions;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.MySql.Extensions;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;
using VirtoCommerce.Platform.Data.SqlServer.Extensions;
using VirtoCommerce.Xapi.Core.Extensions;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.News.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<NewsDbContext>(options =>
        {
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
            var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

            switch (databaseProvider)
            {
                case "MySql":
                    options.UseMySqlDatabase(connectionString, typeof(MySqlDataAssemblyMarker), Configuration);
                    break;
                case "PostgreSql":
                    options.UsePostgreSqlDatabase(connectionString, typeof(PostgreSqlDataAssemblyMarker), Configuration);
                    break;
                default:
                    options.UseSqlServerDatabase(connectionString, typeof(SqlServerDataAssemblyMarker), Configuration);
                    break;
            }
        });

        serviceCollection.AddTransient<INewsArticleRepository, NewsArticleRepository>();
        serviceCollection.AddTransient<Func<INewsArticleRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<INewsArticleRepository>());

        serviceCollection.AddTransient<INewsArticleService, NewsArticleService>();
        serviceCollection.AddTransient<INewsArticleSearchService, NewsArticleSearchService>();

        serviceCollection.AddSingleton<IAuthorizationHandler, NewsArticleContentAuthorizationHandler>();
        serviceCollection.AddSingleton<ScopedSchemaFactory<XapiAssemblyMarker>>();

        // GraphQL
        serviceCollection.AddExperienceApi();
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var serviceProvider = appBuilder.ApplicationServices;

        // Register settings
        var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

        // Register permissions
        var permissionsRegistrar = serviceProvider.GetRequiredService<IPermissionsRegistrar>();
        permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "News", ModuleConstants.Security.Permissions.AllPermissions);

        // Register partial GraphQL schema
        appBuilder.UseScopedSchema<XapiAssemblyMarker>("news");

        // Apply migrations
        using var serviceScope = serviceProvider.CreateScope();
        using var dbContext = serviceScope.ServiceProvider.GetRequiredService<NewsDbContext>();
        dbContext.Database.Migrate();
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
