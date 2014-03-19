'use strict';
angular.module('Playground.match-list', [
    'ngResource',
    'ui.router',
    'Playground.matches']).
controller('MatchListController', [
'$scope',
'$state',
'$stateParams',
'$rootScope',
function ($scope, $state, $stateParams, $rootScope) {
    $scope.pageTitle = $state.current.data.pageTitle;

}]);;