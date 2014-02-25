'use strict';
angular.module('Playground.playground-add', [
    'ngResource',
    'ui.router',
    'google-maps',
    'ui.bootstrap.pagination']).
    controller('PlaygroundAddController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'PlaygroundResource',
    'enums',
    '$log',
    function ($scope, $stateParams, $rootScope, $state, PlaygroundResource, enums, $log) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.playground = {};
        $scope.alerts = [];

        $scope.map = {
            control: {},
            center: {
                latitude: 45.254302,
                longitude: 19.842915
            },
            zoom: 15
        };

        $scope.searchLocationMarker = {
            coords: {
                latitude: 45.254302,
                longitude: 19.842915
            },
            options: { draggable: true },
            events: {
                dragend: function (marker, eventName, args) {
                    $scope.playground.latitude = marker.getPosition().lat();
                    $scope.playground.longitude = marker.getPosition().lng();

                    $scope.$apply();
                }
            }
        }

        $scope.geocoder = new google.maps.Geocoder();
        
        $scope.searchAddress = function () {
            $scope.geocoder.geocode({
                    address: $scope.playground.address
                },
                function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        $scope.playground.latitude = results[0].geometry.location.d;
                        $scope.playground.longitude = results[0].geometry.location.e;
                        $scope.searchLocationMarker.coords.latitude = results[0].geometry.location.d;
                        $scope.searchLocationMarker.coords.longitude = results[0].geometry.location.e;

                        $scope.map.control.getGMap().setCenter(results[0].geometry.location);

                        $scope.$apply();
                    } else {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Geocode was not successful for the following reason: ' + status });
                    }
                }
            );
        }

        $scope.addPlayground = function () {
            angular.forEach($scope.games, function (game) {
                if (game.checked) {
                    $scope.player.games.push({
                        gameID: game.gameID
                    });
                }
            });
            PlaygroundResource.addplayground($scope.playground,
                function (data, status, headers, config) {
                    $scope.playground = data.playground;
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'Playground sucessfully added' });
                },
                function () {
                    $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Error adding playground' });
                }
            );
        };
    }]);