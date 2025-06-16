angular.module('VirtoCommerce.News')
    .factory('VirtoCommerce.News.webApi', ['$resource', function ($resource) {
        return $resource('api/news');
    }]);
