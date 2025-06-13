angular.module('VirtoCommerce.News')
    .controller('VirtoCommerce.News.contentWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
        $scope.currentBlade = $scope.widget.blade;

        $scope.openBlade = function () {
            var listBlade = {
                id: "newsArticleContentList",
                item: $scope.currentBlade.currentEntity,
                controller: 'VirtoCommerce.News.newsArticleContentListController',
                template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-content-list.tpl.html'
            };

            bladeNavigationService.showBlade(listBlade, $scope.currentBlade);
        };

    }]);
