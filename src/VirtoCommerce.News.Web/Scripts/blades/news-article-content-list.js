angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.newsArticleContentListController',
        ['$scope', 'platformWebApp.authService', 'platformWebApp.bladeNavigationService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils', 'platformWebApp.dialogService',
            function ($scope, authService, bladeNavigationService, uiGridHelper, bladeUtils, dialogService) {
                var blade = $scope.blade;
                blade.title = 'news.blades.news-article-content-list.title';
                blade.updatePermission = 'news:update';
                blade.isLoading = false;

                blade.refresh = function () {

                };

                $scope.add = function () {
                    if (!authService.checkPermission(blade.updatePermission)) {
                        return;
                    }

                    selectedNode = undefined;
                    $scope.selectedNodeId = undefined;

                    var detailsBlade = {
                        id: 'newsArticleContentAdd',
                        isEdit: false,
                        title: 'news.blades.news-article-content-detail.title-add',
                        controller: 'VirtoCommerce.News.newsArticleContentDetailsController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-content-detail.tpl.html'
                    };

                    bladeNavigationService.showBlade(detailsBlade, blade);
                };

                $scope.edit = function (item) {
                    if (!authService.checkPermission(blade.updatePermission)) {
                        return;
                    }

                    var detailsBlade = {
                        id: 'newsArticleContentEdit',
                        isEdit: true,
                        itemId: item.id,
                        title: 'news.blades.news-article-content-detail.title-edit',
                        controller: 'VirtoCommerce.News.newsArticleContentDetailsController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-content-detail.tpl.html'
                    };
                    bladeNavigationService.showBlade(detailsBlade, blade);
                };

                $scope.delete = function (item) {
                    console.warn('delete-content', item);
                    if (!authService.checkPermission(blade.updatePermission)) {
                        return;
                    }

                    var dialog = {
                        id: "newsArticleContentDeleteDialog",
                        title: "news.dialogs.news-article-content-delete.title",
                        message: "news.dialogs.news-article-content-delete.message",
                        messageValues: { language: item.languageCode },
                        callback: function (dialogConfirmed) {
                            if (dialogConfirmed) {
                                blade.item.localizedContents.splice(blade.item.localizedContents.indexOf(item), 1);
                            }
                        }
                    };
                    dialogService.showConfirmationDialog(dialog);
                };

                blade.toolbarCommands = [];
                if (authService.checkPermission(blade.updatePermission)) {
                    blade.toolbarCommands.splice(0,
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

                // ui-grid
                $scope.setGridOptions = function (gridOptions) {
                    uiGridHelper.initialize($scope, gridOptions);
                };
            }]);
