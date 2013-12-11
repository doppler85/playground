'use strict';
angular.module('Playground.game-list', ['ngResource', 'ui.router']).
    controller('ListGameController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'GameResource',
    function ($scope, $stateParams, $rootScope, $state, GameResource) {
        $scope.addinggame = false;
        $scope.newGame = {
            title: ''
        };

        $scope.toggleAddGame = function (show) {
            $scope.addinggame = show;
            if (!show) {
                $scope.newGame.title = '';
            }
        };

        $scope.addGame = function (gameCategory) {
            $scope.newGame.gameCategoryID = gameCategory.gameCategoryID;

            GameResource.add($scope.newGame, function (data, status, headers, config) {
                $state.go('game-edit', { id: data.gameID });
            });
        };
    }]).
    controller('DetailsGameController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    'GameResource',
    function ($scope, $stateParams, $rootScope, GameResource) {
        console.log($stateParams);
    }]);