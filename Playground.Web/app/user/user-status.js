'use strict';
angular.module('Playground.user-status', ['ngResource', 'ui.router']).
controller('UserStatusController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
function ($scope, $stateParams, $rootScope, security) {
    $scope.init = function () {
    }

    $scope.$watch(function () {
        $scope.isAuthenticated = security.isAuthenticated();
        return security.currentUser;
    }, function (currentUser) {
        $scope.currentUser = currentUser;
        $scope.isAuthenticated = security.isAuthenticated();
    });

    //$scope.$digest();
}]);;