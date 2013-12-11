'use strict';
angular.module('Playground.match-add', ['ngResource', 'ui.router', 'ui.bootstrap.alert'])
    .filter('gamefull', function () {
        return function (game) {
            return game ? '(' + game.category.title + ') ' + game.title : '';
        };
    }).filter('playerfull', function () {
        return function (player) {
            return (player && player.user) ? '(' + player.user.firstName + ' ' + player.user.lastName + ') ' + player.name : '';
        };
    }).filter('userfull', function () {
        return function (player) {
            return (player && player.user) ? player.user.firstName + ' ' + player.user.lastName : '';
        };
    })
    .controller('MatchAddController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'UserResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, UserResource, enums) {
        $scope.games = [];
        $scope.selectedGame = null;

        /*
        $scope.myplayer = {};
        $scope.searchQuery = '';
        $scope.availablePlayers = [];
        $scope.selectedPlayers = [];
        $scope.alerts = [];
        */

        /*
        $scope.team.players.indexOf = function (obj) {
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
        */

        $scope.addAlert = function (msg) {
            $scope.alerts.push(msg);
        };

        $scope.closeAlert = function (index) {
            $scope.alerts.splice(index, 1);
        };

        $scope.loadGames = function () {
            UserResource.mycompeatinggames(function (data, status, headers, config) {
                $scope.games = data;
            });
        };

        /*
        $scope.loadMyPlayers = function () {
            UserResource.teamGames(function (data, status, headers, config) {
                $scope.games = data;
            });
        };
        */

        /*
        $scope.addTeam = function () {
            UserResource.addTeam($scope.team,
                function (data, status, headers, config) {
                    $scope.team = data.team;
                    $scope.addAlert({ type: 'success', msg: 'Team sucessfully added' });
                    $state.go('profile');
                },
                function () {
                    $scope.addAlert({ type: 'error', msg: 'Error adding team' });
                }
            );
        };

        $scope.gameChanged = function () {
            if ($scope.selectedGame != null) {
                $scope.team.gameID = $scope.selectedGame.gameID;
                UserResource.myteamplayer({ gameID: $scope.selectedGame.gameID },
                    function (data, status, headers, config) {
                        $scope.myplayer = data;
                        $scope.team.players.push({
                            playerID: data.competitorID,
                            player:data
                        });
                    },
                    function () {
                        $scope.myplayer = {};
                    }
                );
            }
            else {
                $scope.team.gameID = null;
                $scope.availablePlayers = [];
                $scope.selectedPlayers = [];
            }
        }

        $scope.searchPlayers = function () {
            if ($scope.selectedGame != null) {
                $scope.availablePlayers = [];
                UserResource.searchplayers({ gameID: $scope.selectedGame.gameID, search: $scope.searchQuery},
                    function (data, status, headers, config) {
                        angular.forEach(data, function (player) {
                            var idx = $scope.team.players.indexOf(player);
                            if (idx == -1) {
                                $scope.availablePlayers.push(player);
                            }
                        });
                    }
                );
            }
        }

        $scope.addPlayer = function (player, index) {
            $scope.team.players.push({
                playerID: player.competitorID,
                player:player
            });
            $scope.availablePlayers.splice(index, 1);
        }

        $scope.removePlayer = function (player, index) {
            $scope.team.players.splice(index, 1);
        }
        */

        $scope.cancel = function () {
            $state.go('profile');
        };

        $scope.loadGames();
    }]);