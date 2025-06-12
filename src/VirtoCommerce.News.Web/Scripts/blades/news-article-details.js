angular.module('VirtoCommerce.News')
    .controller('VirtoCommerce.News.newsArticleDetailsController',
        ['$scope', 'VirtoCommerce.News.WebApi', 'platformWebApp.authService',
            function ($scope, api, authService) {
                var blade = $scope.blade;

                blade.refresh = function () {
                    if (blade.isEdit) {
                        api.get({ ids: [blade.itemId] }, function (apiResult) {
                            blade.item = angular.copy(apiResult[0]);
                            blade.isLoading = false;
                        });
                    }
                    else {
                        blade.isLoading = false;
                    }
                };

                function canSave() {
                    return formScope && formScope.$valid;
                };

                function initializeToolbar() {
                    blade.toolbarCommands = [
                        {
                            name: "platform.commands.save", icon: 'fas fa-save',
                            executeMethod: function () {
                                $scope.saveChanges();
                            },
                            canExecuteMethod: canSave,
                            permission: getSavePermission()
                        }
                    ];
                };

                function getSavePermission() {
                    return blade.isEdit ? 'news:update' : 'news:create';
                };

                var formScope;
                $scope.setForm = function (form) { formScope = form; }

                $scope.saveChanges = function () {
                    if (!authService.checkPermission(getSavePermission())) {
                        return;
                    }

                    blade.isLoading = true;

                    if (!blade.isEdit) {
                        api.create(blade.item, function (apiResult) {
                            blade.isEdit = true;
                            blade.itemId = apiResult.id;

                            blade.item = angular.copy(apiResult);
                            blade.title = 'news.blades.news-article-details.title-edit';
                            blade.titleValues = { name: apiResult.name };
                            initializeToolbar();
                            blade.isLoading = false;
                            blade.parentBlade.refresh(true);
                        }, function (error) {
                            bladeNavigationService.setError('Error ' + error.status, blade);
                            blade.isLoading = false;
                        });
                    }
                    else {
                        api.update(blade.item, function (apiResult) {
                            blade.item = angular.copy(apiResult);
                            blade.titleValues = { name: apiResult.name };
                            blade.isLoading = false;
                            blade.parentBlade.refresh(true);
                        }, function (error) {
                            bladeNavigationService.setError('Error ' + error.status, blade);
                            blade.isLoading = false;
                        });
                    }
                };

                initializeToolbar();
                blade.refresh();
            }]);
