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
    $scope.playerAlerts = [];
    $scope.teamAlerts = [];

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

    $scope.deleteCompetitor = function (competitor, collection, index, msgCollection) {
        UserResource.deletecompetitor({ id: competitor.competitorID },
            function (data, status, headers, config) {
                collection.splice(index, 1);
            }, function () {
                msgCollection.push({ type:'error', msg:'Error deleting competitor: ' + competitor.name });
            });
    };

    $scope.addAlert = function (collection, msg) {
        collection.push(msg);
    };

    $scope.closeAlert = function (collection, index) {
        collection.splice(index, 1);
    };

    $scope.loadPlayers();
    $scope.loadTeams();
    $scope.loadMatches();
}]);