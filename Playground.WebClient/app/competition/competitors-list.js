'use strict';
angular.module('Playground.competitors-list', [
    'ngResource',
    'ui.router',
    'Playground.teams',
    'Playground.players'
]).
controller('CompetitorsListController', [
'$scope',
'$state',
'$stateParams',
'$rootScope',
function ($scope, $state, $stateParams, $rootScope) {
    $scope.pageTitle = $state.current.data.pageTitle;

}]);;