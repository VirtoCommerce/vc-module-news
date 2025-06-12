angular.module('VirtoCommerce.News')
    .factory('VirtoCommerce.News.WebApi', ['$resource', function ($resource) {
        return $resource('api/news', {}, {
            create: { method: 'POST', url: 'api/news/create' },
            update: { method: 'POST', url: 'api/news/update' },
            delete: { method: 'DELETE', url: 'api/news/delete' },
            get: { method: 'GET', url: 'api/news/get' },
            getAll: { method: 'GET', url: 'api/news/get-all' },
        });
    }]);
