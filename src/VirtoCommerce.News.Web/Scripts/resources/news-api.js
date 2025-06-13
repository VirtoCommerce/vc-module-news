angular.module('VirtoCommerce.News')
    .factory('VirtoCommerce.News.WebApi', ['$resource', function ($resource) {
        return $resource('api/news', {}, {
            create: { method: 'POST', url: 'api/news/create' },
            update: { method: 'POST', url: 'api/news/update' },
            delete: { method: 'DELETE', url: 'api/news/delete' },
            get: { method: 'GET', url: 'api/news/get', isArray: true },
            search: { method: 'POST', url: 'api/news/search' },
        });
    }]);
