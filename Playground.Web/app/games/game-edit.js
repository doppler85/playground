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
        $scope.selectedCompetitionTypes = [];
        $scope.alerts = [];

        $scope.selectedCompetitionTypes.indexOf = function (obj) {
            var index = -1;
            if (obj == null || obj.competitionTypeID == null || obj.gameID == null) {
                return index;
            }
            for (var i = 0, imax = this.length; i < imax; i++) {
                if (this[i].competitionTypeID === obj.competitionTypeID && this[i].gameID === obj.gameID) {
                    index = i;
                    break;
                }
            }
            return index;
        };

        GameResource.details({ id: $stateParams.id }, function (data, status, headers, config) {
            $scope.game = data;
            angular.forEach($scope.game.competitionTypes, function (competitionType) {
                this.push({
                    competitionTypeID: competitionType.competitionType.competitionTypeID,
                    gameID: competitionType.gameID
                });
            }, $scope.selectedCompetitionTypes);
        }).$promise.then(function () {
            CompetitionTypeResource.all(function (data, status, headers, config) {
                $scope.availableCompetitionTypes = data;
                angular.forEach($scope.availableCompetitionTypes, function (competitionType) {
                    var ct = {
                        competitionTypeID: competitionType.competitionTypeID,
                        gameID: $scope.game.gameID
                    };
                    var idx = $scope.selectedCompetitionTypes.indexOf(ct);
                    if (idx != -1) {
                        competitionType.checked = true;
                    }
                });
            });
        });

        $scope.toggleCompetitionType = function (competitionType) {
            var ct = {
                competitionTypeID: competitionType.competitionTypeID,
                gameID: $scope.game.gameID
            };

            var idx = $scope.selectedCompetitionTypes.indexOf(ct);

            if (idx > -1) {
                $scope.selectedCompetitionTypes.splice(idx, 1);
            }
            else {
                $scope.selectedCompetitionTypes.push(ct);
            }
        };

        $scope.updateGame = function () {
            $scope.game.competitionTypes = $scope.selectedCompetitionTypes;
            GameResource.update($scope.game,
                function (data, status, headers, config) {
                    $scope.addAlert({ type: 'success', msg: 'Game sucessfully saved' });
                },
                function () {
                    $scope.addAlert({ type: 'error', msg: 'Error saving game' });
                }
            );
        };

        $scope.cancel = function () {
            window.history.back();
        };

        $scope.addAlert = function (msg) {
            $scope.alerts.push(msg);
        };

        $scope.closeAlert = function (index) {
            $scope.alerts.splice(index, 1);
        };
    }]);