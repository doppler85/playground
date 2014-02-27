'use strict';
angular.module('Playground.playground-home', [
    'ngResource',
    'ui.router',
    'google-maps',
    'ui.bootstrap.pagination']).
    controller('PlaygroundHomeController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'PlaygroundResource',
    'enums',
    '$log',
    'security',
    function ($scope, $stateParams, $rootScope, $state, PlaygroundResource, enums, $log, security) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.isAuthenticated = security.isAuthenticated();

        $scope.playground = {
            isMember: true
        };
        $scope.alerts = [];

        $scope.map = {
            bounds: {},
            control: {},
            center: {
                latitude: 45.254302,
                longitude: 19.842915
            },
            zoom: 15,
            events: {
                tilesloaded: function (map, eventName, originalEventArgs) {
                },
                dragend: function (map, eventName, originalEventArgs) {
                },
                bounds_changed: function (map, eventName, originalEventArgs) {
                }
            },
            playgroundMarker: {
                showWindow: false,
                latitude: 0,
                longitude: 0,
                title: '',
                address: ''
            }
        };

        $scope.loadPlayground = function () {
            PlaygroundResource.getplayground({ playgroundID: $stateParams.id },
                function (data, status, headers, config) {
                    $scope.playground = data;
                    $scope.map.playgroundMarker = {
                        latitude: data.latitude,
                        longitude: data.longitude,
                        title: data.name,
                        address: data.address
                    };

                    $scope.map.control.getGMap().setCenter({
                        lat: data.latitude,
                        lng: data.longitude
                    });
                },
                function (err) {
                    var msgs = $scope.getErrorsFromResponse(err);
                    for (var key in msgs) {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                    }
                });
        };

        $scope.joinPlayground = function () {
            PlaygroundResource.joinplayground({ playgroundID: $scope.playground.playgroundID },
                function () {
                    $scope.playground.isMember = true;
                },
                function(err) 
                {
                    var msgs = $scope.getErrorsFromResponse(err);
                    for (var key in msgs) {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                    }
                });
        };

        $scope.loadPlaygroundGames = function () {
            // todo
        };

        $scope.loadPlaygroundCompetitors = function() {
            // todo
        };

        $scope.loadPlaygroundMatches = function () {
            // todo
        };

        $scope.loadPlayground();
        
    }]);