'use strict';
angular.module('Playground.game-category-edit', [
    'ngResource',
    'ui.router',
    'Playground.imageupload'])
    .config(['$compileProvider',
        '$stateProvider',
        '$injector',
        function ($compileProvider, $stateProvider, $injector) {
            $stateProvider
            .state('game-category-edit', {
                abstract: true,
                url: '/game/category/edit',
                templateUrl: 'app/templates/games/game-category-edit.tpl.html',
                data: { pageTitle: 'Edit game category' },
                resolve: {
                    authenticaated: function (securityAuthorization) {
                        return securityAuthorization.requireAuthenticatedUser();
                    }
                }
            }).state('game-category-edit.info', {
                url: '/info/:id',
                controller: 'GameCategoryEditInfoController',
                templateUrl: 'app/templates/games/game-category-edit-info.tpl.html'
            }).state('game-category-edit.games', {
                url: '/games/:id',
                controller: 'GameCategoryEditGamesController',
                templateUrl: 'app/templates/games/game-category-edit-games.tpl.html'
            })
        }
    ])
    .controller('GameCategoryEditInfoController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'GameCategoryResource',
    function ($scope, $state, $stateParams, $rootScope, GameCategoryResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'info';
        $scope.gameCategory = {};
        $scope.alerts = [];

        $scope.loadGameCategory = function () {
            GameCategoryResource.getcategory({ id: $stateParams.id },
                function (data, status, headers, config) {
                    $scope.gameCategory = data;
                },
                function () {
                    $scope.addAlert($scope.alerts, { type: 'error', msg: 'Error saving game' });
                }
            );
        }

        $scope.updateGameCategory = function () {
            GameCategoryResource.update($scope.gameCategory,
                function (data, status, headers, config) {
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'Game category sucessfully updated' });
                }, function (err) {
                    var msg = err.data ? err.data.replace(/"/g, "") : "Error updating game category";
                    $scope.addAlert($scope.alerts, { type: 'error', msg: msg });
                }
            );
        };

        $scope.loadGameCategory();
    }])
    .controller('GameCategoryEditGamesController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'GameCategoryResource',
    'GameResource',
    function ($scope, $state, $stateParams, $rootScope, GameCategoryResource, GameResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'games';
        $scope.gameCategoryID = $stateParams.id;
        $scope.alerts = [];

        $scope.loadGames = function (page) {
            GameCategoryResource.getgames({
                    id: $scope.gameCategoryID,
                    page: page,
                    count: 5
                },
                function (data, status, headers, config) {
                    $scope.games = data;
                }
            );
        }

        $scope.onGamePageSelect = function (page) {
            $scope.loadGames(page);
        }

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
            $scope.newGame.gameCategoryID = $scope.gameCategoryID;

            GameResource.add($scope.newGame, function (data, status, headers, config) {
                $state.go('game-edit', { id: data.gameID });
            });
        };

        $scope.deleteGame = function (game, collection, index) {
            GameResource.remove({
                id: game.gameID
            }, function (data, status, headers, config) {
                collection.splice(index, 1);
            }, function (err) {
                $scope.addAlert($scope.alerts, { type: 'error', msg: 'Error deleting game' });
            });
        };

        $scope.loadGames(1);
    }]);