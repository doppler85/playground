'use strict';
angular.module('Playground.register-external', ['ng', 'ngResource', 'ui.router']).
controller('RegisterController', [
'$http',
'$scope',
'$state',
'$stateParams',
'$rootScope',
'$window',
'security',
function ($http, $scope, $state, $stateParams, $rootScope, $window, security) {
    $scope.pageTitle = $state.current.data.pageTitle;
    $scope.authError = null;

    $scope.registerModel = {
        userName: '',
    };

    $scope.register = function () {
        // Clear any previous security errors
        $scope.authError = null;

        // register local account
        $http(
            {
                method: 'POST',
                url: '/api/account/registerexternal',
                data: $scope.registerModel
            }).success(function (data, status, headers, config) {
                //$scope.externallogins = data;
                $window.location = $window.sessionStorage["loginUrl"]
            }).error(function (error) {
                $scope.clearAccessToken();
                $state.transitionTo('login');
            });

    };

    $scope.clearForm = function () {
        $scope.user = {};
    };

    $scope.cancelRegister = function () {
        $state.go('home');
    };
}]);