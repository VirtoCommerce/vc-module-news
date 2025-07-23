angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.contentWidgetController',
        [
            '$scope',
            'platformWebApp.bladeNavigationService',
            function (
                $scope,
                bladeNavigationService) {
                $scope.currentBlade = $scope.widget.blade;

                $scope.openBlade = function () {
                    const listBlade = {
                        id: 'newsArticleContentList',
                        controller: 'VirtoCommerce.News.newsArticleContentListController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-content-list.html',
                        item: $scope.currentBlade.currentEntity,
                    };
                    bladeNavigationService.showBlade(listBlade, $scope.currentBlade);
                };

            }]);
