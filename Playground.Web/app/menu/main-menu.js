'use strict';
angular.module('Playground.main-menu', ['ngResource', 'ui.router']).
controller('MainMenuController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
function ($scope, $stateParams, $rootScope, security) {
    console.log('i ll render meny some day maiiiiiiiiiiin meeeeeeeenuuuuuu');
}]).
controller('UserStatusController', [
'$scope',
'$stateParams',
'$rootScope',
'security',
function ($scope, $stateParams, $rootScope, security) {
    console.log('i ll render meny some day');
}]);;