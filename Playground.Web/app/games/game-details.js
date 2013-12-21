'use strict';
angular.module('Playground.game-details', ['ngResource', 'ui.router', 'ui.bootstrap.tabs']).
controller('GameDetailsController', [
'$scope',
'$stateParams',
'$rootScope',
'$state',
'security',
function ($scope, $stateParams, $rootScope, $state, security) {
    $scope.pageTitle = $state.current.data.pageTitle;

}]);;