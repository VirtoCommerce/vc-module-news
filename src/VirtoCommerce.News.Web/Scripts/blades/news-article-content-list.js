angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.newsArticleContentListController',
        [
            '$scope',
            'platformWebApp.bladeNavigationService', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService',
            function (
                $scope,
                bladeNavigationService, uiGridHelper, dialogService) {
                const blade = $scope.blade;

                //blade properties
                blade.title = 'news.blades.news-article-content-list.title';
                blade.isLoading = false;

                //blade functions
                blade.refresh = function () {

                };

                //scope functions
                $scope.add = function () {
                    $scope.selectedNodeId = undefined;

                    showDetailsBlade(true);
                };

                $scope.edit = function (item) {
                    showDetailsBlade(false, item);
                };

                $scope.delete = function (item) {
                    const dialog = {
                        id: 'newsArticleContentDeleteDialog',
                        title: 'news.dialogs.news-article-content-delete.title',
                        message: 'news.dialogs.news-article-content-delete.message',
                        messageValues: { language: item.languageCode },
                        callback: function (dialogConfirmed) {
                            if (dialogConfirmed) {
                                blade.item.localizedContents.splice(blade.item.localizedContents.indexOf(item), 1);
                            }
                        }
                    };
                    dialogService.showConfirmationDialog(dialog);
                };

                // ui-grid
                $scope.setGridOptions = function (gridOptions) {
                    uiGridHelper.initialize($scope, gridOptions);
                };

                //local functions
                function initializeToolbar() {
                    blade.toolbarCommands = [
                        {
                            name: 'platform.commands.add',
                            icon: 'fas fa-plus',
                            executeMethod: $scope.add,
                            canExecuteMethod: function () {
                                return true;
                            }
                        }
                    ];
                }

                function showDetailsBlade(isNew, item) {
                    const detailsBlade = {
                        id: 'newsArticleContentDetails',
                        isNew: isNew,
                        newsArticle: blade.item,
                        currentEntity: item,
                        title: isNew ? 'news.blades.news-article-content-details.title-add' : 'news.blades.news-article-content-details.title-edit',
                        controller: 'VirtoCommerce.News.newsArticleContentDetailsController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-content-details.html'
                    };
                    bladeNavigationService.showBlade(detailsBlade, blade);
                }

                //calls
                initializeToolbar();
            }]);
