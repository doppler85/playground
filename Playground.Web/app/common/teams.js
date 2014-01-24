'use strict';
angular.module('Playground.teams', ['ui.bootstrap.pagination'])

    .controller('TeamsController', [
        '$http',
        '$scope',
        '$attrs',
        '$parse',
        '$interpolate',
        'teamsConfig',
        function ($http, $scope, $attrs, $parse, $interpolate, config) {
            $scope.teams = {};

            $scope.loadTeams = function (page) {
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
                        $scope.teams = data;
                    }).error(function (error) {
                        console.log('something went wrong ');
                    });
                }
                else {
                    throw Error('No resource url specified');
                }
            }

            $scope.onTeamPageSelect = function (page) {
                $scope.loadTeams(page);
            }

            $scope.loadTeams(1);
        }])

    .constant('teamsConfig', {
        itemsPerPage: 5
    })

    .directive('teams', ['teamsConfig', function (config) {
        return {
            restrict: 'E',
            templateUrl: 'app/templates/competition/teams.html',
            replace: true,
            scope: {
                objectId: '=',
                resourceUrl: '@',
                itemsPerPage: '@'
            },
            controller: 'TeamsController',
            // set default values in compile time
            compile: function(element, attrs){
                if (!attrs.itemsPerPage) {
                    attrs.itemsPerPage = config.itemsPerPage
                }
            }
        }
    }])