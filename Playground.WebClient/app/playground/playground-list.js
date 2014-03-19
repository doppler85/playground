'use strict';
angular.module('Playground.playground-list', [
    'ngResource',
    'ui.router',
    'ui.bootstrap.pagination']).
    controller('PlaygroundListController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'PlaygroundResource',
    'enums',
    function ($scope, $stateParams, $rootScope, $state, PlaygroundResource, enums) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.playgrounds = {};
        $scope.alerts = [];

        $scope.loadPlaygrounds = function (page) {
            PlaygroundResource.getplaygrounds({
                page: page,
                count: 5
            },
            function (data) {
                $scope.playgrounds = data;
            });
        };

        $scope.deletePlayground = function (playground) {
            PlaygroundResource.removeplayground({
                    id: playground.playgroundID
                },
                function (data) {
                    $scope.addAlert($scope.alerts, { type: 'success', msg: "Playground successfully removed" });
                    $scope.loadPlaygrounds($scope.playgrounds.currentPage);
                },
                function (error) {
                    var msgs = $scope.getErrorsFromResponse(error.data);
                    for (var key in msgs) {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        }
        
        $scope.onPlaygroundPageSelect = function (page) {
            $scope.loadPlaygrounds(page);
        };

        $scope.loadPlaygrounds(1);
    }]);