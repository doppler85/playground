'use strict';
angular.module('Playground.match-add', ['ngResource', 'ui.router', 'ui.bootstrap.alert', 'ui.bootstrap.datepicker', 'ui.bootstrap.timepicker'])
    .filter('gamefull', function () {
        return function (game) {
            return game ? '(' + game.category.title + ') ' + game.title : '';
        };
    }).filter('playerfull', function () {
        return function (player) {
            return (player && player.user) ? '(' + player.user.firstName + ' ' + player.user.lastName + ') ' + player.name : '';
        };
    })
    .filter('competitiontypefull', function () {
        return function (competitionType) {
            if (competitionType && competitionType.competitionType) {
                var retVal = competitionType.competitionType.name + ' (';
                retVal += competitionType.competitionType.competitorsCount + ' competitor';
                retVal += competitionType.competitionType.competitorsCount > 1 ? 's)' : ')';
                return retVal;
            }
            return '';
            return (competitionType && competitionType.competitionType) ? competitionType.competitionType.name + ' ' + player.user.lastName : '';
        };
    })
    .controller('MatchAddController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'UserResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, UserResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.match = {
            date: new Date(),
            scores: []
        };
        $scope.games = [];
        $scope.categories = [];
        $scope.selectedCategory = null;
        $scope.myCompetitors = [];
        $scope.selectedCompetitor = null;
        $scope.competitorScores = [];
        $scope.selectedGame = null;
        $scope.availableCompetitors = [];
        $scope.availableCometitionTypes = [];
        $scope.searchQuery = '';
        $scope.competitorScores.indexOf = function (obj) {
            console.log(obj);
            var index = -1;
            if (obj == null || obj.competitorID == null) {
                return index;
            }
            for (var i = 0, imax = this.length; i < imax; i++) {
                if (this[i].competitorID === obj.competitorID) {
                    index = i;
                    break;
                }
            }
            return index;
        };

        $scope.alerts = [];

        $scope.addAlert = function (msg) {
            $scope.alerts.push(msg);
        };

        $scope.closeAlert = function (index) {
            $scope.alerts.splice(index, 1);
        };

        $scope.loadGames = function () {
            UserResource.mycompeatinggames(function (data, status, headers, config) {
                $scope.categories = data;
            });
        };

        $scope.categoryChanged = function () {
            $scope.games = [];
            $scope.availableCompetitors = [];
            $scope.selectedPlayers = [];
            $scope.selectedGame = null;
            $scope.competitorScores.length = 0;
            $scope.availableCometitionTypes = [];
            $scope.match.competitionTypeID = null;

            if ($scope.selectedCategory != null) {
                UserResource.mycompeatitors({ gameCategoryID: $scope.selectedCategory.gameCategoryID },
                    function (data, status, headers, config) {
                        $scope.myCompetitors = data;
                    },
                    function () {
                        $scope.myCompetitors = [];
                    }
                );
            }
        };

        $scope.competitorChanged = function () {
            $scope.selectedGame = null;
            $scope.competitorScores.length = 0;
            $scope.availableCompetitors = [];
            $scope.availableCometitionTypes = [];
            $scope.match.competitionTypeID = null;

            if ($scope.selectedCompetitor != null) {
                $scope.games = $scope.selectedCompetitor.games;
                $scope.competitorScores.push({
                    competitor: $scope.selectedCompetitor,
                    competitorID: $scope.selectedCompetitor.competitorID,
                    score: 0
                });
            }
            else {
                $scope.games = [];
            }
        };

        $scope.toggleGame = function (game) {
            $scope.match.competitionTypeID = null;
            angular.forEach(game.game.competitionTypes, function (competitionType) {
                if ($scope.selectedCompetitor.competitorType == competitionType.competitionType.competitorType) {
                    $scope.availableCometitionTypes.push(competitionType);
                }
            });
            $scope.selectedGame = game;
            
            if ($scope.searchQuery != '') {
                $scope.searchPlayers();
            } else {
                $scope.availableCompetitors = [];
            }
        };

        $scope.searchPlayers = function () {
            if ($scope.selectedGame != null) {
                $scope.availableCompetitors = [];
                UserResource.searchcompetitors(
                    {
                        gameCategoryID: $scope.selectedGame.game.gameCategoryID,
                        competitorType: $scope.selectedCompetitor.competitorType,
                        search: $scope.searchQuery
                    },
                    function (data, status, headers, config) {
                        console.log(data);
                        angular.forEach(data, function (competitor) {
                            var idx = $scope.competitorScores.indexOf(competitor);
                            if (idx == -1) {
                                $scope.availableCompetitors.push(competitor);
                            }
                        });
                    }
                );
            }
        };

        $scope.addCompetitor = function (competitor, index) {
            $scope.competitorScores.push({
                competitor: competitor,
                competitorID: competitor.competitorID,
                score: 0
            });
            $scope.availableCompetitors.splice(index, 1);
        };

        $scope.removeCompetitor = function (index) {
            $scope.competitorScores.splice(index, 1);
        };

        $scope.addMatch = function () {
            angular.forEach($scope.competitorScores, function (score) {
                $scope.match.scores.push({
                    competitorID: score.competitorID,
                    score: score.score
                });
            });
            $scope.match.gameID = $scope.selectedGame.gameID;
            UserResource.addmatch($scope.match,
                function (data, status, headers, config) {
                    $scope.match.matchID = data.matchID;
                    $scope.addAlert({ type: 'success', msg: 'Match sucessfully added' });
                },
                function () {
                    $scope.addAlert({ type: 'error', msg: 'Error adding team' });
                }
            );
        };

        $scope.cancel = function () {
            $state.go('profile');
        };

        $scope.openCalendar = function () {
            $timeout(function () {
                $scope.opened = true;
            });
        };

        $scope.loadGames();
    }]);