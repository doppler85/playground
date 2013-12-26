'use strict';
angular.module('Playground.player-profile', ['ngResource', 'ui.router', 'ui.bootstrap.tabs']).
controller('PlayerProfileController', [
'$scope',
'$state',
'$stateParams',
'$rootScope',
'security',
'CompetitorResource',
function ($scope, $state, $stateParams, $rootScope, security, CompetitorResource) {
    $scope.pageTitle = $state.current.data.pageTitle;
    $scope.competitorID = $stateParams.id;
    $scope.playerStats = {};

    $scope.loadPlayerStats = function () {
        CompetitorResource.playerstats({ id: $stateParams.id },
            function (data, status, headers, config) {
                $scope.playerStats = data;
            },
            function () {
            }
        );
    };

    $scope.loadPlayerStats();
}]);;