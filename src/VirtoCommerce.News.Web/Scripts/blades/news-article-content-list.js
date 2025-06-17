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

                $scope.delete = function (items) {
                    const dialog = {
                        id: 'newsArticleContentDeleteDialog',
                        title: 'news.dialogs.news-article-content-delete.title',
                        message: items.length === 1 ? 'news.dialogs.news-article-content-delete.message-single' : 'news.dialogs.news-article-content-delete.message-many',
                        messageValues: items.length === 1 ? { language: items[0].languageCode } : { count: items.length },
                        callback: function (dialogConfirmed) {
                            if (dialogConfirmed) {
                                angular.forEach(
                                    items,
                                    function (item) {
                                        blade.item.localizedContents.splice(blade.item.localizedContents.indexOf(item), 1);
                                    });
                            }
                        }
                    };
                    dialogService.showConfirmationDialog(dialog);
                };

                // ui-grid
                $scope.setGridOptions = function (gridOptions) {
                    uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                        $scope.gridApi = gridApi;
                    });
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
                        },
                        {
                            name: 'platform.commands.delete',
                            icon: 'fas fa-trash-alt',
                            executeMethod: function () {
                                $scope.delete($scope.gridApi.selection.getSelectedRows())
                            },
                            canExecuteMethod: hasSelectedItems
                        }
                    ];
                }

                function hasSelectedItems() {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                }

                function showDetailsBlade(isNew, item) {
                    const detailsBlade = {
                        id: 'newsArticleContentDetails',
                        isNew: isNew,
                        newsArticle: blade.item,
                        currentEntity: item,
                        controller: 'VirtoCommerce.News.newsArticleContentDetailsController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-content-details.html'
                    };
                    bladeNavigationService.showBlade(detailsBlade, blade);
                }

                //calls
                initializeToolbar();
            }]);
