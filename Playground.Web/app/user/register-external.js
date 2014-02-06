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
                if (error.data) {
                    if (error.data.modelState) {
                        for (var m in error.data.modelState) {
                            $scope.addAlert($scope.alerts, { type: 'error', msg: m });
                        }
                    }
                    else {
                        $scope.addAlert($scope.alerts, { type: 'error', msg: error.data });
                    }
                }
                else {
                    $scope.addAlert($scope.alerts, { type: 'error', msg: "Unknown error happened" });
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