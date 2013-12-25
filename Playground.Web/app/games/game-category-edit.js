'use strict';
angular.module('Playground.game-category-edit', ['ngResource', 'ui.router', 'Playground.imageupload']).
    controller('EditGameCategoryController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'GameCategoryResource',
    function ($scope, $state, $stateParams, $rootScope, GameCategoryResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.gameCategory = {};
        $scope.alerts = [];

        $scope.loadGameCategory = function () {
            GameCategoryResource.getcategory({ id: $stateParams.id },
                function (data, status, headers, config) {
                    $scope.gameCategory = data;
                },
                function () {
                    $scope.addAlert({ type: 'error', msg: 'Error saving game' });
                }
            );
        }

        $scope.updateGameCategory = function () {
            GameCategoryResource.update($scope.gameCategory,
                function (data, status, headers, config) {
                    $scope.addAlert({ type: 'success', msg: 'Game category sucessfully updated' });
                },
                function () {
                    $scope.addAlert({ type: 'error', msg: 'Error saving game' });
                }
            );
        };

        $scope.addAlert = function (msg) {
            $scope.alerts.push(msg);
        };

        $scope.closeAlert = function (index) {
            $scope.alerts.splice(index, 1);
        };

        $scope.loadGameCategory();
    }]);