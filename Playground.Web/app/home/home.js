'use strict';
angular.module('Playground.home', ['ngResource', 'ui.router']).
    controller('HomeController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    function ($scope, $stateParams, $rootScope) {
        console.log('i am home controller');
    }]);