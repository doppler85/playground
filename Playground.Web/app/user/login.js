'use strict';
angular.module('Playground.login', ['ngResource', 'ui.router']).
controller('LoginController', [
'$location',
'$http',
'$scope',
'$state',
'$stateParams',
'$rootScope',
'security',
function ($location, $http, $scope, $state, $stateParams, $rootScope, security) {
    $scope.pageTitle = $state.current.data ? $state.current.data.pageTitle : "Login";

    $scope.user = {
        email: 'voja@playground.com',
        password: 'pass*123',
        rememberMe: true
    };

    $scope.authError = null;
    $scope.authReason = null;
    $scope.externallogins = null;

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

    $scope.loginExternal = function (loginprovider) {
        sessionStorage["state"] = loginprovider.state;
        sessionStorage["loginUrl"] = loginprovider.url;
        // IE doesn't reliably persist sessionStorage when navigating to another URL. Move sessionStorage temporarily
        // to localStorage to work around this problem.
        $scope.archiveSessionStorageToLocalStorage();

        window.location = loginprovider.url;
    };

    $scope.LoadExternalLogins = function () {
        $http(
        {
            method: 'GET',
            url: '/api/account/externallogins',
            params: {
                returnurl: '/',
                generatestate: true
            },
        }).success(function (data, status, headers, config) {
            $scope.externallogins = data;
        }).error(function (error) {
            console.log('something went wrong ');
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

    $scope.LoadExternalLogins();
}]);