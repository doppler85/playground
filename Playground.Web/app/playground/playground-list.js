'use strict';
angular.module('Playground.playground-list', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.pagination']).
    controller('PlaygroundListController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'PlaygroundResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, PlaygroundResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.playgrounds = {};

        $scope.loadPlaygrounds = function (page) {
            PlaygroundResource.getplaygrounds({
                page: page,
                count: 5
            },
            function (data) {
                $scope.playgrounds = data;
            });
        };
        
        $scope.onPlaygroundPageSelect = function (page) {
            $scope.loadPlaygrounds(page);
        };

        $scope.loadPlaygrounds(1);
    }]);