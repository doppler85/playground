'use strict';
angular.module('Playground.game-details', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.tabs',
    'Playground.matches',
    'Playground.players',
    'Playground.teams']).
controller('GameDetailsController', [
'$scope',
'$stateParams',
'$rootScope',
'$state',
'security',
'GameResource',
function ($scope, $stateParams, $rootScope, $state, security, GameResource) {
    $scope.pageTitle = $state.current.data.pageTitle;
    $scope.gameID = $stateParams.id;
    $scope.gameStats = {};

    $scope.loadGameStats = function () {
        GameResource.stats({ id: $stateParams.id },
            function (data, status, headers, config) {
                $scope.gameStats = data;
            },
            function () {
            }
        );
    };

    $scope.loadGameStats();
}]);;