'use strict';
angular.module('Playground.home', ['ngResource', 'ui.router']).
    config(['$stateProvider', function config($stateProvider) {
        $stateProvider.state('home', {
            url: '/home',
            templateUrl: 'app/home/home.tpl.html',
            controller: 'HomeController',
            data: { pageTitle: 'Home page' }
        });
    }]).
    controller('HomeController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    function ($scope, $stateParams, $rootScope) {
    }]);