'use strict';
angular.module('Playground.game-edit', ['ngResource', 'ui.router']).
    controller('EditGameController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    'GameResource',
    'CompetitionTypeResource',
    'enums',
    function ($scope, $stateParams, $rootScope, GameResource, CompetitionTypeResource, enums) {
        $scope.competitionTypes = enums.competitionType;
        $scope.selectedCompetitionTypes = [];

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

        $scope.saveGame = function () {
            $scope.game.competitionTypes = $scope.selectedCompetitionTypes;
            GameResource.save($scope.game, function (data, status, headers, config) {
            });
        };

        // competition types
        $scope.addingCompetitionType = false;
        $scope.createComepetitionType = function () {
            var competitionType = 
            {
                name: '',
                competitorType: 0,
                competitorsCount: ''
            };
            return competitionType;
        };
        $scope.newCompetitionType = $scope.createComepetitionType();

        $scope.toggleAddCompetitorType = function (show) {
            $scope.addingCompetitionType = show;
            if (!show) {
                $scope.newCompetitionType = $scope.createComepetitionType();
            }
        };

        $scope.addCompetitionType = function () {
            CompetitionTypeResource.add($scope.newCompetitionType, function (data, status, headers, config) {
                $scope.availableCompetitionTypes.push(data);
                $scope.toggleAddCompetitorType(false);
            });
        };
    }]);