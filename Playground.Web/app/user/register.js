'use strict';
angular.module('Playground.register', ['ngResource', 'ui.router']).
controller('RegisterController', [
'$http',
'$scope',
'$state',
'$stateParams',
'$rootScope',
'security',
function ($http, $scope, $state, $stateParams, $rootScope, security) {
    $scope.pageTitle = $state.current.data.pageTitle;
    $scope.authError = null;

    $scope.registerModel = {
        grant_type: "password",
        userName: '',
        password: '',
        confirmPassword: ''
    };

    $scope.alerts = [];

    $scope.register = function () {

        $http(
        {
            method: 'POST',
            url: '/api/account/register',
            data: $scope.registerModel
        }).success(function (data, status, headers, config) {
            var xsrf = $.param($scope.registerModel);
            $http(
            {
                method: 'POST',
                url: '/Token',
                data: xsrf,
                headers: { 'Content-Type': 'application/x-www-form-urlencoded; ' }
            }).success(function (data, status, headers, config) {
                if (data.userName && data.access_token) {
                    $scope.setAccessToken(data.access_token, false);
                    $state.transitionTo('profile.info');
                } else {
                    $scope.addAlert($scope.alerts, { type: 'error', msg: "An unknown error occurred" });
                }
            }).error(function (error) {
                for (var g in error) {
                    $scope.addAlert($scope.alerts, { type: 'error', msg: error[g] });
                }
            });
        }).error(function (error) {
            for (var g in error) {
                $scope.addAlert($scope.alerts, { type: 'error', msg: error[g] });
            }
        });
        
        //// Try to login
        //security.register($scope.registerModel, $scope.user).then(function (data) {
        //    if (data.user) {
        //        $state.go('profile.info');
        //    }
        //    else {
        //        $scope.authError = "Erorr";
        //    }
        //}, function (x) {
        //    // If we get here then there was a problem with the login request to the server
        //    $scope.authError = "Server error, please try again later";
        //});
    };

    $scope.clearForm = function () {
        $scope.user = {};
    };

    $scope.cancelRegister = function () {
        $state.go('home');
    };
}]);