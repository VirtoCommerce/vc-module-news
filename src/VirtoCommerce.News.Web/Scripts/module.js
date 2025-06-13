// Call this to register your module to main application
var moduleName = 'VirtoCommerce.News';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider',
        function ($stateProvider) {
            $stateProvider
                .state('workspace.NewsState', {
                    url: '/news',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        'platformWebApp.bladeNavigationService',
                        function (bladeNavigationService) {
                            var newBlade = {
                                id: 'newsArticleList',
                                controller: 'VirtoCommerce.News.newsArticleListController',
                                template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-list.tpl.html',
                                isClosingDisabled: true,
                            };
                            bladeNavigationService.showBlade(newBlade);
                        }
                    ]
                });
        }
    ])
    .run(['platformWebApp.mainMenuService', '$state', 'platformWebApp.metaFormsService', 'platformWebApp.widgetService',
        function (mainMenuService, $state, metaFormsService, widgetService) {
            var menuItem = {
                path: 'browse/news',
                icon: 'fa fa-cube',
                title: 'news.main-menu-title',
                priority: 100,
                action: function () { $state.go('workspace.NewsState'); },
                permission: 'news:access',
            };
            mainMenuService.addMenuItem(menuItem);

            var contentWidget = {
                controller: 'VirtoCommerce.News.contentWidgetController',
                template: 'Modules/$(VirtoCommerce.News)/Scripts/widgets/contentWidget.tpl.html'
            };
            widgetService.registerWidget(contentWidget, 'newsArticleDetail');

            metaFormsService.registerMetaFields("newsArticleDetail", [{
                name: 'name',
                title: "news.blades.news-article-detail.labels.name",
                placeholder: "news.blades.news-article-detail.placeholders.name",
                colSpan: 2,
                isRequired: true,
                valueType: "ShortText"
            },
            {
                name: 'publishDate',
                title: "news.blades.news-article-detail.labels.publish-date",
                placeholder: "news.blades.news-article-detail.placeholders.publish-date",
                colSpan: 1,
                valueType: "DateTime"
            },
            {
                name: 'isPublished',
                title: "news.blades.news-article-detail.labels.is-published",
                placeholder: "news.blades.news-article-detail.placeholders.is-published",
                colSpan: 1,
                isRequired: true,
                valueType: "Boolean"
            }]);
        }
    ]);
