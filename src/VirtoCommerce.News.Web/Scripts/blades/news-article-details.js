angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.newsArticleDetailsController',
        [
            '$scope',
            'VirtoCommerce.News.WebApi', 'virtoCommerce.storeModule.stores',
            'platformWebApp.authService', 'platformWebApp.bladeNavigationService', 'platformWebApp.metaFormsService',
            function (
                $scope,
                newsApi, storeApi,
                authService, bladeNavigationService, metaFormsService) {
                const publishPermission = 'news:publish';

                const blade = $scope.blade;

                //blade properties
                blade.title = blade.isNew ? 'news.blades.news-article-details.title-add' : 'news.blades.news-article-details.title-edit';
                blade.metaFields = metaFormsService.getMetaFields('newsArticleDetails');
                blade.stores = storeApi.query();

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
                    return !isDirty() && hasPublishedState(false) && hasContent();
                }

                function canUnpublish() {
                    return !isDirty() && hasPublishedState(true);
                }

                function hasContent() {
                    return blade.originalEntity && blade.originalEntity.localizedContents && (blade.originalEntity.localizedContents.length > 0);
                }

                function hasPublishedState(isPublished) {
                    return blade.originalEntity && (blade.originalEntity.isPublished === isPublished);
                }

                function reset() {
                    angular.copy(blade.originalEntity, blade.currentEntity);
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

                    if (!blade.isNew) {
                        blade.toolbarCommands.push({
                            name: "platform.commands.reset",
                            icon: 'fa fa-undo',
                            executeMethod: reset,
                            canExecuteMethod: isDirty
                        });
                        blade.toolbarCommands.push({
                            name: 'news.blades.news-article-details.toolbar.publish',
                            icon: 'fas fa-eye',
                            executeMethod: function () {
                                $scope.publish();
                            },
                            canExecuteMethod: canPublish,
                            permission: publishPermission
                        });
                        blade.toolbarCommands.push({
                            name: 'news.blades.news-article-details.toolbar.unpublish',
                            icon: 'fas fa-eye-slash',
                            executeMethod: function () {
                                $scope.unpublish();
                            },
                            canExecuteMethod: canUnpublish,
                            permission: publishPermission
                        });
                    }
                }

                function getSavePermission() {
                    return blade.isNew ? 'news:create' : 'news:update';
                }

                //calls
                initializeToolbar();
                blade.refresh();
            }]);
