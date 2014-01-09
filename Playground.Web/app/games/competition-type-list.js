'use strict';
angular.module('Playground.competition-type-list', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.pagination']).
    controller('CompetitionTypeListController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'CompetitionTypeResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, CompetitionTypeResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.competitorTypes = enums.competitionType;
        $scope.competitionTypes = {};

        $scope.loadCompetitionTypes = function (page) {
            CompetitionTypeResource.getcompetitiontypes({
                page: page,
                count: 5
            },
            function (data) {
                $scope.competitionTypes = data;
            });
        };
        
        $scope.onCompetitionTypePageSelect = function (page) {
            $scope.loadCompetitionTypes(page);
        };

        $scope.deleteCompetitionType = function (competitionType) {
            CompetitionTypeResource.remove({ 
                id: competitionType.competitionTypeID 
            },
            function (data) {
                $scope.loadCompetitionTypes($scope.competitionTypes.currentPage)
            });
        };
    

        $scope.loadCompetitionTypes(1);
    }]);