'use strict';
angular.module('Playground.user-public-profile', ['ngResource', 'ui.router', 'ui.bootstrap.tabs', 'Playground.matches', 'Playground.players']).
controller('PublicProfileController', [
'$scope',
'$state',
'$stateParams',
'$rootScope',
'security',
'UserResource',
function ($scope, $state, $stateParams, $rootScope, security, UserResource) {
    $scope.pageTitle = $state.current.data.pageTitle;
    $scope.userID = $stateParams.id;

}]);;