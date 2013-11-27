'use strict';
angular.module('Playground.game', ['ngResource', 'ui.router']).
    config(['$stateProvider', function config($stateProvider) {
        $stateProvider
            .state('game', {
                    url: '/game',
                    controller: 'ListGameController',
                    data: { pageTitle: 'Games' }
            }).state('game-edit', {
                        url: '/game/edit/:id',
                        templateUrl: 'app/games/game-edit.tpl.html',
                        controller: 'EditGameController',
                        data: { pageTitle: 'Edit Game' }
            }).state('game-details', {
                url: '/game/details/:id',
                templateUrl: 'app/games/game-details.tpl.html',
                controller: 'DetailsGameController',
                data: { pageTitle: 'Game details' }
            });
    }]).
    controller('ListGameController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    'GameResource',
    function ($scope, $stateParams, $rootScope, GameResource) {
        $scope.addinggame = false;
        $scope.newGame = {
            gameCategoryID: $scope.gameCategory.gameCategoryID,
            title: ''
        };

        $scope.toggleAddGame = function (show) {
            $scope.addinggame = show;
            if (!show) {
                $scope.newGame.title = '';
            }
        };

        $scope.addGame = function() {
            GameResource.add($scope.newGame, function (data, status, headers, config) {
                $scope.newGame.title = '';
                $scope.gameCategory.games.push(data);
            });
        };
    }]).
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

        GameResource.details({ id: $stateParams.id }, function (data, status, headers, config) {
            $scope.game = data;

            angular.forEach($scope.game.competitionTypes, function (competitionType) {
                this.push(competitionType.competitionType);
            }, $scope.selectedCompetitionTypes);

        });

        GameResource.availablecomptypes({ id: $stateParams.id }, function (data, status, headers, config) {
            $scope.availableCompetitionTypes = data;
        });

        $scope.toggleCompetitionType = function (competitionType) {
            var idx = $scope.selectedCompetitionTypes.indexOf(competitionType);

            if (idx > -1) {
                $scope.selectedCompetitionTypes.splice(idx, 1);
            }
            else {
                $scope.selectedCompetitionTypes.push(competitionType);
            }
        };

        $scope.saveGame = function () {
            console.log($scope.selectedCompetitionTypes);
            console.log($scope.game.competitionTypes);

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
    }]).
    controller('DetailsGameController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    'GameResource',
    function ($scope, $stateParams, $rootScope, GameResource) {
        console.log($stateParams);
    }]);