angular.module('VirtoCommerce.News')
    .controller('VirtoCommerce.News.newsArticleContentDetailsController',
        ['$scope', 'platformWebApp.settings', 'platformWebApp.bladeNavigationService', 'FileUploader',
            function ($scope, settings, bladeNavigationService, FileUploader) {
                var blade = $scope.blade;
                blade.isLoading = false;

                var languagesPromise = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' }).$promise;
                $scope.languages = [];

                blade.refresh = function () {
                    languagesPromise.then(function (promiseResult) {
                        $scope.languages = promiseResult;
                    });
                    blade.originalEntity = blade.currentEntity;
                    blade.currentEntity = angular.copy(blade.currentEntity);
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
                            canExecuteMethod: canSave
                        }
                    ];
                };

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
                        bladeNavigationService.setError(fileItem._file.name + ' failed: ' + (response.message ? response.message : status), blade);
                    },
                    onCompleteAll: function () {
                        blade.isLoading = false;
                    }
                });

                var formScope;
                $scope.setForm = function (form) { formScope = form; }

                $scope.saveChanges = function () {
                    if (!blade.isEdit) {
                        blade.newsArticle.localizedContents.push(blade.currentEntity);
                        blade.originalEntity = blade.currentEntity;
                        blade.isEdit = true;
                        blade.title = 'news.blades.news-article-content-details.title-edit';
                    }
                    else {
                        var existing = _.find(blade.newsArticle.localizedContents, function (x) { return x === blade.originalEntity; });
                        angular.copy(blade.currentEntity, existing);
                    }
                };

                initializeToolbar();
                blade.refresh();
            }]);
