'use strict';
angular.module('Playground.team-edit', ['ngResource', 'ui.router', 'ui.bootstrap.alert'])
    .filter('playerfull', function () {
        return function (player) {
            return (player && player.user) ? '(' + player.user.firstName + ' ' + player.user.lastName + ') ' + player.name : '';
        };
    }).filter('playeruserfull', function () {
        return function (player) {
            return (player && player.user) ? player.user.firstName + ' ' + player.user.lastName : '';
        };
    })
    .controller('TeamEditController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'UserResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, UserResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.alerts = [];
        $scope.team = {
            competitorID: 0,
            games: []
        };
        $scope.players = [];
        $scope.myplayer = {};
        $scope.availablePlayers = [];
        $scope.searchQuery = '';

        $scope.getTeam = function () {
            UserResource.getUpdateTeam({ id: $stateParams.id }, function (data, status, headers, config) {
                $scope.team = data;
                $scope.players = data.players;
                $scope.team.players = [];
                angular.forEach($scope.players, function (player) {
                    if (player.player.isCurrentUserCompetitor) {
                        $scope.myplayer = player.player;
                    }
                });
            });
        };

        $scope.searchPlayers = function (page, count) {
            $scope.availablePlayers = [];
            UserResource.searchteamplayers({
                    id: $scope.team.competitorID,
                    page: page,
                    count: count,
                    search: $scope.searchQuery
                },
                function (data, status, headers, config) {
                    $scope.availablePlayers = data;
                }
            );
        };

        $scope.onAvailablePlayersPageSelect = function (page) {
            $scope.searchPlayers(page, 5);
        };

        $scope.removePlayer = function (player, index) {
            UserResource.deleteteamplayer({ teamID: player.teamID, playerID: player.playerID },
                function (data, status, headers, config) {
                    $scope.players.splice(index, 1);
                }
            );
        };

        $scope.addPlayer = function (player, index) {
            UserResource.addteamplayer({ teamID: $scope.team.competitorID, playerID: player.competitorID },
                function (data, status, headers, config) {
                    $scope.players.push({
                        teamID: $scope.team.competitorID,
                        playerID: player.competitorID,
                        player: player
                    });
                    $scope.searchPlayers($scope.availablePlayers.currentPage, 5);
                }
            );
        }

        $scope.updateTeam = function () {
            UserResource.updateTeam($scope.team,
                function (data, status, headers, config) {
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'Team sucessfully updated' });
                },
                function (err) {
                    $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Error updating team' });
                }
            );
        };

        $scope.cancel = function () {
            window.history.back();
        }

        $scope.getTeam();
    }]);