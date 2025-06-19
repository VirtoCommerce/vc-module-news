// Call this to register your module to main application
const moduleName = 'VirtoCommerce.News';

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
                            const newBlade = {
                                id: 'newsArticleList',
                                controller: 'VirtoCommerce.News.newsArticleListController',
                                template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-list.html',
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
            const menuItem = {
                path: 'browse/news',
                icon: 'fa fa-cube',
                title: 'news.main-menu-title',
                priority: 100,
                action: function () { $state.go('workspace.NewsState'); },
                permission: 'news:access',
            };
            mainMenuService.addMenuItem(menuItem);

            const contentWidget = {
                controller: 'VirtoCommerce.News.contentWidgetController',
                template: 'Modules/$(VirtoCommerce.News)/Scripts/widgets/contentWidget.html'
            };
            widgetService.registerWidget(contentWidget, 'newsArticleDetails');

            metaFormsService.registerMetaFields('newsArticleDetails', [{
                name: 'name',
                title: 'news.blades.news-article-details.labels.name',
                placeholder: 'news.blades.news-article-details.placeholders.name',
                colSpan: 2,
                isRequired: true,
                valueType: 'ShortText'
            },
            {
                name: 'storeId',
                title: 'news.blades.news-article-details.labels.store',
                templateUrl: 'storeSelector.html',
                colSpan: 1,
                valueType: 'ShortText'
            },
            {
                name: 'publishDate',
                title: 'news.blades.news-article-details.labels.publish-date',
                placeholder: 'news.blades.news-article-details.placeholders.publish-date',
                colSpan: 1,
                valueType: 'DateTime'
            }]);
        }
    ]);
