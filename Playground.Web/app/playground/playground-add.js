'use strict';
angular.module('Playground.playground-add', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.pagination']).
    controller('PlaygroundAddController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'PlaygroundResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, PlaygroundResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.playground = {};
        $scope.alerts = [];

        $scope.addPlayground = function () {
            angular.forEach($scope.games, function (game) {
                if (game.checked) {
                    $scope.player.games.push({
                        gameID: game.gameID
                    });
                }
            });
            PlaygroundResource.addplayground($scope.playground,
                function (data, status, headers, config) {
                    $scope.playground = data.playground;
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'Playground sucessfully added' });
                },
                function () {
                    $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Error adding playground' });
                }
            );
        };
    }]);