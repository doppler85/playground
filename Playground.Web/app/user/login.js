'use strict';
angular.module('Playground.login', ['ng', 'ngResource', 'ui.router']).
controller('LoginController', [
'$location',
'$http',
'$scope',
'$window',
'$state',
'$stateParams',
'$rootScope',
'security',
function ($location, $http, $scope, $window, $state, $stateParams, $rootScope, security) {
    $scope.pageTitle = $state.current.data ? $state.current.data.pageTitle : "Login";

    $scope.user = {
        grant_type: "password",
        username: '',
        password: '',
        rememberMe: false
    };

    $scope.authError = null;
    $scope.authReason = null;
    $scope.externallogins = null;

    $scope.login = function () {
        // Clear any previous security errors
        $scope.authError = null;

        var xsrf = $.param($scope.user);
        $http(
        {
            method: 'POST',
            url: '/Token',
            data: xsrf,
            headers: { 'Content-Type': 'application/x-www-form-urlencoded; ' }
            //headers: { 'Content-Type': 'application/json' }
        }).success(function (data, status, headers, config) {
            if (data.userName && data.access_token) {
                $scope.setAccessToken(data.access_token, $scope.user.rememberMe);
                $state.transitionTo('profile.info');
            } else {
                self.errors.push("An unknown error occurred.");
            }
        }).error(function (error) {
            console.log('something went wrong ');
        });

        /*
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
        */
    };

    $scope.loginExternal = function (loginprovider) {
        $window.sessionStorage["state"] = loginprovider.state;
        $window.sessionStorage["loginUrl"] = loginprovider.url;
        // IE doesn't reliably persist sessionStorage when navigating to another URL. Move sessionStorage temporarily
        // to localStorage to work around this problem.
        $scope.archiveSessionStorageToLocalStorage();

        $window.location = loginprovider.url;
    };

    $scope.LoadExternalLogins = function () {
        $http(
        {
            method: 'GET',
            url: '/api/account/externallogins',
            params: {
                returnurl: '/',
                generatestate: true
            }
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