'use strict';
angular.module('Playground.competition-type-list', ['ngResource', 'ui.router']).
    controller('CompetitionTypeListController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    'CompetitionTypeResource',
    'enums',
    function ($scope, $stateParams, $rootScope, CompetitionTypeResource, enums) {
        $scope.competitorTypes = enums.competitionType;
        $scope.competitionTypes = {};

        $scope.loadCompetitionTypes = function () {
            CompetitionTypeResource.all(function (data) {
                $scope.competitionTypes = data;
            });
        };

        $scope.deleteCompetitionType = function (competitionType, index) {
            console.log('not impelmented');
        }

        $scope.loadCompetitionTypes();
    }]);