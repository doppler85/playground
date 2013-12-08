'use strict';
angular.module('Playground.user-profile', ['ngResource', 'ui.router']).
controller('ProfileController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
'UserResource',
'enums',
function ($scope, $stateParams, $rootScope, security, UserResource, enums) {
    $scope.players = {};
    $scope.teams = {};
    $scope.matches = {};
    $scope.matchStatuses = enums.matchStatus;

    $scope.$watch(function () {
        $scope.isAuthenticated = security.isAuthenticated();
        return security.currentUser;
    }, function (currentUser) {
        $scope.currentUser = currentUser;
        $scope.isAuthenticated = security.isAuthenticated();
    });

    $scope.logout = function () {
        // Try to login
        security.logout("home");
    };

    $scope.loadPlayers = function () {
        UserResource.allPlayers(function (data, status, headers, config) {
            $scope.players = data;
        });
    }

    $scope.loadTeams = function () {
        UserResource.allTeams(function (data, status, headers, config) {
            $scope.teams = data;
        });
    }

    $scope.loadMatches = function () {
        UserResource.allMatches({count: 5}, function (data, status, headers, config) {
            $scope.matches = data;
        });
    }

    $scope.loadPlayers();
    $scope.loadTeams();
    $scope.loadMatches();
}]);