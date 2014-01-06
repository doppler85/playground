'use strict';
angular.module('Playground.login', ['ngResource', 'ui.router']).
controller('LoginController', [
'$scope',
'$state',
'$stateParams',
'$rootScope',
'security',
function ($scope, $state, $stateParams, $rootScope, security) {
    $scope.pageTitle = $state.current.data ? $state.current.data.pageTitle : "Login";

    $scope.user = {
        email: 'voja@playground.com',
        password: 'pass*123',
        rememberMe: true
    };

    $scope.authError = null;
    $scope.authReason = null;

    if (security.getLoginReason()) {
        $scope.authReason = (security.isAuthenticated()) ?
          "User not authenticated" :
          "User not autorized";
    }

    $scope.login = function () {
        // Clear any previous security errors
        $scope.authError = null;

        // Try to login
        security.login($scope.user).then(function (data) {
            if (data.user) {
                security.onSucessLogin(true);
            }
            else {
                $scope.authError = "Invalid credentials";
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