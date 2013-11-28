'use strict';
angular.module('Playground.game-list', ['ngResource', 'ui.router']).
    config(['$stateProvider', function config($stateProvider) {
        $stateProvider
            .state('game', {
                url: '/game',
                controller: 'ListGameController',
                data: { pageTitle: 'Games' }
            }).state('game-details', {
                url: '/game/details/:id',
                templateUrl: 'app/games/game-details.tpl.html',
                controller: 'DetailsGameController',
                data: { pageTitle: 'Game details' }
            });
    }]).
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
                $state.transitionTo('game-edit', { id: data.gameID });
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