'use strict';
angular.module('Playground.register', ['ngResource', 'ui.router']).
controller('RegisterController', [
'$scope',
'$state',
'$stateParams',
'$rootScope',
'security',
function ($scope, $state, $stateParams, $rootScope, security) {
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

        security.register($scope.registerModel).then(
            function (data) {
                security.login($scope.registerModel).then(
                    function (data) {
                        if (data.userName && data.access_token) {
                            $scope.setAccessToken(data.access_token, false);
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
            },
            function (error) {
                for (var g in error) {
                    $scope.addAlert($scope.alerts, { type: 'error', msg: error[g] });
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