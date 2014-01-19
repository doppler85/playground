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
        $scope.totalMatches = 0;
        $scope.competitors = [];
        $scope.totalCompetitors = 0;

        $scope.loadMatches = function () {
            HomeResource.lastmatches({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.matches = data.items;
                $scope.totalMatches = data.totalItems;
            });
        }

        $scope.loadCompetitors = function () {
            HomeResource.lastcompetitors({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.competitors = data.items;
                $scope.totalCompetitors = data.totalItems;
            });
        }

        $scope.loadMatches();
        $scope.loadCompetitors();

        var hub = $.connection.livescores;

        hub.client.refreshMatches = function (totalMathces) {
            $scope.loadMatches();
        };

        $.connection.hub.start()
        .then(function () {
            hub.server.getTotalMatches();
        })
        .done(function () {
            console.log('connection started');
        }).fail(function () {
            console.log('failure starting hub');
        });
    }]);