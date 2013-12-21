'use strict';
angular.module('Playground.home', ['ngResource', 'ui.router']).
    controller('HomeController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'HomeResource',
    function ($scope, $stateParams, $rootScope, $state, HomeResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.matches = [];
        $scope.competitors = [];

        $scope.loadMatches = function () {
            HomeResource.lastMatches({ count: 5 }, function (data, status, headers, config) {
                $scope.matches = data;
            });
        }

        $scope.loadCompetitors = function () {
            HomeResource.lastCompetitors({ count: 5 }, function (data, status, headers, config) {
                $scope.competitors = data;
            });
        }

        $scope.loadMatches();
        $scope.loadCompetitors();
    }]);