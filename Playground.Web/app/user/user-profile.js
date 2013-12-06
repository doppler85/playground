'use strict';
angular.module('Playground.user-profile', ['ngResource', 'ui.router']).
controller('ProfileController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
function ($scope, $stateParams, $rootScope, security) {

    $scope.$watch(function () {
        $scope.isAuthenticated = security.isAuthenticated();
        return security.currentUser;
    }, function (currentUser) {
        $scope.currentUser = currentUser;
        $scope.isAuthenticated = security.isAuthenticated();
    });

    $scope.logout = function () {
        // Try to login
        security.logout("home");
    };
}]);