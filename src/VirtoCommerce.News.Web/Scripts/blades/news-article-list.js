angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.newsArticleListController',
        [
            '$scope',
            'VirtoCommerce.News.WebApi',
            'platformWebApp.authService', 'platformWebApp.bladeNavigationService', 'platformWebApp.uiGridHelper',
            'platformWebApp.bladeUtils', 'platformWebApp.dialogService',
            function (
                $scope,
                newsApi,
                authService, bladeNavigationService, uiGridHelper,
                bladeUtils, dialogService) {
                const blade = $scope.blade;

                //blade properties
                blade.title = 'news.blades.news-article-list.title';

                //blade functions
                blade.refresh = function () {
                    newsApi.search(getSearchCriteria(), function (searchResult) {
                        blade.data = searchResult.results;
                        $scope.pageSettings.totalItems = searchResult.totalCount;
                        blade.isLoading = false;
                    });
                };

                //scope functions
                $scope.add = function () {
                    if (!authService.checkPermission('news:create')) {
                        return;
                    }

                    $scope.selectedNodeId = undefined;

                    showDetailsBlade(true);
                };

                $scope.edit = function (item) {
                    if (!authService.checkPermission('news:update')) {
                        return;
                    }

                    showDetailsBlade(false, item.id);
                };

                $scope.delete = function (items) {
                    if (!authService.checkPermission('news:delete')) {
                        return;
                    }

                    const dialog = {
                        id: 'newsArticleDeleteDialog',
                        title: 'news.dialogs.news-article-delete.title',
                        message: items.length === 1 ? 'news.dialogs.news-article-delete.message-single' : 'news.dialogs.news-article-delete.message-many',
                        messageValues: items.length === 1 ? { name: items[0].name } : { count: items.length },
                        callback: function (dialogConfirmed) {
                            if (dialogConfirmed) {
                                bladeNavigationService.closeChildrenBlades(blade);
                                blade.isLoading = true;
                                var ids = _.pluck(items, 'id');
                                newsApi.delete({ ids: ids }, function () {
                                    blade.refresh();
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
                        uiGridHelper.bindRefreshOnSortChanged($scope);
                    });
                    bladeUtils.initializePagination($scope);
                };

                //other functions
                const filter = $scope.filter = blade.filter || {};
                filter.criteriaChanged = function () {
                    if ($scope.pageSettings.currentPage > 1) {
                        $scope.pageSettings.currentPage = 1;
                    }
                    blade.refresh();
                };

                //local functions
                function getSearchCriteria() {
                    return {
                        searchPhrase: filter.keyword ? filter.keyword : undefined,
                        sort: uiGridHelper.getSortExpression($scope),
                        skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                        take: $scope.pageSettings.itemsPerPageCount
                    };
                }

                function initializeToolbar() {
                    blade.toolbarCommands = [
                        {
                            name: 'platform.commands.refresh',
                            icon: 'fa fa-refresh',
                            executeMethod: blade.refresh,
                            canExecuteMethod: function () {
                                return true;
                            }
                        },
                        {
                            name: 'platform.commands.add',
                            icon: 'fas fa-plus',
                            executeMethod: $scope.add,
                            canExecuteMethod: function () {
                                return true;
                            },
                            permission: 'news:create'
                        },
                        {
                            name: 'platform.commands.delete',
                            icon: 'fas fa-trash-alt',
                            executeMethod: function () {
                                $scope.delete($scope.gridApi.selection.getSelectedRows())
                            },
                            canExecuteMethod: hasSelectedItems,
                            permission: 'news:delete'
                        }
                    ];
                }

                function hasSelectedItems() {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                }

                function showDetailsBlade(isNew, itemId) {
                    const detailsBlade = {
                        id: 'newsArticleDetails',
                        controller: 'VirtoCommerce.News.newsArticleDetailsController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-details.html',
                        isNew: isNew,
                        itemId: itemId,
                    };

                    bladeNavigationService.showBlade(detailsBlade, blade);
                }

                //calls
                initializeToolbar();
            }]);
