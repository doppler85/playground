'use strict';
angular.module('Playground.game-edit', ['ngResource', 'ui.router', 'Playground.imageupload']).
    controller('EditGameController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'GameResource',
    'CompetitionTypeResource',
    'enums',
    function ($scope, $state, $stateParams, $rootScope, GameResource, CompetitionTypeResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.game = {};
        $scope.competitionTypes = enums.competitionType;
        $scope.gameCompetitionTypes = [];
        $scope.alerts = [];

        GameResource.getupdategame({ id: $stateParams.id }, function (data, status, headers, config) {
            $scope.game = data;
        });

        $scope.updateGame = function () {
            // $scope.game.competitionTypes = $scope.selectedCompetitionTypes;
            GameResource.update($scope.game,
                function (data, status, headers, config) {
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'Game sucessfully updated' });
                },
                function () {
                    $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Error updating game' });
                }
            );
        };

        $scope.cancel = function () {
            window.history.back();
        };
    }]);