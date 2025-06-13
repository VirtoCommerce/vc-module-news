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
                                id: 'newsList',
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
    .run(['platformWebApp.mainMenuService', '$state', 'platformWebApp.metaFormsService',
        function (mainMenuService, $state, metaFormsService) { 
            var menuItem = {
                path: 'browse/news',
                icon: 'fa fa-cube',
                title: 'news.main-menu-title',
                priority: 100,
                action: function () { $state.go('workspace.NewsState'); },
                permission: 'news:access',
            };
            mainMenuService.addMenuItem(menuItem);

             
            metaFormsService.registerMetaFields("newsArticleDetail", [{
                name: 'name',
                title: "news.blades.news-article-detail.labels.name",
                placeholder: "news.blades.news-article-detail.placeholders.name",
                colSpan: 2,
                isRequired: true,
                valueType: "ShortText"
            }]);
        }
    ]);
