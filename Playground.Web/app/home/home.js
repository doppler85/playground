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

        $scope.loadMatches = function () {
            HomeResource.lastMatches({ count: 5, page: 1 }, function (data, status, headers, config) {
                $scope.matches = data.items;
                $scope.totalMatches = data.totalItems;
            });
        }

        $scope.loadCompetitors = function () {
            HomeResource.lastCompetitors({ count: 5 }, function (data, status, headers, config) {
                $scope.competitors = data;
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