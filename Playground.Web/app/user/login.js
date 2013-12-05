'use strict';
angular.module('Playground.login', ['ngResource', 'ui.router']).
controller('LoginController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
function ($scope, $stateParams, $rootScope, security) {
    $scope.user = {
        email: 'admin@playground.com',
        password: 'admin123!',
        rememberMe: true
    };

    $scope.authError = null;

    $scope.login = function () {
        // Clear any previous security errors
        $scope.authError = null;

        // Try to login
        security.login($scope.user).then(function (data) {
            // TODO: make this more ellegant
            //if (!loggedIn) {
                // If we get here then the login failed due to bad credentials
                //$scope.authError = "Invalid credentials";
            //}
            if (data.user) {
                security.onSucessLogin(true);
            }
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