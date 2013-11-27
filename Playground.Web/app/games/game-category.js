'use strict';
angular.module('Playground.game-category', ['ngResource', 'ui.router']).
    config(['$stateProvider', function config($stateProvider) {
        $stateProvider.state('game-categories', {
            url: '/game-categories',
            templateUrl: 'app/games/game-category.tpl.html',
            controller: 'GameCategoryController',
            data: { pageTitle: 'Games' }
        });
    }]).
    controller('GameCategoryController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    'GameCategoryResource',
    function ($scope, $stateParams, $rootScope, GameCategoryResource) {
        $scope.gameCategories = {};
        $scope.addingcategory = false;
        $scope.newCategory = {
            title: ''
        };

        $scope.loadGameCategories = function () {
            GameCategoryResource.getall(function (data) {
                $scope.gameCategories = data;
            });
        };

        // $scope.loadGameCategories();

        $scope.toggleAddCategory = function (show) {
            $scope.addingcategory = show;
            if (!show) {
                $scope.newCategory.title = '';
            }
        };

        $scope.addCategory = function() {
            GameCategoryResource.add($scope.newCategory, function (data, status, headers, config) {
                $scope.newCategory.title = '';
                $scope.gameCategories.push(data);
            });
        };
    }]);