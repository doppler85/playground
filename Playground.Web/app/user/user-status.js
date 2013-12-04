'use strict';
angular.module('Playground.user-status', ['ngResource', 'ui.router']).
controller('UserStatusController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
function ($scope, $stateParams, $rootScope, security) {
    console.log('ima user status controllerrrrrrr');

    $scope.init = function () {
        $scope.currentUser = security.requestCurrentUser();
        $scope.isAuthenticated = false;

        $scope.$watch(function () {
            $scope.isAuthenticated = security.isAuthenticated();
            return security.currentUser;
        }, function (currentUser) {
            $scope.currentUser = currentUser;
            $scope.isAuthenticated = security.isAuthenticated();
        });
    }
}]);;