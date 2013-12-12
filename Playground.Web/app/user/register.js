'use strict';
angular.module('Playground.register', ['ngResource', 'ui.router']).
controller('RegisterController', [
'$scope',
'$stateParams',
'$rootScope',
'$state',
'security',
function ($scope, $stateParams, $rootScope, $state, security) {
    $scope.authError = null;

    $scope.registerModel = {
        firstName: '',
        lastName: '',
        userName: '',
        password: '',
        confirmPassword: ''
    };

    if (security.getLoginReason()) {
        $scope.authReason = (security.isAuthenticated()) ?
          "User not authenticated" :
          "User not autorized";
    }

    $scope.register = function () {
        // Clear any previous security errors
        $scope.authError = null;

        // Try to login
        security.register($scope.registerModel, $scope.user).then(function (data) {
            if (data.user) {
                $state.go('profile');
            }
            else {
                $scope.authError = "Erorr";
            }
        }, function (x) {
            // If we get here then there was a problem with the login request to the server
            $scope.authError = "Server error, please try again later";
        });
    };

    $scope.clearForm = function () {
        $scope.user = {};
    };

    $scope.cancelRegister = function () {
        $state.go('home');
    };
}]);