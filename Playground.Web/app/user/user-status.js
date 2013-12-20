'use strict';
angular.module('Playground.user-status', ['ngResource', 'ui.router']).
controller('UserStatusController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
'UserResource',
function ($scope, $stateParams, $rootScope, security, UserResource) {
    $scope.$watch(function () {
        $scope.isAuthenticated = security.isAuthenticated();
        return security.currentUser;
    }, function (currentUser) {
        $scope.currentUser = currentUser;
        $scope.isAuthenticated = security.isAuthenticated();
        if ($scope.isAuthenticated) {
            UserResource.getprofile(function (data, status, headers, config) {
                $scope.profile = data;
            });
        } else {
            $scope.profile = {};
        }
    });

    $scope.logout = function () {
        security.logout('home');
    };

    //$scope.$digest();
}]);;