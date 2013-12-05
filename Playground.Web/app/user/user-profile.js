'use strict';
angular.module('Playground.user-profile', ['ngResource', 'ui.router']).
controller('ProfileController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
function ($scope, $stateParams, $rootScope, security) {
    $scope.user = {};

    $scope.authError = null;

    $scope.logout = function () {
        // Clear any previous security errors
        $scope.authError = null;

        // Try to login
        security.logout("home");
    };
}]);