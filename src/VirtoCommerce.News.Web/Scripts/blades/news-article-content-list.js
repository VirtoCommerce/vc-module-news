angular.module('VirtoCommerce.News')
    .controller(
        'VirtoCommerce.News.newsArticleContentListController',
        ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService',
            function ($scope, bladeNavigationService, uiGridHelper, dialogService) {
                var blade = $scope.blade;
                blade.title = 'news.blades.news-article-content-list.title'; 
                blade.isLoading = false;

                blade.refresh = function () {

                };

                $scope.add = function () { 
                    selectedNode = undefined;
                    $scope.selectedNodeId = undefined;

                    var detailsBlade = {
                        id: 'newsArticleContentAdd',
                        isEdit: false,
                        newsArticle: blade.item,
                        title: 'news.blades.news-article-content-detail.title-add',
                        controller: 'VirtoCommerce.News.newsArticleContentDetailController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-content-detail.tpl.html'
                    };

                    bladeNavigationService.showBlade(detailsBlade, blade);
                };

                $scope.edit = function (item) { 
                    var detailsBlade = {
                        id: 'newsArticleContentEdit',
                        isEdit: true,
                        itemId: item.id,
                        newsArticle: blade.item,
                        currentEntity: item,
                        title: 'news.blades.news-article-content-detail.title-edit',
                        controller: 'VirtoCommerce.News.newsArticleContentDetailController',
                        template: 'Modules/$(VirtoCommerce.News)/Scripts/blades/news-article-content-detail.tpl.html'
                    };
                    bladeNavigationService.showBlade(detailsBlade, blade);
                };

                $scope.delete = function (item) { 
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

                blade.toolbarCommands = [{
                    name: "platform.commands.add",
                    icon: 'fas fa-plus',
                    executeMethod: $scope.add,
                    canExecuteMethod: function () {
                        return true;
                    }
                }]; 

                // ui-grid
                $scope.setGridOptions = function (gridOptions) {
                    uiGridHelper.initialize($scope, gridOptions);
                };
            }]);
