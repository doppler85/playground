/// <reference path="lib/angular/angular.js" />
'use strict';
var Playground = angular.module('Playground', [
    'ngRoute',
    'Playground.home',
    'Playground.game-category',
    'Playground.game-list',
    'Playground.game-edit',
    'Playground.login',
    'Playground.security.service',
    'Playground.security.interceptor',
    'Playground.security.retry-queue'
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
        function ($compileProvider) {
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
                $state.transitionTo(path);
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
