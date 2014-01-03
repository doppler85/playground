'use strict';
angular.module('Playground.players', ['ui.bootstrap.pagination'])

    .controller('PlayersController', [
        '$http',
        '$scope',
        '$attrs',
        '$parse',
        '$interpolate',
        'playersConfig',
        function ($http, $scope, $attrs, $parse, $interpolate, config) {
            $scope.players = {};

            $scope.loadPlayers = function (page) {
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
                        $scope.players = data;
                    }).error(function (error) {
                        console.log('something went wrong ');
                    });
                }
                else {
                    throw Error('No resource url specified');
                }
            }

            $scope.onPlayerPageSelect = function (page) {
                $scope.loadPlayers(page);
            }

            $scope.loadPlayers(1);
        }])

    .constant('playersConfig', {
        itemsPerPage: 5
    })

    .directive('players', function () {
        return {
            restrict: 'E',
            templateUrl: 'app/templates/competition/players.html',
            replace: true,
            scope: {
                objectId: '=',
                resourceUrl: '@',
                itemsPerPage: '@'
            },
            controller: 'PlayersController',
            // set default values in compile time
            compile: function(element, attrs) {
                if (!attrs.itemsPerPage) {
                    attrs.itemsPerPage = config.itemsPerPage
                }
            }
        }
    })