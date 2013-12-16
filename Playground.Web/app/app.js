/// <reference path="lib/angular/angular.js" />
'use strict';
var Playground = angular.module('Playground', [
    'ui.router',
    'Playground.imageupload',
    'Playground.main-menu',
    'Playground.home',
    'Playground.game-category',
    'Playground.game-list',
    'Playground.game-edit',
    'Playground.competition-type-list',
    'Playground.competition-type-add',
    'Playground.register',
    'Playground.login',
    'Playground.user-status',
    'Playground.user-profile',
    'Playground.player-add',
    'Playground.team-add',
    'Playground.match-add',
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
            $locationProvider.html5Mode(false).hashPrefix('!');
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
                    data: { pageTitle: 'Home page' }
                }).state('game-categories', {
                    url: '/game-categories',
                    templateUrl: 'app/games/game-category.tpl.html',
                    controller: 'GameCategoryController',
                    data: { pageTitle: 'Games' },
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
                }).state('game-details', {
                    url: '/game/details/:id',
                    templateUrl: 'app/games/game-details.tpl.html',
                    controller: 'DetailsGameController',
                    data: { pageTitle: 'Game details' }
                }).state('game-edit', {
                    url: '/game/edit/:id',
                    templateUrl: 'app/games/game-edit.tpl.html',
                    controller: 'EditGameController',
                    data: { pageTitle: 'Edit Game' },
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
                    data: { pageTitle: 'Login' }
                }).state('profile', {
                    url: '/profile',
                    templateUrl: 'app/user/user-profile.tpl.html',
                    controller: 'ProfileController',
                    data: { pageTitle: 'Profile' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('player-add', {
                    url: '/user/player/add',
                    templateUrl: 'app/user/player-edit.tpl.html',
                    controller: 'PlayerAddController',
                    data: { pageTitle: 'Player add' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('team-add', {
                    url: '/user/team/add',
                    templateUrl: 'app/user/team-edit.tpl.html',
                    controller: 'TeamAddController',
                    data: { pageTitle: 'Team add' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('match-add', {
                    url: '/user/match/add',
                    templateUrl: 'app/user/match-edit.tpl.html',
                    controller: 'MatchAddController',
                    data: { pageTitle: 'Match add' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
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
