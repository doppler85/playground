'use strict';
angular.module('Playground.game-details', ['ngResource', 'ui.router', 'ui.bootstrap.tabs', 'Playground.matches', 'Playground.players']).
controller('GameDetailsController', [
'$scope',
'$stateParams',
'$rootScope',
'$state',
'security',
function ($scope, $stateParams, $rootScope, $state, security) {
    $scope.pageTitle = $state.current.data.pageTitle;
    $scope.gameID = $stateParams.id;

}]);;