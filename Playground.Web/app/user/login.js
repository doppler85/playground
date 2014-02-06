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
    $scope.alerts = [];
    $scope.externallogins = null;

    $scope.login = function () {
        $scope.authError = null;

        var xsrf = $.param($scope.user);
        $http(
        {
            method: 'POST',
            url: '/Token',
            data: xsrf,
            headers: { 'Content-Type': 'application/x-www-form-urlencoded; ' }
        }).success(function (data, status, headers, config) {
            if (data.userName && data.access_token) {
                $scope.setAccessToken(data.access_token, $scope.user.rememberMe);
                $state.transitionTo('profile.info');
            } else {
                $scope.addAlert($scope.alerts, { type: 'error', msg: "An unknown error occurred" });
            }
        }).error(function (error) {
            for (var g in error) {
                $scope.addAlert($scope.alerts, { type: 'error', msg: error[g] });
            }
        });
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
            $scope.addAlert($scope.alerts, { type: 'error', msg: error });
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