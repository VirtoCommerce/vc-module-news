angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.newsArticleDetailsController',
        [
            '$scope',
            'VirtoCommerce.News.WebApi',
            'platformWebApp.authService', 'platformWebApp.bladeNavigationService', 'platformWebApp.metaFormsService',
            function (
                $scope,
                newsApi,
                authService, bladeNavigationService, metaFormsService) {
                const blade = $scope.blade;

                //blade properties
                blade.metaFields = metaFormsService.getMetaFields('newsArticleDetails');

                //blade functions
                blade.refresh = function () {
                    if (!blade.isNew) {
                        newsApi.get({ id: [blade.itemId] }, function (getResult) {
                            blade.currentEntity = angular.copy(getResult);
                            blade.isLoading = false;
                        });
                    }
                    else {
                        blade.isLoading = false;
                    }
                };

                //scope functions
                let formScope;
                $scope.setForm = function (form) {
                    formScope = form;
                }

                $scope.saveChanges = function () {
                    if (!authService.checkPermission(getSavePermission())) {
                        return;
                    }

                    blade.isLoading = true;

                    if (blade.isNew) {
                        newsApi.create(blade.currentEntity, function (createResult) {
                            blade.isNew = false;
                            blade.itemId = createResult.id;
                            blade.currentEntity.id = createResult.id;
                            blade.title = 'news.blades.news-article-details.title-edit';
                            initializeToolbar();
                            blade.isLoading = false;
                            blade.parentBlade.refresh(true);
                        }, function (error) {
                            bladeNavigationService.setError('Error ' + error.status, blade);
                            blade.isLoading = false;
                        });
                    }
                    else {
                        newsApi.update(blade.currentEntity, function () {
                            blade.isLoading = false;
                            blade.parentBlade.refresh(true);
                        }, function (error) {
                            bladeNavigationService.setError('Error ' + error.status, blade);
                            blade.isLoading = false;
                        });
                    }
                };

                //local functions
                function canSave() {
                    return formScope && formScope.$valid;
                }

                function initializeToolbar() {
                    blade.toolbarCommands = [
                        {
                            name: 'platform.commands.save',
                            icon: 'fas fa-save',
                            executeMethod: function () {
                                $scope.saveChanges();
                            },
                            canExecuteMethod: canSave,
                            permission: getSavePermission()
                        }
                    ];
                }

                function getSavePermission() {
                    return blade.isNew ? 'news:create' : 'news:update';
                }

                //calls
                initializeToolbar();
                blade.refresh();
            }]);
