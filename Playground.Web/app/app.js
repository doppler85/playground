/// <reference path="lib/angular/angular.js" />
'use strict';
var Playground = angular.module('Playground', [
    'ui.router',
    'Playground.main-menu',
    'Playground.home',
    'Playground.game-category',
    'Playground.game-list',
    'Playground.game-edit',
    'Playground.login',
    'Playground.user-status',
    'Playground.user-profile',
    'Playground.security.security-service',
    'Playground.security.interceptor',
    'Playground.security.retry-queue',
    'Playground.security.security-authorization',
]);

Playground.constant('settingss', {
}).constant('enums', {
    competitionType: { 0: 'Individual', 1: 'Team' }
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
                });
           }]).
    run([
        '$rootScope',
        '$location',
        '$state',
        function ($rootScope, $location, $state) { //*** Bootstrap the app, init config etc.
            $rootScope.ShowTitle = true;
            $rootScope.ShowMenu = true;

            $rootScope.IsActive = function (path) {
                return path === $location.path();
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
