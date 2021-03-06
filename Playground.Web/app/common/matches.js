﻿'use strict';
angular.module('Playground.matches', ['ui.bootstrap.pagination'])

    .controller('MatchesController', [
        '$http',
        '$scope',
        '$attrs',
        '$parse',
        '$interpolate',
        'matchesConfig',
        'UserResource',
        'enums',
        function ($http, $scope, $attrs, $parse, $interpolate, config, UserResource, enums) {
        $scope.matches = {};
        $scope.showStatusCol = $attrs.showStatusCol ? true : config.minWidth;
        $scope.showConfirmButton = $attrs.showConfirmButton ? true : config.showConfirmButton;
        $scope.matchStatuses = enums.matchStatus;

        $scope.loadMatches = function (page) {
            if ($attrs.resourceUrl) {
                $http(
                {
                    method: 'GET',
                    url: $attrs.resourceUrl,
                    params: {
                        id: $scope.objectId,
                        page: page,
                        count: $attrs.itemsPerPage
                    },
                }).success(function (data, status, headers, config) {
                    $scope.matches = data;
                }).error(function (error) {
                    console.log('something went wrong ');
                });
            }
            else {
                throw Error('No resource url specified');
            }
        }

        $scope.onMatchPageSelect = function (page) {
            $scope.loadMatches(page);
        }

        $scope.confirmScore = function (match, score, $index, msgCollection) {
            UserResource.confirmscore({
                matchID: match.matchID,
                competitorID: score.competitorID,
                confirmed: true
            },
                function (data, status, headers, config) {
                    score.confirmed = true;
                    match.status = data.status;
                }, function () {
                    msgCollection.push({ type: 'danger', msg: 'Error confirming score: ' + competitor.name });
                });
        };

        $scope.loadMatches(1);
    }])

    .constant('matchesConfig', {
        itemsPerPage: 5,
        showStatusCol: false,
        showConfirmButton: false
    })

    .directive('matches', function () {
        return {
            restrict: 'E',
            templateUrl: 'app/templates/matches/matches.html',
            replace: true,
            scope: {
                objectId: '=',
                resourceUrl: '@',
                itemsPerPage: '@'
            },
            controller: 'MatchesController'
        }
    })