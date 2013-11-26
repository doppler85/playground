'use strict';
angular.module('Playground.games', ['ngResource', 'ui.router']).
    config(['$stateProvider', function config($stateProvider) {
        $stateProvider.state('games', {
            url: '/games',
            templateUrl: 'app/games/games.tpl.html',
            controller: 'GameController',
            data: { pageTitle: 'Games' }
        });
    }]).
    controller('GameController', [
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

        $scope.loadGameCategories();

        $scope.toggleAddCategory = function (show) {
            $scope.addingcategory = show;
            if (!show) {
                $scope.newCategory.title = '';
            }
        };

        $scope.addCategory = function() {
            GameCategoryResource.add($scope.newCategory, function () {
                $scope.newCategory.title = '';
                $scope.loadGameCategories();
            });
        };
    }]);