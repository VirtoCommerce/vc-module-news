using System.Linq;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.News.Core.Models;
using VirtoCommerce.News.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.News.ExperienceApi.Schemas;

public class NewsArticleContentType : ExtendableGraphType<NewsArticle>
{
    public NewsArticleContentType(IDataLoaderContextAccessor dataLoader, IMemberService memberService)
    {
        Name = "NewsArticleContent";

        Field(x => x.Id);
        Field(x => x.PublishDate, nullable: true);
        Field(x => x.IsArchived);

        Field<StringGraphType>("title").Resolve(context => context.Source.LocalizedContents.FirstOrDefault()?.Title);
        Field<StringGraphType>("content").Resolve(context => context.Source.LocalizedContents.FirstOrDefault()?.Content);
        Field<StringGraphType>("contentPreview").Resolve(context => context.Source.LocalizedContents.FirstOrDefault()?.ContentPreview);

        Field<ListGraphType<StringGraphType>>("tags").Resolve(context => context.Source.LocalizedTags.Select(x => x.Tag));

        ExtendableField<NonNullGraphType<SeoInfoType>>("seoInfo", resolve: context => context.Source.SeoInfos.FirstOrDefault());

        ExtendableField<NewsArticleAuthorType>("author", resolve: (context) => ResolveAuthor(context, dataLoader, memberService));
    }

    protected virtual object ResolveAuthor(IResolveFieldContext<NewsArticle> context, IDataLoaderContextAccessor dataLoader, IMemberService memberService)
    {
        var loader = dataLoader.Context.GetOrAddBatchLoader<string, NewsArticleAuthor>("NewsArticles", async (ids) =>
        {
            var members = await memberService.GetByIdsAsync(ids.ToArray());
            return members
                .Select(x => new NewsArticleAuthor() { Id = x.Id, Name = x.Name, IconUrl = x.IconUrl })
                .ToDictionary(x => x.Id);
        });
        return loader.LoadAsync(context.Source.AuthorId);
    }
}
