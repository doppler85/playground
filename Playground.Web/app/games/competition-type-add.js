'use strict';
angular.module('Playground.competition-type-add', ['ngResource', 'ui.router', 'ui.bootstrap.alert']).
    controller('CompetitionTypeAddController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'CompetitionTypeResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, CompetitionTypeResource, enums) {
        $scope.competitorTypes = enums.competitionType;
        $scope.competitionType = {};
        $scope.alerts = [];

        $scope.addCompetitionType = function () {
            CompetitionTypeResource.add($scope.competitionType,
                function (data, status, headers, config) {
                    $state.go('competition-types');
                },
                function () {
                    $scope.addAlert({ type: 'error', msg: 'Error adding competition type' });
                }
            );
        };

        $scope.cancel = function () {
            $state.go('competition-types');
        }

        $scope.addAlert = function (msg) {
            $scope.alerts.push(msg);
        };

        $scope.closeAlert = function () {
            $scope.alerts.splice(index, 1);
        };
    }]);