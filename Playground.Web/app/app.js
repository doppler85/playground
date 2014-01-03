/// <reference path="lib/angular/angular.js" />
'use strict';
var Playground = angular.module('Playground', [
    'ui.router',
    'Playground.imageupload',
    'Playground.matches',
    'Playground.players',
    'Playground.teams',
    'Playground.games',
    'Playground.main-menu',
    'Playground.home',
    'Playground.game-category',
    'Playground.game-list',
    'Playground.game-edit',
    'Playground.game-details',
    'Playground.game-category-details',
    'Playground.game-category-edit',
    'Playground.competition-type-list',
    'Playground.competition-type-add',
    'Playground.register',
    'Playground.login',
    'Playground.user-status',
    'Playground.user-profile',
    'Playground.competitors-list',
    'Playground.player-add',
    'Playground.player-edit',
    'Playground.team-add',
    'Playground.team-edit',
    'Playground.match-add',
    'Playground.match-list',
    'Playground.player-profile',
    'Playground.team-profile',
    'Playground.user-public-profile',
    'Playground.security.security-service',
    'Playground.security.interceptor',
    'Playground.security.retry-queue',
    'Playground.security.security-authorization'
]);

Playground.constant('settingss', {
}).constant('enums', {
    competitionType: {
        0: 'Individual',
        1: 'Team'
    },
    matchStatus: {
        0: 'Submited',
        1: 'Confirmed',
        2: 'Invalid'
    },
    gender: {
        0: 'Male',
        1: 'Female'
    }
});

Playground.
    config([
        '$stateProvider',
        '$urlRouterProvider',
        '$routeProvider',
        '$httpProvider',
        '$locationProvider',
        function ($stateProvider, $urlRouterProvider, $routeProvider, $httpProvider, $locationProvider) {
            $locationProvider.html5Mode(false)//.hashPrefix('!');
            delete $httpProvider.defaults.headers.common["X-Requested-With"];
            $urlRouterProvider.otherwise('/home');
        }]).
    config([
        '$compileProvider',
        '$stateProvider',
        '$injector',
        function ($compileProvider, $stateProvider, $injector) {
            $stateProvider.
                state('home', {
                    url: '/home',
                    templateUrl: 'app/home/home.tpl.html',
                    controller: 'HomeController',
                    data: { pageTitle: 'Playground' }
                }).state('game-categories', {
                    url: '/game/categories',
                    templateUrl: 'app/games/game-category.tpl.html',
                    controller: 'GameCategoryController',
                    data: { pageTitle: 'Games' },
                    resolve: {
                        authenticaated: function (securityAuthorization) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('game-category-edit', {
                    url: '/game/category/edit/:id',
                    templateUrl: 'app/games/game-category-edit.tpl.html',
                    controller: 'EditGameCategoryController',
                    data: { pageTitle: 'Edit game category' },
                    resolve: {
                        authenticaated: function (securityAuthorization) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('competition-types', {
                    url: '/competition-types',
                    templateUrl: 'app/games/competition-type-list.tpl.html',
                    controller: 'CompetitionTypeListController',
                    data: { pageTitle: 'Competition types' },
                    resolve: {
                        authenticaated: function (securityAuthorization) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('competition-type-add', {
                    url: '/competition-type/add',
                    templateUrl: 'app/games/competition-type-add.tpl.html',
                    controller: 'CompetitionTypeAddController',
                    data: { pageTitle: 'Add competition type' },
                    resolve: {
                        authenticaated: function (securityAuthorization) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('game-edit', {
                    url: '/game/edit/:id',
                    templateUrl: 'app/games/game-edit.tpl.html',
                    controller: 'EditGameController',
                    data: { pageTitle: 'Edit game' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('login', {
                    url: '/login',
                    templateUrl: 'app/user/login.tpl.html',
                    controller: 'LoginController',
                    data: { pageTitle: 'Login' }
                }).state('register', {
                    url: '/register',
                    templateUrl: 'app/user/register.tpl.html',
                    controller: 'RegisterController',
                    data: { pageTitle: 'Register' }
                }).state('public-profile', {
                    url: '/profile/:id',
                    templateUrl: 'app/user/user-public-profile.tpl.html',
                    controller: 'PublicProfileController',
                    data: { pageTitle: 'Public profile' }
                }).state('player-add', {
                    url: '/user/competition/player/add',
                    templateUrl: 'app/competition/player-add.tpl.html',
                    controller: 'PlayerAddController',
                    data: { pageTitle: 'Add player' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('player-edit', {
                    url: '/user/competition/player/edit/:id',
                    templateUrl: 'app/competition/player-edit.tpl.html',
                    controller: 'PlayerEditController',
                    data: { pageTitle: 'Edit player' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('team-add', {
                    url: '/competition/team/add',
                    templateUrl: 'app/competition/team-add.tpl.html',
                    controller: 'TeamAddController',
                    data: { pageTitle: 'Add team' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('team-edit', {
                    url: '/competition/team/edit/:id',
                    templateUrl: 'app/competition/team-edit.tpl.html',
                    controller: 'TeamEditController',
                    data: { pageTitle: 'Edit team' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('match-add', {
                    url: '/competition/match/add',
                    templateUrl: 'app/competition/match-edit.tpl.html',
                    controller: 'MatchAddController',
                    data: { pageTitle: 'Add match' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('match-list', {
                    url: '/competition/match/list',
                    templateUrl: 'app/competition/match-list.tpl.html',
                    controller: 'MatchListController',
                    data: { pageTitle: 'All matches' }
                }).state('player-profile', {
                    url: '/competition/player/:id',
                    templateUrl: 'app/competition/player-profile.tpl.html',
                    controller: 'PlayerProfileController',
                    data: { pageTitle: 'Player profile' }
                }).state('team-profile', {
                    url: '/competition/team/:id',
                    templateUrl: 'app/competition/team-profile.tpl.html',
                    controller: 'TeamProfileController',
                    data: { pageTitle: 'Team profile' }
                }).state('competitors-list', {
                    url: '/competition/list',
                    templateUrl: 'app/competition/competitors-list.tpl.html',
                    controller: 'CompetitorsListController',
                    data: { pageTitle: 'All competitors' }
                });
           }]).
    run([
        '$rootScope',
        '$location',
        '$state',
        function ($rootScope, $location, $state) { //*** Bootstrap the app, init config etc.
            $rootScope.ShowTitle = true;
            $rootScope.ShowMenu = true;

            $rootScope.IsActive = function (state) {
                return $state.is(state);
            }

            $rootScope.changeLocation = function (path) {
                $state.go(path);
            }
        }]).
    controller('AppCtrl', [
        '$scope',
        '$location',
        function AppCtrl($scope, $location) {
            $scope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                if (angular.isDefined(toState.data.pageTitle)) {
                    $scope.pageTitle = toState.data.pageTitle + ' | ngBoilerplate';
                }
            });
        }]);
