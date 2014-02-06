'use strict';
angular.module('Playground.player-edit', ['ngResource', 'ui.router', 'ui.bootstrap.alert'])
    .filter('gamefull', function () {
        return function (game) {
            return game ? '(' + game.category.title + ') ' + game.title : '';
        };
    })
    .controller('PlayerEditController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'UserResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, UserResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.alerts = [];
        $scope.player = {
            competitorID: 0,
            games: []
        };

        $scope.getPlayer = function () {
            UserResource.getUpdatePlayer({ id: $stateParams.id }, function (data, status, headers, config) {
                $scope.player = data;
            });
        };

        $scope.updatePlayer = function () {
            UserResource.updatePlayer($scope.player, 
                function (data, status, headers, config) {
                    $scope.addAlert({ type: 'success', msg: 'Player sucessfully updated' });
                },
                function(err) {
                    $scope.addAlert({ type: 'error', msg: 'Error updating player' });
                }
            );
        };

        $scope.cancel = function () {
            window.history.back();
        }

        $scope.getPlayer();
    }]);