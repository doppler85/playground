'use strict';
angular.module('Playground.player-add', ['ngResource', 'ui.router', 'ui.bootstrap.alert'])
    .filter('gamefull', function () {
        return function (game) {
            return game ? '(' + game.category.title + ') ' + game.title : '';
        };
    })
    .controller('PlayerAddController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'UserResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, UserResource, enums) {
        $scope.player = {
            games: []
        };
        $scope.selectedCategory = null;
        $scope.categories = [];
        $scope.games = [];
        $scope.alerts = [];

        $scope.addAlert = function (msg) {
            $scope.alerts.push(msg);
        };

        $scope.closeAlert = function (index) {
            $scope.alerts.splice(index, 1);
        };

        $scope.loadGames = function () {
            UserResource.individualGames(function (data, status, headers, config) {
                $scope.categories = data;
            });
        };

        $scope.categoryChanged = function () {
            if ($scope.selectedCategory != null) {
                $scope.games = $scope.selectedCategory.games;
            }
            else {
                $scope.games = [];
            }
        };

        $scope.toggleGame = function (game) {
            game.checked = !game.checked;
        };

        $scope.addPlayer = function () {
            angular.forEach($scope.games, function (game) {
                if (game.checked) {
                    $scope.player.games.push({
                        gameID: game.gameID
                    });
                }
            });
            UserResource.addPlayer($scope.player,
                function (data, status, headers, config) {
                    $scope.player = data.player;
                    $scope.addAlert({ type: 'success', msg: 'Player sucessfully added' });
                    $state.go('profile');
                },
                function () {
                    $scope.addAlert({ type: 'error', msg: 'Error adding player' });
                    $scope.player.games = [];
                });
        };

        $scope.cancel = function () {
            $state.go('profile');
        };

        $scope.loadGames();
    }]);