'use strict';
angular.module('Playground.game-category-details', ['ngResource', 'ui.router', 'ui.bootstrap.tabs']).
controller('GameCategoryDetailsController', [
'$scope',
'$stateParams',
'$rootScope',
'$state',
'security',
function ($scope, $stateParams, $rootScope, $state, security) {
    $scope.pageTitle = $state.current.data.pageTitle;
    $scope.gameCategoryID = $stateParams.id;
}]);;
