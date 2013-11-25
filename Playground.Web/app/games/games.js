'use strict';
angular.module('Playground.games', ['ngResource', 'ui.router']).
    config(['$stateProvider', function config($stateProvider) {
        $stateProvider.state('games', {
            url: '/games',
            templateUrl: 'app/games/games.tpl.html',
            controller: 'GameController',
            data: { pageTitle: 'Games' }
        });
    }]).
    controller('GameController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    function ($scope, $stateParams, $rootScope) {
    }]);