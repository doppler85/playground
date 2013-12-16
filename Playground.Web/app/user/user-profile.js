﻿'use strict';
angular.module('Playground.user-profile', ['ngResource', 'ui.router', 'ui.bootstrap.tabs'])
    .filter('userfull', function () {
        return function (user) {
            return user ? user.firstName + ' ' + user.lastName : '';
        };
    })
    .controller('ProfileController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    'security',
    'UserResource',
    'enums',
    function ($scope, $stateParams, $rootScope, security, UserResource, enums) {
        $scope.players = {};
        $scope.teams = {};
        $scope.matches = [];
        $scope.matchStatuses = enums.matchStatus;
        $scope.playerAlerts = [];
        $scope.teamAlerts = [];
        $scope.profile = {};
        $scope.automaticMatchConfirmations = [];
        $scope.automaticMatchConfirmationsUsers = [];
        $scope.searchQuery = '';
        $scope.addingconfirmation = false;
    
        $scope.$watch(function () {
            $scope.isAuthenticated = security.isAuthenticated();
            return security.currentUser;
        }, function (currentUser) {
            $scope.currentUser = currentUser;
            $scope.isAuthenticated = security.isAuthenticated();
        });

        $scope.getProfile = function () {
            UserResource.getprofile(function (data, status, headers, config) {
                $scope.profile = data;
            });
        };

        $scope.updateProfile = function () {
            UserResource.updateprofile($scope.profile, function (data, status, headers, config) {
                $scope.profile = data;
                security.refreshCurrentUser();
            });
        };

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

        $scope.confirmScore = function (match, score, $index, msgCollection) {
            UserResource.confirmscore({
                    matchID: match.matchID,
                    competitorID: score.competitorID,
                    confirmed: true
                },
                function (data, status, headers, config) {
                    score.confirmed = true;
                    match.status = data.status;
                }, function () {
                    msgCollection.push({ type: 'error', msg: 'Error deleting competitor: ' + competitor.name });
                });
        };

        $scope.loadAutomaticConfirmations = function () {
            UserResource.automaticmatchconfirmations(function(data, status, headers, config) {
                $scope.automaticMatchConfirmations = data;
            });
        };

        $scope.searchAutomaticConfirmationUsers = function () {
            UserResource.automaticmatchconfirmationsusers({ search: $scope.searchQuery }, function (data, status, headers, config) {
                $scope.automaticMatchConfirmationsUsers = data;
            });
        };

        $scope.addAutomaticConfirmation = function (user, index) {
            UserResource.addautomaticconfirmation(user,
                function (data, status, headers, config) {
                    $scope.automaticMatchConfirmationsUsers.splice(index, 1);
                    $scope.loadAutomaticConfirmations();
                } 
            );
        };

        $scope.deleteAutomaticConfirmation = function (user, index) {
            UserResource.deleteautomaticconfirmation({ confirmeeID: user.userID },
                function (data, status, headers, config) {
                    $scope.automaticMatchConfirmations.splice(index, 1);
                }
            );
        };

        $scope.toggleAddConfirmation = function(show) {
            $scope.addingconfirmation = show;
            if (!show) {
                $scope.automaticMatchConfirmationsUsers = [];
            }
        };

        $scope.loadPlayers();
        $scope.loadTeams();
        $scope.loadMatches();
        $scope.getProfile();
        $scope.loadAutomaticConfirmations();
    }]);