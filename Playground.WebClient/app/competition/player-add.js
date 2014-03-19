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
    'GameResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, UserResource, GameResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.player = {
            competitorID: 0,
            games: []
        };
        $scope.selectedCategory = null;
        $scope.categories = [];
        $scope.games = [];
        $scope.alerts = [];

        $scope.loadCategories = function () {
            GameResource.individualcategories(function (data, status, headers, config) {
                $scope.categories = data;
            });
        };

        $scope.loadGames = function (categoryID) {
            GameResource.individualgames({ id: categoryID },
                function (data, status, headers, config) {
                    $scope.games = data;
            });
        };

        $scope.categoryChanged = function () {
            $scope.games = [];
            if ($scope.selectedCategory != null) {
                $scope.loadGames($scope.selectedCategory.gameCategoryID);
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
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'Player sucessfully added' });
                    $state.go('profile.players');
                },
                function () {
                    $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Error adding player' });
                    $scope.player.games = [];
                }
            );
        };

        $scope.cancel = function () {
            $state.go('profile.players');
        };

        $scope.loadCategories();
    }]);