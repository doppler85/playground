'use strict';
angular.module('Playground.game-category-details', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.tabs',
    'Playground.games',
    'Playground.players',
    'Playground.teams',
    'Playground.matches'
]).
controller('GameCategoryDetailsController', [
'$scope',
'$stateParams',
'$rootScope',
'$state',
'security',
'GameCategoryResource',
function ($scope, $stateParams, $rootScope, $state, security, GameCategoryResource) {
    $scope.pageTitle = $state.current.data.pageTitle;
    $scope.gameCategoryID = $stateParams.id;
    $scope.gameCategoryStats = {};

    $scope.loadGameCategoryStats = function () {
        GameCategoryResource.stats({id : $stateParams.id},
            function(data, status, headers, config) {
                $scope.gameCategoryStats = data;
            },
            function() {
            }
        );
    };

    $scope.loadGameCategoryStats();
}]);;
