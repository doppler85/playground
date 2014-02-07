'use strict';
angular.module('Playground.register-external', ['ng', 'ngResource', 'ui.router']).
controller('RegisterExternalController', [
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

    $scope.alerts = [];

    $scope.register = function () {

        security.registerExternal($scope.registerModel).then(
            function (data) {
                $window.location = $window.sessionStorage["loginUrl"]
            },
            function (error) {
                var msgs = $scope.getErrorsFromResponse(error.data);
                for (var key in msgs) {
                    $scope.addAlert($scope.alerts, { type: 'error', msg: msgs[key] });
                }
            }
        );
    };

    $scope.clearForm = function () {
        $scope.user = {};
    };

    $scope.cancelRegister = function () {
        $state.go('home');
    };
}]);