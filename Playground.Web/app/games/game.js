'use strict';
angular.module('Playground.game', ['ngResource', 'ui.router']).
    config(['$stateProvider', function config($stateProvider) {
        $stateProvider.state('game', {
            url: '/game/:id',
            templateUrl: 'app/games/game.tpl.html',
            controller: 'GameController',
            data: { pageTitle: 'Games' }
        });
    }]).
    controller('GameController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    'GameResource',
    function ($scope, $stateParams, $rootScope, GameResource) {
        $scope.addinggame = false;
        $scope.newGame = {
            gameCategoryID: $scope.gameCategory.gameCategoryID,
            title: ''
        };

        $scope.toggleAddGame = function (show) {
            $scope.addinggame = show;
            if (!show) {
                $scope.newGame.title = '';
            }
        };

        $scope.addGame = function() {
            GameResource.add($scope.newGame, function () {
                $scope.newGame.title = '';
                $scope.loadGameCategories();
            });
        };
    }]);