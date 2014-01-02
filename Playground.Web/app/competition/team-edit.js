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
            players: [],
            games: []
        };
        $scope.myplayer = {};
        $scope.availablePlayers = [];
        $scope.searchQuery = '';

        $scope.addAlert = function (msg) {
            $scope.alerts.push(msg);
        };

        $scope.closeAlert = function (index) {
            $scope.alerts.splice(index, 1);
        };

        $scope.getTeam = function () {
            UserResource.getUpdateTeam({ id: $stateParams.id }, function (data, status, headers, config) {
                $scope.team = data;
                angular.forEach(data.players, function (player) {
                    if (player.player.isCurrentUserCompetitor) {
                        $scope.myplayer = player.player;
                    }
                });
            });
        };

        $scope.searchPlayers = function () {
            $scope.availablePlayers = [];
            UserResource.searchteamplayers({ teamID: $scope.team.competitorID, search: $scope.searchQuery },
                function (data, status, headers, config) {
                    $scope.availablePlayers = data;
                }
            );
        };

        $scope.removePlayer = function (player, index) {
            UserResource.deleteteamplayer({ teamID: player.teamID, playerID: player.playerID },
                function (data, status, headers, config) {
                    $scope.team.players.splice(index, 1);
                }
            );
        };

        $scope.addPlayer = function (player, index) {
            UserResource.addteamplayer({ teamID: $scope.team.competitorID, playerID: player.competitorID },
                function (data, status, headers, config) {
                    $scope.team.players.push({
                        teamID: $scope.team.competitorID,
                        playerID: player.competitorID,
                        player: player
                    });
                    $scope.availablePlayers.splice(index, 1);
                }
            );
        }

        $scope.updateTeam = function () {
            UserResource.updateTeam($scope.team,
                function (data, status, headers, config) {
                    $scope.addAlert({ type: 'success', msg: 'Team sucessfully updated' });
                },
                function (err) {
                    $scope.addAlert({ type: 'error', msg: 'Error updating team' });
                }
            );
        };

        $scope.cancel = function () {
            window.history.back();
        }

        $scope.getTeam();
    }]);