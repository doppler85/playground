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
    'Playground.game-edit',
    'Playground.game-details',
    'Playground.game-category-details',
    'Playground.game-category-edit',
    'Playground.competition-type-list',
    'Playground.competition-type-add',
    'Playground.competition-type-edit',
    'Playground.register',
    'Playground.login',
    'Playground.user-list',
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

Playground.constant('settings', {
    debug: true
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
                    templateUrl: 'app/templates/home/home.tpl.html',
                    controller: 'HomeController',
                    data: { pageTitle: 'Playground' },
                    resolve: {
                        currentUser: function (securityAuthorization) {
                            return securityAuthorization.requireCurrentUser();
                        }
                    }
                }).state('game-categories', {
                    url: '/game/categories',
                    templateUrl: 'app/templates/games/game-category-list.tpl.html',
                    controller: 'GameCategoryController',
                    data: { pageTitle: 'Game categories' },
                    resolve: {
                        authenticaated: function (securityAuthorization) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('competition-types', {
                    url: '/competition-types',
                    templateUrl: 'app/templates/games/competition-type-list.tpl.html',
                    controller: 'CompetitionTypeListController',
                    data: { pageTitle: 'Competition types' },
                    resolve: {
                        authenticaated: function (securityAuthorization) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('competition-type-add', {
                    url: '/competition-type/add',
                    templateUrl: 'app/templates/games/competition-type-add.tpl.html',
                    controller: 'CompetitionTypeAddController',
                    data: { pageTitle: 'Add competition type' },
                    resolve: {
                        authenticaated: function (securityAuthorization) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('competition-type-edit', {
                    url: '/competition-type/edit/:id',
                    templateUrl: 'app/templates/games/competition-type-edit.tpl.html',
                    controller: 'CompetitionTypeEditController',
                    data: { pageTitle: 'Edit competition type' },
                    resolve: {
                        authenticaated: function (securityAuthorization) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('game-edit', {
                    url: '/game/edit/:id',
                    templateUrl: 'app/templates/games/game-edit.tpl.html',
                    controller: 'EditGameController',
                    data: { pageTitle: 'Edit game' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('user-list', {
                    url: '/user/list',
                    templateUrl: 'app/templates/user/user-list.tpl.html',
                    controller: 'UserListController',
                    data: { pageTitle: 'Users' },
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
                }).state('player-add', {
                    url: '/user/competition/player/add',
                    templateUrl: 'app/templates/competition/player-add.tpl.html',
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
                    templateUrl: 'app/templates/competition/team-add.tpl.html',
                    controller: 'TeamAddController',
                    data: { pageTitle: 'Add team' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('team-edit', {
                    url: '/competition/team/edit/:id',
                    templateUrl: 'app/templates/competition/team-edit.tpl.html',
                    controller: 'TeamEditController',
                    data: { pageTitle: 'Edit team' },
                    resolve: {
                        authenticaated: function (securityAuthorization, $location, $state) {
                            return securityAuthorization.requireAuthenticatedUser();
                        }
                    }
                }).state('match-add', {
                    url: '/competition/match/add',
                    templateUrl: 'app/competition/match-add.tpl.html',
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
                    data: { pageTitle: 'All matches' },
                    resolve: {
                        currentUser: function (securityAuthorization) {
                            return securityAuthorization.requireCurrentUser();
                        }
                    }
                }).state('competitors-list', {
                    url: '/competition/list',
                    templateUrl: 'app/competition/competitors-list.tpl.html',
                    controller: 'CompetitorsListController',
                    data: { pageTitle: 'All competitors' },
                    resolve: {
                        currentUser: function (securityAuthorization) {
                            return securityAuthorization.requireCurrentUser();
                        }
                    }
                });
           }]).
    run([
        '$rootScope',
        '$location',
        '$state',
        'settings',
        function ($rootScope, $location, $state, settings) { //*** Bootstrap the app, init config etc.
            $rootScope.ShowTitle = true;
            $rootScope.ShowMenu = true;
            $rootScope.settings = settings;

            $rootScope.IsActive = function (state) {
                return $state.is(state);
            }

            $rootScope.changeLocation = function (path) {
                $state.go(path);
            }

            $rootScope.getKey = function (key) {
                return parseInt(key, 10);
            };

            // alserts
            $rootScope.addAlert = function (arr, msg) {
                arr.push(msg);
            };

            $rootScope.closeAlert = function (arr, index) {
                arr.splice(index, 1);
            };

            // validation
            $rootScope.getCssClasses = function (ngModelContoller) {
                return {
                    error: ngModelContoller.$invalid && ngModelContoller.$dirty,
                    success: ngModelContoller.$valid && ngModelContoller.$dirty
                };
            };

            $rootScope.showError = function (ngModelController, error) {
                return ngModelController.$error[error];
            };

            $rootScope.canSave = function (ngFormController) {
                return ngFormController.$valid && ngFormController.$dirty;
            };

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
