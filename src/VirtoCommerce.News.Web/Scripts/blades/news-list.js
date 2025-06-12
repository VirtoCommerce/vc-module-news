angular.module('VirtoCommerce.News')
    .controller('VirtoCommerce.News.newsArticleController', ['$scope', 'VirtoCommerce.News.WebApi', function ($scope, api) {
        var blade = $scope.blade;
        blade.title = 'VirtoCommerce.News';

        blade.refresh = function () {
            api.getAll(function (data) {
                blade.title = 'news.blades.news-list.title';
                blade.data = data.results;
                blade.isLoading = false;
            });
        };

        blade.refresh();
    }]);
