'use strict';
angular.module('Playground.user-public-profile', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.tabs',
    'Playground.matches',
    'Playground.players'])
    .config([
        '$stateProvider',
        function ($stateProvider) {
            $stateProvider
            .state('public-profile', {
                url: '/userprofile',
                abstract: true,
                templateUrl: 'app/templates/user/user-public-profile.tpl.html',
                controller: 'PublicProfileController',
                data: { pageTitle: 'Public profile' }
            }).state('public-profile.info', {
                url: '/info/:id',
                controller: 'PublicProfileInfoController',
                templateUrl: 'app/templates/user/user-public-profile-info.tpl.html'
            }).state('public-profile.players', {
                url: '/players/:id',
                controller: 'PublicProfilePlayersController',
                templateUrl: 'app/templates/user/user-public-profile-players.tpl.html'
            }).state('public-profile.teams', {
                url: '/teams/:id',
                controller: 'PublicProfileTeamsController',
                templateUrl: 'app/templates/user/user-public-profile-teams.tpl.html'
            }).state('public-profile.matches', {
                url: '/matches/:id',
                controller: 'PublicProfileMatchesController',
                templateUrl: 'app/templates/user/user-public-profile-matches.tpl.html'
            })
        }
    ])
    .controller('PublicProfileController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    'UserResource',
    function ($scope, $state, $stateParams, $rootScope, security, UserResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.userID = $stateParams.id;
        $scope.tab = 'info';

    }])
    .controller('PublicProfileInfoController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    'UserResource',
    function ($scope, $state, $stateParams, $rootScope, security, UserResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.userID = $stateParams.id;
        $scope.$parent.tab = 'info';
        $scope.userStats = {};

        $scope.loadUserStats = function () {
            UserResource.getuserstats({ id: $stateParams.id },
                function (data, status, headers, config) {
                    $scope.userStats = data;
                },
                function () {
                }
            );
        };

        $scope.loadUserStats();
    }])
    .controller('PublicProfilePlayersController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    function ($scope, $state, $stateParams, $rootScope, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.userID = $stateParams.id;
        $scope.$parent.tab = 'players';
    }])
    .controller('PublicProfileTeamsController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    function ($scope, $state, $stateParams, $rootScope, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.userID = $stateParams.id;
        $scope.$parent.tab = 'teams';
    }])
    .controller('PublicProfileMatchesController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    function ($scope, $state, $stateParams, $rootScope, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.userID = $stateParams.id;
        $scope.$parent.tab = 'matches';
    }]);