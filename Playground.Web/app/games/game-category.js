'use strict';
angular.module('Playground.game-category', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.pagination']).
    controller('GameCategoryController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'GameCategoryResource',
    function ($scope, $stateParams, $rootScope, $state, GameCategoryResource, GameResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.gameCategories = {};
        $scope.addingcategory = false;
        $scope.createCategory = function () {
            var retVal = {
                title: '',
                games: []
            };
        };

        $scope.newCategory = $scope.createCategory();

        $scope.loadGameCategories = function (page) {
            GameCategoryResource.getall({
                page: page,
                count: 5
            }, function (data) {
                $scope.gameCategories = data;
            });
        };

        $scope.onCategoryPageSelect = function (page) {
            $scope.loadGameCategories(page);
        };

        $scope.toggleAddCategory = function (show) {
            $scope.addingcategory = show;
            if (!show) {
                $scope.newCategory = $scope.createCategory();
            }
        };

        $scope.addCategory = function() {
            GameCategoryResource.add($scope.newCategory, function (data, status, headers, config) {
                $scope.loadGameCategories($scope.gameCategories.currentPage);
                $scope.addingcategory = false;
            });
        };

        $scope.deleteCategory = function (category, index) {
            GameCategoryResource.remove({ id: category.gameCategoryID }, function (data, status, headers, config) {
                $scope.gameCategories.splice(index, 1);
            });
        };

        $scope.loadGameCategories(1);
    }]);