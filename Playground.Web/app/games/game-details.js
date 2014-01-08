'use strict';
angular.module('Playground.game-details', [
    'ngResource',
    'ui.router',
    'Playground.matches',
    'Playground.players',
    'Playground.teams'])
    .config([
        '$stateProvider',
        function ($stateProvider) {
            $stateProvider
            .state('game-details', {
                url: '/game/details',
                abstract: true,
                templateUrl: 'app/templates/games/game-details.tpl.html',
                controller: 'GameDetailsController',
                data: { pageTitle: 'Game details' },
                resolve: {
                    currentUser: function (securityAuthorization) {
                        return securityAuthorization.requireCurrentUser();
                    }
                }
            }).state('game-details.info', {
                url: '/info/:id',
                controller: 'GameDetailsInfoController',
                templateUrl: 'app/templates/games/game-details-info.tpl.html'
            }).state('game-details.players', {
                url: '/players/:id',
                controller: 'GameDetailsPlayersController',
                templateUrl: 'app/templates/games/game-details-players.tpl.html'
            }).state('game-details.teams', {
                url: '/teams/:id',
                controller: 'GameDetailsTeamsController',
                templateUrl: 'app/templates/games/game-details-teams.tpl.html'
            }).state('game-details.matches', {
                url: '/matches/:id',
                controller: 'GameDetailsMatchesController',
                templateUrl: 'app/templates/games/game-details-matches.tpl.html'
            })
        }
    ])
    .controller('GameDetailsController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    'GameResource',
    function ($scope, $stateParams, $rootScope, $state, security, GameResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.gameID = $stateParams.id;
        $scope.tab = 'info';

    }])
    .controller('GameDetailsInfoController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    'GameResource',
    function ($scope, $stateParams, $rootScope, $state, security, GameResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'info';
        $scope.gameID = $stateParams.id;
        $scope.gameStats = {};

        $scope.loadGameStats = function () {
            GameResource.stats({ id: $stateParams.id },
                function (data, status, headers, config) {
                    $scope.gameStats = data;
                },
                function () {
                }
            );
        };

        $scope.loadGameStats();
    }])
    .controller('GameDetailsPlayersController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    function ($scope, $stateParams, $rootScope, $state, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'players';
        $scope.gameID = $stateParams.id;
    }])
    .controller('GameDetailsTeamsController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    function ($scope, $stateParams, $rootScope, $state, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'teams';
        $scope.gameID = $stateParams.id;
    }])
    .controller('GameDetailsMatchesController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    function ($scope, $stateParams, $rootScope, $state, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'matches';
        $scope.gameID = $stateParams.id;
    }]);