'use strict';
angular.module('Playground.player-profile', [
    'ngResource',
    'ui.router',
    'Playground.matches',
    'Playground.players',
    'Playground.teams'])
    .config([
        '$stateProvider',
        function ($stateProvider) {
            $stateProvider
            .state('player-profile', {
                abstract: true,
                url: '/competition/player',
                templateUrl: 'app/templates/competition/player-profile.tpl.html',
                controller: 'PlayerProfileController',
                data: { pageTitle: 'Player profile' }
            }).state('player-profile.info', {
                url: '/info/:id',
                controller: 'PlayerProfileInfoController',
                templateUrl: 'app/templates/competition/player-profile-info.tpl.html'
            }).state('player-profile.teams', {
                url: '/teams/:id',
                controller: 'PlayerProfileTeamsController',
                templateUrl: 'app/templates/competition/player-profile-teams.tpl.html'
            }).state('player-profile.matches', {
                url: '/matches/:id',
                controller: 'PlayerProfileMatchesController',
                templateUrl: 'app/templates/competition/player-profile-matches.tpl.html'
            })
        }
    ])
    .controller('PlayerProfileController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    'CompetitorResource',
    function ($scope, $state, $stateParams, $rootScope, security, CompetitorResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.competitorID = $stateParams.id;
        $scope.tab = 'info';
    }])
    .controller('PlayerProfileInfoController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    'CompetitorResource',
    function ($scope, $state, $stateParams, $rootScope, security, CompetitorResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.competitorID = $stateParams.id;
        $scope.$parent.tab = 'info';
        $scope.playerStats = {};

        $scope.loadPlayerStats = function () {
            CompetitorResource.playerstats({ id: $stateParams.id },
                function (data, status, headers, config) {
                    $scope.playerStats = data;
                },
                function () {
                }
            );
        };

        $scope.loadPlayerStats();
    }])
    .controller('PlayerProfileTeamsController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    function ($scope, $state, $stateParams, $rootScope, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.competitorID = $stateParams.id;
        $scope.$parent.tab = 'teams';
    }])
    .controller('PlayerProfileMatchesController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    function ($scope, $state, $stateParams, $rootScope, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.competitorID = $stateParams.id;
        $scope.$parent.tab = 'matches';
    }]);