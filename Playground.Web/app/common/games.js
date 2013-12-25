'use strict';
angular.module('Playground.games', ['ui.bootstrap.pagination'])

    .controller('GamesController', [
        '$http',
        '$scope',
        '$attrs',
        '$parse',
        '$interpolate',
        'gamesConfig',
        function ($http, $scope, $attrs, $parse, $interpolate, config) {
            $scope.games = {};

            $scope.loadGames = function (page) {
                if ($scope.resourceUrl) {
                    $http(
                    {
                        method: 'GET',
                        url: $attrs.resourceUrl,
                        params: {
                            id: $scope.objectId,
                            page: page,
                            count: $scope.itemsPerPage ? $scope.itemsPerPage : config.itemsPerPage
                        },
                    }).success(function (data, status, headers, config) {
                        $scope.games = data;
                    }).error(function (error) {
                        console.log('something went wrong ');
                    });
                }
                else {
                    throw Error('No resource url specified');
                }
            }

            $scope.onGamePageSelect = function (page) {
                $scope.loadGames(page);
            }

            $scope.loadGames(1);
        }])

    .constant('gamesConfig', {
        itemsPerPage: 5
    })

    .directive('games', ['gamesConfig', function (config) {
        return {
            restrict: 'E',
            templateUrl: 'app/templates/games/games.html',
            replace: true,
            scope: {
                objectId: '=',
                resourceUrl: '@',
                itemsPerPage: '@'
            },
            controller: 'GamesController',
            // set default values in compile time
            compile: function(element, attrs){
                if (!attrs.itemsPerPage) {
                    attrs.itemsPerPage = config.itemsPerPage
                }
            }
        }
    }])