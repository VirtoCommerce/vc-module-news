angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.newsArticleListController',
        ['$scope', 'VirtoCommerce.News.WebApi', 'platformWebApp.authService', 'platformWebApp.bladeNavigationService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils', 'platformWebApp.dialogService',
            function ($scope, api, authService, bladeNavigationService, uiGridHelper, bladeUtils, dialogService) {
                const blade = $scope.blade;

                //blade properties
                blade.title = 'news.blades.news-article-list.title';

                //blade functions
                blade.refresh = function () {
                    api.search(getSearchCriteria(), function (apiResult) {
                        blade.data = apiResult.results;
                        $scope.pageSettings.totalItems = apiResult.totalCount;
                        blade.isLoading = false;
                    });
                };

                //scope functions
                $scope.add = function () {
                    if (!authService.checkPermission('news:create')) {
                        return;
                    }

                    selectedNode = undefined;
                    $scope.selectedNodeId = undefined;

                    const detailsBlade = {
                        id: 'newsArticleAdd',
                        isEdit: false,
                        title: 'news.blades.news-article-details.title-add',
                        controller: 'VirtoCommerce.News.newsArticleDetailsController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-details.html'
                    };

                    bladeNavigationService.showBlade(detailsBlade, blade);
                };

                $scope.edit = function (item) {
                    if (!authService.checkPermission('news:update')) {
                        return;
                    }

                    const detailsBlade = {
                        id: 'newsArticleEdit',
                        isEdit: true,
                        itemId: item.id,
                        title: 'news.blades.news-article-details.title-edit',
                        titleValues: { name: item.name },
                        controller: 'VirtoCommerce.News.newsArticleDetailsController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-details.html'
                    };
                    bladeNavigationService.showBlade(detailsBlade, blade);
                };

                $scope.delete = function (item) {
                    if (!authService.checkPermission('news:delete')) {
                        return;
                    }

                    const dialog = {
                        id: "newsArticleDeleteDialog",
                        title: "news.dialogs.news-article-delete.title",
                        message: "news.dialogs.news-article-delete.message",
                        messageValues: { name: item.name },
                        callback: function (dialogConfirmed) {
                            if (dialogConfirmed) {
                                blade.isLoading = true;
                                api.delete({ ids: [item.id] }, function () {
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
                            name: "platform.commands.refresh",
                            icon: 'fa fa-refresh',
                            executeMethod: blade.refresh,
                            canExecuteMethod: function () {
                                return true;
                            }
                        }
                    ];
                    if (authService.checkPermission('news:create')) {
                        blade.toolbarCommands.splice(1,
                            0,
                            {
                                name: "platform.commands.add",
                                icon: 'fas fa-plus',
                                executeMethod: $scope.add,
                                canExecuteMethod: function () {
                                    return true;
                                }
                            });
                    }
                }

                //calls
                initializeToolbar();
            }]);
