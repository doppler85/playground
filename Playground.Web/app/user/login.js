'use strict';
angular.module('Playground.login', ['ng', 'ngResource', 'ui.router']).
controller('LoginController', [
'$location',
'$scope',
'$window',
'$state',
'$stateParams',
'$rootScope',
'security',
function ($location, $scope, $window, $state, $stateParams, $rootScope, security) {
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
        $scope.alerts = [];
        
        security.login($scope.user).then(
            function (data) {
                if (data.userName && data.access_token) {
                    $scope.setAccessToken(data.access_token, $scope.user.rememberMe);
                    $state.transitionTo('profile.info');
                } else {
                    $scope.addAlert($scope.alerts, { type: 'error', msg: "An unknown error occurred" });
                }
            },
            function (error) {
                for (var g in error) {
                    $scope.addAlert($scope.alerts, { type: 'error', msg: error[g] });
                }
            }
        );
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
        security.getExternalLogins().then(
            function (data) {
                $scope.externallogins = data;
            },
            function (error) {
                $scope.addAlert($scope.alerts, {type: 'error', msg: error});
            }
        );
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