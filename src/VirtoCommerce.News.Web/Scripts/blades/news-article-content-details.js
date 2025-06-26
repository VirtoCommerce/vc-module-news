angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.newsArticleContentDetailsController',
        [
            '$scope',
            'platformWebApp.settings', 'platformWebApp.bladeNavigationService', 'platformWebApp.metaFormsService',
            'FileUploader',
            function (
                $scope,
                settings, bladeNavigationService, metaFormsService,
                FileUploader) {
                const blade = $scope.blade;

                //blade properties
                blade.metaFields = metaFormsService.getMetaFields('newsArticleContentDetails');
                blade.isLoading = false;

                const languagesPromise = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' }).$promise;
                blade.languages = [];

                blade.fileUploader = createFileUploader();

                //blade functions
                blade.refresh = function () {
                    languagesPromise.then(function (promiseResult) {
                        blade.languages = promiseResult;
                    });
                    blade.originalEntity = blade.currentEntity;
                    blade.currentEntity = angular.copy(blade.currentEntity);
                };

                blade.onClose = function (closeCallback) {
                    bladeNavigationService.showConfirmationIfNeeded(
                        isDirty(),
                        canSave(),
                        blade,
                        $scope.saveChanges,
                        closeCallback,
                        'news.dialogs.news-article-content-save.title',
                        'news.dialogs.news-article-content-save.message'
                    );
                };

                //scope functions
                let formScope;
                $scope.setForm = function (form) {
                    formScope = form;
                };

                $scope.saveChanges = function () {
                    if (blade.isNew) {
                        blade.newsArticle.localizedContents.push(blade.currentEntity);
                        blade.parentBlade.refresh(true);
                        blade.originalEntity = angular.copy(blade.currentEntity);
                        $scope.bladeClose();
                    }
                    else {
                        const existing = _.find(
                            blade.newsArticle.localizedContents,
                            function (x) { return x === blade.originalEntity; }
                        );
                        angular.copy(blade.currentEntity, existing);
                        angular.copy(blade.currentEntity, blade.originalEntity);
                    }
                };

                //local functions
                function isDirty() {
                    return !angular.equals(blade.currentEntity, blade.originalEntity);
                }

                function canSave() {
                    return isDirty() && formScope && formScope.$valid;
                }

                function reset() {
                    angular.copy(blade.originalEntity, blade.currentEntity);
                    $scope.$broadcast('resetContent', { body: blade.currentEntity.content });
                }

                function initializeToolbar() {
                    blade.toolbarCommands = [
                        {
                            name: 'platform.commands.save',
                            icon: 'fas fa-save',
                            executeMethod: function () {
                                $scope.saveChanges();
                            },
                            canExecuteMethod: canSave
                        }
                    ];

                    if (!blade.isNew) {
                        blade.toolbarCommands.push({
                            name: "platform.commands.reset",
                            icon: 'fa fa-undo',
                            executeMethod: reset,
                            canExecuteMethod: isDirty
                        });
                    }
                }

                function initializeTitle() {
                    blade.title = blade.isNew ? 'news.blades.news-article-content-details.title-add' : 'news.blades.news-article-content-details.title-edit';
                }

                function createFileUploader() {
                    return new FileUploader({
                        url: 'api/assets?folderUrl=news-articles/' + blade.newsArticle.id,
                        headers: { Accept: 'application/json' },
                        autoUpload: true,
                        removeAfterUpload: true,
                        onBeforeUploadItem: function (fileItem) {
                            blade.isLoading = true;
                        },
                        onSuccessItem: function (fileItem, response) {
                            $scope.$broadcast('filesUploaded', { items: response });
                        },
                        onErrorItem: function (fileItem, response, status) {
                            bladeNavigationService.setError(`${fileItem._file.name} failed: ${(response.message ? response.message : status)}`, blade);
                        },
                        onCompleteAll: function () {
                            blade.isLoading = false;
                        }
                    });
                }

                //calls
                initializeToolbar();
                initializeTitle();
                blade.refresh();
            }]);
