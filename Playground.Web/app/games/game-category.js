'use strict';
angular.module('Playground.game-category', ['ngResource', 'ui.router']).
    controller('GameCategoryController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    'GameCategoryResource',
    'GameResource',
    function ($scope, $stateParams, $rootScope, GameCategoryResource, GameResource) {

        $scope.gameCategories = {};
        $scope.addingcategory = false;
        $scope.createCategory = function () {
            var retVal = {
                title: '',
                games: []
            };
        };

        $scope.newCategory = $scope.createCategory();

        $scope.loadGameCategories = function () {
            GameCategoryResource.getall(function (data) {
                $scope.gameCategories = data;
            });
        };

        $scope.toggleAddCategory = function (show) {
            $scope.addingcategory = show;
            if (!show) {
                $scope.newCategory = $scope.createCategory();
            }
        };

        $scope.addCategory = function() {
            GameCategoryResource.add($scope.newCategory, function (data, status, headers, config) {
                $scope.gameCategories.push(data);
                $scope.toggleAddCategory(false);
            });
        };

        $scope.deleteCategory = function (category, index) {
            GameCategoryResource.remove({ id: category.gameCategoryID }, function (data, status, headers, config) {
                $scope.gameCategories.splice(index, 1);
            });
        };

        $scope.deleteGame = function (game, collection, index) {
            GameResource.remove({ id: game.gameID }, function (data, status, headers, config) {
                collection.splice(index, 1);
            });
        };

        $scope.loadGameCategories();
    }]);