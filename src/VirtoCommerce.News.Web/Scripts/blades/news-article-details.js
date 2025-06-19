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
                const publishPermission = 'news:publish';

                const blade = $scope.blade;

                //blade properties
                blade.title = blade.isNew ? 'news.blades.news-article-details.title-add' : 'news.blades.news-article-details.title-edit';
                blade.metaFields = metaFormsService.getMetaFields('newsArticleDetails');

                //blade functions
                blade.refresh = function () {
                    if (!blade.isNew) {
                        newsApi.get({ id: [blade.itemId] }, function (getResult) {
                            blade.originalEntity = angular.copy(getResult);
                            blade.currentEntity = angular.copy(getResult);
                            blade.isLoading = false;
                        });
                    }
                    else {
                        blade.isLoading = false;
                    }
                };

                blade.onClose = function (closeCallback) {
                    bladeNavigationService.showConfirmationIfNeeded(
                        isDirty(),
                        canSave(),
                        blade,
                        $scope.saveChanges,
                        closeCallback,
                        'news.dialogs.news-article-save.title',
                        'news.dialogs.news-article-save.message'
                    );
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
                        newsApi.create(blade.currentEntity, function () {
                            blade.parentBlade.refresh(true);
                            blade.originalEntity = angular.copy(blade.currentEntity);
                            blade.isLoading = false;
                            $scope.bladeClose();
                        }, function (error) {
                            bladeNavigationService.setError('Error ' + error.status, blade);
                            blade.isLoading = false;
                        });
                    }
                    else {
                        newsApi.update(blade.currentEntity, function (updateResult) {
                            blade.parentBlade.refresh(true);
                            blade.originalEntity = angular.copy(blade.currentEntity);
                            blade.isLoading = false;
                        }, function (error) {
                            bladeNavigationService.setError('Error ' + error.status, blade);
                            blade.isLoading = false;
                        });
                    }
                };

                $scope.publish = function () {
                    //Question: save first?
                    if (!authService.checkPermission(publishPermission)) {
                        return;
                    }

                    newsApi.publish([blade.itemId], function () {
                        blade.currentEntity.isPublished = true;
                        blade.originalEntity.isPublished = true;
                        blade.parentBlade.refresh(true);
                    });
                };

                $scope.unpublish = function () {
                    if (!authService.checkPermission(publishPermission)) {
                        return;
                    }

                    newsApi.unpublish([blade.itemId], function () {
                        blade.currentEntity.isPublished = false;
                        blade.originalEntity.isPublished = false;
                        blade.parentBlade.refresh(true);
                    });
                };

                //local functions
                function isDirty() {
                    return !angular.equals(blade.currentEntity, blade.originalEntity);
                }

                function canSave() {
                    return isDirty() && formScope && formScope.$valid;
                }

                function canPublish() {
                    return !blade.isNew && blade.originalEntity && !blade.originalEntity.isPublished && blade.originalEntity.localizedContents && blade.originalEntity.localizedContents.length > 0;
                }

                function canUnpublish() {
                    return !blade.isNew && blade.originalEntity && blade.originalEntity.isPublished;
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
                        },
                        {
                            name: 'news.blades.news-article-details.toolbar.publish',
                            icon: 'fas fa-eye',
                            executeMethod: function () {
                                $scope.publish();
                            },
                            canExecuteMethod: canPublish,
                            permission: publishPermission
                        },
                        {
                            name: 'news.blades.news-article-details.toolbar.unpublish',
                            icon: 'fas fa-eye-slash',
                            executeMethod: function () {
                                $scope.unpublish();
                            },
                            canExecuteMethod: canUnpublish,
                            permission: publishPermission
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
