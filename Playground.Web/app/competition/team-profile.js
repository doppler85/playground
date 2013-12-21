'use strict';
angular.module('Playground.team-profile', ['ngResource', 'ui.router', 'ui.bootstrap.tabs']).
controller('TeamProfileController', [
'$scope',
'$state',
'$stateParams',
'$rootScope',
'security',
'UserResource',
function ($scope, $state, $stateParams, $rootScope, security, UserResource) {
    $scope.pageTitle = $state.current.data.pageTitle;

}]);;