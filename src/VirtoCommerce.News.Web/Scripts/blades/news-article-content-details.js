angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.newsArticleContentDetailsController',
        [
            '$scope',
            'platformWebApp.settings', 'platformWebApp.bladeNavigationService',
            'FileUploader',
            function (
                $scope,
                settings, bladeNavigationService,
                FileUploader) {
                const blade = $scope.blade;

                //blade properties
                blade.isLoading = false;

                //scope properties
                const languagesPromise = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' }).$promise;
                $scope.languages = [];

                $scope.fileUploader = new FileUploader({
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

                //blade functions
                blade.refresh = function () {
                    languagesPromise.then(function (promiseResult) {
                        $scope.languages = promiseResult;
                    });
                    blade.originalEntity = blade.currentEntity;
                    blade.currentEntity = angular.copy(blade.currentEntity);
                };

                //scope functions
                let formScope;
                $scope.setForm = function (form) {
                    formScope = form;
                }

                $scope.saveChanges = function () {
                    if (blade.isNew) {
                        blade.newsArticle.localizedContents.push(blade.currentEntity);
                        blade.originalEntity = blade.currentEntity;
                        blade.isNew = false;
                        blade.title = 'news.blades.news-article-content-details.title-edit';
                    }
                    else {
                        const existing = _.find(
                            blade.newsArticle.localizedContents,
                            function (x) { return x === blade.originalEntity; }
                        );
                        angular.copy(blade.currentEntity, existing);
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
                            canExecuteMethod: canSave
                        }
                    ];
                }

                //calls
                initializeToolbar();
                blade.refresh();
            }]);
