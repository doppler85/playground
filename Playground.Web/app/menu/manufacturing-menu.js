'use strict';
angular.module('PlantDemo.manufacturing-menu',[]).
    controller('ManufacturingMenuController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$location',
    '$state',
    function ($scope, $stateParams, $rootScope, $location, $state) {
        $scope.changeLocation = function (path) {
            $state.transitionTo(path);
        }
    }]);