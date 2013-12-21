'use strict';
angular.module('Playground.player-profile', ['ngResource', 'ui.router', 'ui.bootstrap.tabs']).
controller('PlayerProfileController', [
'$scope',
'$state',
'$stateParams',
'$rootScope',
'security',
'UserResource',
function ($scope, $state, $stateParams, $rootScope, security, UserResource) {
    $scope.pageTitle = $state.current.data.pageTitle;

}]);;