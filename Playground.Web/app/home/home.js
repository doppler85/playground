'use strict';
angular.module('Playground.home', ['ngResource', 'ui.router', 'google-maps', 'ngAnimate']).
    controller('HomeController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'HomeResource',
    'CompetitorResource',
    function ($scope, $stateParams, $rootScope, $state, homeResource, competitorResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.matches = [];
        $scope.totalMatches = 0;
        $scope.competitors = [];
        $scope.hallOfFame = [];
        $scope.hallOfShame = [];
        $scope.stats = {};

        $scope.totalCompetitors = 0;
        $scope.map = {
            center: {
                latitude: 45.254302,
                longitude: 19.842915
            },
            zoom: 15
        };

        $scope.loadStats = function () {
            homeResource.getstats(
                function (data, status, headers, config) {
                    $scope.stats = data;
                }
            );
        }

        $scope.loadStats();

        /*
        $scope.loadMatches = function () {
            homeResource.lastmatches({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.matches = data.items;
                $scope.totalMatches = data.totalItems;
            });
        }

        $scope.loadCompetitors = function () {
            homeResource.lastcompetitors({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.competitors = data.items;
                $scope.totalCompetitors = data.totalItems;
            });
        }

        $scope.loadHallOfFame = function () {
            competitorResource.halloffame({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.hallOfFame = data.items;
            });
        }

        $scope.loadHallOFShame = function () {
            competitorResource.hallofshame({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.hallOfShame = data.items;
            });
        }
        
        $scope.loadMatches();
        $scope.loadCompetitors();
        $scope.loadHallOfFame();
        $scope.loadHallOFShame();
        */

        var hub = $.connection.livescores;
        hub.client.refreshMatches = function (match, totalMathces) {
            $scope.totalMatches = totalMathces;
            $scope.matches.unshift(match);

            if ($scope.matches.length > 5) {
                $scope.matches.pop();
            }

            $scope.$digest();

            // reload top lists
            $scope.loadHallOfFame();
            $scope.loadHallOFShame();
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