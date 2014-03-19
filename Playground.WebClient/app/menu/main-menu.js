'use strict';
angular.module('Playground.main-menu', ['ngResource', 'ui.router']).
controller('MainMenuController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
function ($scope, $stateParams, $rootScope, security) {
    $scope.isAuthenticated = false;
    $scope.currentUser = {};

    $scope.$watch(function () {
        $scope.isAuthenticated = security.isAuthenticated();
        return security.currentUser;
    }, function (currentUser) {
        $scope.currentUser = currentUser;
    });
}]);