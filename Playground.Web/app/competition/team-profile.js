'use strict';
angular.module('Playground.team-profile', [
    'ngResource',
    'ui.router'])
    .config([
        '$stateProvider',
        function ($stateProvider) {
            $stateProvider
            .state('team-profile', {
                url: '/competition/team',
                abstract: true,
                templateUrl: 'app/templates/competition/team-profile.tpl.html',
                controller: 'TeamProfileController',
                data: { pageTitle: 'Team profile' }
            }).state('team-profile.info', {
                url: '/info/:id',
                controller: 'TeamProfileInfoController',
                templateUrl: 'app/templates/competition/team-profile-info.tpl.html'
            }).state('team-profile.players', {
                url: '/players/:id',
                controller: 'TeamProfilePlayersController',
                templateUrl: 'app/templates/competition/team-profile-players.tpl.html'
            }).state('team-profile.matches', {
                url: '/matches/:id',
                controller: 'TeamProfileMatchesController',
                templateUrl: 'app/templates/competition/team-profile-matches.tpl.html'
            })
        }
    ])
    .controller('TeamProfileController', [
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
    .controller('TeamProfileInfoController', [
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
        $scope.teamStats = {};

        $scope.loadTeamStats = function () {
            CompetitorResource.teamstats({ id: $stateParams.id },
                function (data, status, headers, config) {
                    $scope.teamStats = data;
                },
                function () {
                }
            );
        };

        $scope.loadTeamStats();
    }])
    .controller('TeamProfilePlayersController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'security',
    function ($scope, $state, $stateParams, $rootScope, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.competitorID = $stateParams.id;
        $scope.$parent.tab = 'players';
    }])
    .controller('TeamProfileMatchesController', [
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
