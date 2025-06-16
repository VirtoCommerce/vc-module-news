angular.module('VirtoCommerce.News')
    .controller('VirtoCommerce.News.contentWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
        $scope.currentBlade = $scope.widget.blade;

        $scope.openBlade = function () {
            const listBlade = {
                id: "newsArticleContentList",
                item: $scope.currentBlade.currentEntity,
                controller: 'VirtoCommerce.News.newsArticleContentListController',
                template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-content-list.html'
            };
            bladeNavigationService.showBlade(listBlade, $scope.currentBlade);
        };

    }]);
