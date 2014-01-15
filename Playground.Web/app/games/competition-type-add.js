'use strict';
angular.module('Playground.competition-type-add', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.alert',
    'Playground.validation',
    'Playground.competition-type.validation'
    ])
    .controller('CompetitionTypeAddController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'CompetitionTypeResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, CompetitionTypeResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.competitorTypes = enums.competitionType;
        $scope.competitionType = {
            competitorType: 0,
            playersPerTeam : 2
        };
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

        $scope.closeAlert = function (index) {
            $scope.alerts.splice(index, 1);
        };
    }]);