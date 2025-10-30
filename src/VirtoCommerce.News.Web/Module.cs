using System;
using System.IO;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.News.Core;
using VirtoCommerce.News.Core.Events;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.Core.Services;
using VirtoCommerce.News.Data.ExportImport;
using VirtoCommerce.News.Data.Handlers;
using VirtoCommerce.News.Data.MySql;
using VirtoCommerce.News.Data.PostgreSql;
using VirtoCommerce.News.Data.Repositories;
using VirtoCommerce.News.Data.Search.Indexed;
using VirtoCommerce.News.Data.Services;
using VirtoCommerce.News.Data.SqlServer;
using VirtoCommerce.News.Data.Validation;
using VirtoCommerce.News.ExperienceApi;
using VirtoCommerce.News.ExperienceApi.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.MySql.Extensions;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;
using VirtoCommerce.Platform.Data.SqlServer.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.Xapi.Core.Extensions;

namespace VirtoCommerce.News.Web;

public class Module : IModule, IExportSupport, IImportSupport, IHasConfiguration
{
    private IApplicationBuilder _appBuilder;

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
        serviceCollection.AddTransient<INewsArticleSeoService, NewsArticleSeoService>();

        serviceCollection.AddTransient<AbstractValidator<NewsArticle>, NewsArticleValidator>();
        serviceCollection.AddTransient<AbstractValidator<NewsArticleLocalizedContent>, NewsArticleLocalizedContentValidator>();

        // Indexing
        serviceCollection.AddTransient<IndexNewsChangedEventHandler>();

        // Для NewsDocumentBuilder
        serviceCollection.AddSingleton<IIndexDocumentBuilder, NewsDocumentBuilder>();
        serviceCollection.AddSingleton<IIndexSchemaBuilder, NewsDocumentBuilder>();
        serviceCollection.AddSingleton<ISearchRequestBuilder, NewsSearchRequestBuilder>();

        serviceCollection.AddSingleton<IIndexDocumentChangesProvider, NewsChangesProvider>();
        serviceCollection.AddScoped<INewsArticleIndexedSearchService, NewsArticleIndexedSearchService>(); // Изменено на Scoped

        serviceCollection.AddSingleton(provider => new IndexDocumentConfiguration
        {
            DocumentType = ModuleConstants.NewsIndexDocumentType,
            DocumentSource = new IndexDocumentSource
            {
                ChangesProvider = provider.GetService<IIndexDocumentChangesProvider>(),
                DocumentBuilder = provider.GetService<IIndexDocumentBuilder>(),
            },
        });

        serviceCollection.AddTransient<NewsArticlesExportImport>();

        // GraphQL
        serviceCollection.AddExperienceApi();
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        _appBuilder = appBuilder;

        var serviceProvider = appBuilder.ApplicationServices;

        // Register settings
        appBuilder.RegisterEventHandler<NewsChangedEvent, IndexNewsChangedEventHandler>();

        var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);
        settingsRegistrar.RegisterSettingsForType(ModuleConstants.Settings.AllSettings, nameof(Store));

        // Indexing
        var searchRequestBuilderRegistrar = appBuilder.ApplicationServices.GetService<ISearchRequestBuilderRegistrar>();
        searchRequestBuilderRegistrar.Register(ModuleConstants.NewsIndexDocumentType, appBuilder.ApplicationServices.GetService<ISearchRequestBuilder>); // Изменено на получение по интерфейсу

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

    public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        await _appBuilder.ApplicationServices.GetRequiredService<NewsArticlesExportImport>().DoExportAsync(outStream, progressCallback, cancellationToken);
    }

    public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        await _appBuilder.ApplicationServices.GetRequiredService<NewsArticlesExportImport>().DoImportAsync(inputStream, progressCallback, cancellationToken);
    }
}
