'use strict';
angular.module('Playground.game-category-details', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.tabs',
    'Playground.games',
    'Playground.players',
    'Playground.teams',
    'Playground.matches'
    ])
    .config([
        '$stateProvider',
        function ($stateProvider) {
            $stateProvider
            .state('game-category-details', {
                url: '/game/category/details',
                abstract: true,
                templateUrl: 'app/templates/games/game-category-details.tpl.html',
                controller: 'GameCategoryDetailsController',
                data: { pageTitle: 'Game category details' }
            }).state('game-category-details.info', {
                url: '/info/:id',
                controller: 'GameCategoryDetailsInfoController',
                templateUrl: 'app/templates/games/game-category-details-info.tpl.html'
            })
            .state('game-category-details.games', {
                url: '/games/:id',
                controller: 'GameCategoryDetailsGamesController',
                templateUrl: 'app/templates/games/game-category-details-games.tpl.html'
            }).state('game-category-details.players', {
                url: '/players/:id',
                controller: 'GameCategoryDetailsPlayersController',
                templateUrl: 'app/templates/games/game-category-details-players.tpl.html'
            }).state('game-category-details.teams', {
                url: '/teams/:id',
                controller: 'GameCategoryDetailsTeamsController',
                templateUrl: 'app/templates/games/game-category-details-teams.tpl.html'
            }).state('game-category-details.matches', {
                url: '/matches/:id',
                controller: 'GameCategoryDetailsMatchesController',
                templateUrl: 'app/templates/games/game-category-details-matches.tpl.html'
            })
        }
    ])
    .controller('GameCategoryDetailsController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    'GameCategoryResource',
    function ($scope, $stateParams, $rootScope, $state, security, GameCategoryResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.gameCategoryID = $stateParams.id;
        $scope.tab = 'info';
    }])
    .controller('GameCategoryDetailsInfoController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    'GameCategoryResource',
    function ($scope, $stateParams, $rootScope, $state, security, GameCategoryResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'info';
        $scope.gameCategoryID = $stateParams.id;
        $scope.gameCategoryStats = {};


        $scope.loadGameCategoryStats = function () {
            GameCategoryResource.stats({ id: $stateParams.id },
                function (data, status, headers, config) {
                    $scope.gameCategoryStats = data;
                },
                function () {
                }
            );
        };

        $scope.loadGameCategoryStats();
    }])
    .controller('GameCategoryDetailsGamesController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    'GameCategoryResource',
    function ($scope, $stateParams, $rootScope, $state, security, GameCategoryResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'games';
        $scope.gameCategoryID = $stateParams.id;
    }])
    .controller('GameCategoryDetailsPlayersController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    'GameCategoryResource',
    function ($scope, $stateParams, $rootScope, $state, security, GameCategoryResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'players';
        $scope.gameCategoryID = $stateParams.id;
    }])
    .controller('GameCategoryDetailsTeamsController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    'GameCategoryResource',
    function ($scope, $stateParams, $rootScope, $state, security, GameCategoryResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'teams';
        $scope.gameCategoryID = $stateParams.id;
    }])
    .controller('GameCategoryDetailsMatchesController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'security',
    'GameCategoryResource',
    function ($scope, $stateParams, $rootScope, $state, security, GameCategoryResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'matches';
        $scope.gameCategoryID = $stateParams.id;
    }]);