'use strict';
angular.module('Playground.team-add', ['ngResource', 'ui.router', 'ui.bootstrap.alert'])
    .filter('playerfull', function () {
        return function (player) {
            return (player && player.user) ? '(' + player.user.firstName + ' ' + player.user.lastName + ') ' + player.name : '';
        };
    }).filter('playeruserfull', function () {
        return function (player) {
            return (player && player.user) ? player.user.firstName + ' ' + player.user.lastName : '';
        };
    })
    .controller('TeamAddController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'UserResource',
    'GameResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, UserResource, GameResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.team = {
            competitorID: 0,
            players: [],
            games: []
        };

        $scope.games = [];
        $scope.players = [];
        $scope.categories = [];
        $scope.selectedCategory = null;
        $scope.myplayer = {};
        $scope.searchQuery = '';
        $scope.availablePlayers = [];
        $scope.selectedPlayers = [];
        $scope.alerts = [];

        $scope.players.indexOf = function (obj) {
            var index = -1;
            if (obj == null || obj.competitorID == null) {
                return index;
            }
            for (var i = 0, imax = this.length; i < imax; i++) {
                if (this[i].playerID === obj.competitorID) {
                    index = i;
                    break;
                }
            }
            return index;
        };
        
        $scope.addAlert = function (msg) {
            $scope.alerts.push(msg);
        };

        $scope.closeAlert = function (index) {
            $scope.alerts.splice(index, 1);
        };

        $scope.loadCategories = function () {
            GameResource.teamcategories(function (data, status, headers, config) {
                $scope.categories = data;
            });
        };

        $scope.loadGames = function (categoryID) {
            GameResource.teamgames({id: categoryID}, 
                function (data, status, headers, config) {
                    $scope.games = data;
                }
            );
        };

        $scope.loadMyPlayer = function (categoryID) {
            UserResource.myteamplayer({ gameCategoryID: $scope.selectedCategory.gameCategoryID },
                function (data, status, headers, config) {
                    $scope.myplayer = data;
                    $scope.players.push({
                        playerID: data.competitorID,
                        player: data
                    });
                },
                function () {
                    $scope.myplayer = {};
                }
            );
        };

        $scope.addTeam = function () {
            $scope.team.players = [];
            $scope.team.games = [];
            angular.forEach($scope.games, function (game) {
                if (game.checked) {
                    $scope.team.games.push({
                        gameID: game.gameID
                    });
                }
            });
            angular.forEach($scope.players, function (player) {
                $scope.team.players.push({
                    playerID: player.playerID
                });
            });
            UserResource.addTeam($scope.team,
                function (data, status, headers, config) {
                    $scope.team = data.team;
                    $scope.addAlert({ type: 'success', msg: 'Team sucessfully added' });
                    $state.go('profile.teams');
                },
                function () {
                    $scope.addAlert({ type: 'error', msg: 'Error adding team' });
                }
            );
        };

        $scope.categoryChanged = function () {
            $scope.games = [];
            if ($scope.selectedCategory != null) {
                $scope.loadGames($scope.selectedCategory.gameCategoryID);
                $scope.loadMyPlayer($scope.selectedCategory.gameCategoryID);
            }
            else {
                $scope.team.gameID = null;
                $scope.availablePlayers = [];
                $scope.selectedPlayers = [];
            }
        }

        $scope.toggleGame = function (game) {
            game.checked = !game.checked;
        };

        $scope.searchPlayers = function () {
            if ($scope.selectedCategory != null) {
                $scope.availablePlayers = [];
                UserResource.searchplayers({ gameCategoryID: $scope.selectedCategory.gameCategoryID, search: $scope.searchQuery },
                    function (data, status, headers, config) {
                        angular.forEach(data, function (player) {
                            var idx = $scope.players.indexOf(player);
                            if (idx == -1) {
                                $scope.availablePlayers.push(player);
                            }
                        });
                    }
                );
            }
        }

        $scope.addPlayer = function (player, index) {
            $scope.players.push({
                playerID: player.competitorID,
                player:player
            });
            $scope.availablePlayers.splice(index, 1);
        }

        $scope.removePlayer = function (player, index) {
            $scope.players.splice(index, 1);
        }

        $scope.cancel = function () {
            window.history.back();
        };

        $scope.loadCategories();
    }]);