'use strict';
angular.module('Playground.login', ['ngResource', 'ui.router']).
controller('LoginController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
function ($scope, $stateParams, $rootScope, security) {
    $scope.user = {};

    $scope.authError = null;

    $scope.login = function () {
        // Clear any previous security errors
        $scope.authError = null;

        // Try to login
        security.login($scope.user).then(function (data) {
            //if (!loggedIn) {
                // If we get here then the login failed due to bad credentials
                //$scope.authError = "Invalid credentials";
            //}
            console.log(data);
        }, function (x) {
            // If we get here then there was a problem with the login request to the server
            $scope.authError = "Server error, please try again later";
        });
    };

    $scope.clearForm = function () {
        $scope.user = {};
    };

    $scope.cancelLogin = function () {
        security.cancelLogin();
    };

    $scope.toggleRememberMe = function () {
        $scope.user.rememberMe = !$scope.user.rememberMe;
    };
}]);