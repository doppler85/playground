'use strict';
angular.module('Playground.competition-type-edit', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.alert',
    'Playground.validation',
    'Playground.competition-type.validation'
    ])
    .controller('CompetitionTypeEditController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'CompetitionTypeResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, CompetitionTypeResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.competitorTypes = enums.competitionType;
        $scope.competitionType = {};
        $scope.alerts = [];

        $scope.loadCompetitionType = function () {
            CompetitionTypeResource.getcompetitiontype({
                id: $stateParams.id
            },
            function (data, status, headers, config) {
                $scope.competitionType = data;
            },
            function (data) {
                $scope.addAlert($scope.alerts, { type: 'danger', msg: data.data });
            })
        }

        $scope.updateCompetitionType = function () {
            CompetitionTypeResource.update($scope.competitionType,
                function (data, status, headers, config) {
                    $state.go('competition-types');
                },
                function () {
                    $scope.addAlert({ type: 'danger', msg: 'Error updating competition type' });
                }
            );
        };

        $scope.cancel = function () {
            $state.go('competition-types');
        }

        $scope.loadCompetitionType();
    }]);