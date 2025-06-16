angular.module('VirtoCommerce.News')
    .factory('VirtoCommerce.News.WebApi', ['$resource', function ($resource) {
        return $resource('api/news', {}, {
            create: { method: 'POST', url: 'api/news' },
            update: { method: 'PUT', url: 'api/news' },
            delete: { method: 'DELETE', url: 'api/news' },
            get: { method: 'GET', url: 'api/news/:id' },
            search: { method: 'POST', url: 'api/news/search' },
        });
    }]);
