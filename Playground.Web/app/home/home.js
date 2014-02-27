'use strict';
angular.module('Playground.home', ['ngResource', 'ui.router', 'google-maps', 'ngAnimate']).
    controller('HomeController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'HomeResource',
    'CompetitorResource',
    'PlaygroundResource',
    function ($scope, $stateParams, $rootScope, $state, homeResource, competitorResource, PlaygroundResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.matches = [];
        $scope.totalMatches = 0;
        $scope.competitors = [];
        $scope.hallOfFame = [];
        $scope.hallOfShame = [];
        $scope.stats = {};
        $scope.localSearch = false;
        $scope.search = '';
        $scope.alerts = [];
        $scope.playgrounds = {};

        $scope.map = {
            bounds: {},
            control: {},
            center: {
                latitude: 45.254302,
                longitude: 19.842915
            },
            zoom: 14,
            events: {
                tilesloaded: function (map, eventName, originalEventArgs) {
                },
                dragend: function (map, eventName, originalEventArgs) {
                },
                bounds_changed: function (map, eventName, originalEventArgs) {
                }
            },
            playgroundMarkers : []
        };

        $scope.loadStats = function () {
            homeResource.getstats(
                function (data, status, headers, config) {
                    $scope.stats = data;
                }
            );
        }

        $scope.searchPlaygrounds = function(page){
            var searchArgs = {
                count: 5,
                page: page || 1,
                search: $scope.search,
                globalSearch: !$scope.localSearch,
                startLocationLatitude: $scope.map.bounds.southwest.latitude,
                startLocationLongitude: $scope.map.bounds.southwest.longitude,
                endLocationLatitude: $scope.map.bounds.northeast.latitude,
                endLocationLongitude: $scope.map.bounds.northeast.longitude
            };

            PlaygroundResource.searchplaygrounds(searchArgs, 
                function (data, status, headers, config) {
                    $scope.playgrounds = data;
                    var markers = [];
                    for (var i = 0; i < data.items.length; i++) {
                        markers.push({
                            latitude: data.items[i].latitude,
                            longitude: data.items[i].longitude,
                            title: data.items[i].name,
                            address: data.items[i].address,
                            showWindow: false
                        })
                        data.items[i].marker = markers[i];
                    }
                    $scope.map.playgroundMarkers = markers;

                    if (data.items.length > 0) {
                        $scope.map.control.getGMap().setCenter({
                            lat: data.items[0].latitude,
                            lng: data.items[0].longitude
                        });
                    }
                }, 
                function(err) 
                {
                    var msgs = $scope.getErrorsFromResponse(err);
                    for (var key in msgs) {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            )
        }

        $scope.onPlaygroundPageSelect = function (page) {
            $scope.searchPlaygrounds(page);
        }

        $scope.onPlaygroundClick = function (playground) {
            $scope.map.control.getGMap().setCenter({
                lat: playground.latitude,
                lng: playground.longitude
            });
            angular.forEach($scope.map.playgroundMarkers, function (marker) {
                marker.showWindow = false;
            });
            playground.marker.showWindow = true;
        }

        $scope.loadPlaygroundsInArea = function (bounds) {
            PlaygroundResource.getplaygroundsinarea({
                    StartLocationLatitude: bounds.southwest.latitude,
                    StartLocationLongitude: bounds.southwest.longitude,
                    EndLocationLatitude: bounds.northeast.latitude,
                    EndLocationLongitude: bounds.northeast.longitude,
                    MaxResults: 200
                },
                function (data, status, headers, config) {
                    var markers = [];
                    for (var i = 0; i < data.length; i++) {
                        markers.push({
                            latitude: data[i].latitude,
                            longitude: data[i].longitude,
                            title: data[i].name,
                            address: data[i].address
                        })
                    }
                    $scope.map.playgroundMarkers = markers;
                },
                function (err) {
                    var msgs = $scope.getErrorsFromResponse(err);
                    for (var key in msgs) {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        }

        $scope.loadStats();

        var onMarkerClicked = function (marker) {
            marker.showWindow = !marker.showWindow;
            //window.alert("Marker: lat: " + marker.latitude + ", lon: " + marker.longitude + " clicked!!")
        };

        /*
        $scope.loadMatches = function () {
            homeResource.lastmatches({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.matches = data.items;
                $scope.totalMatches = data.totalItems;
            });
        }

        $scope.loadCompetitors = function () {
            homeResource.lastcompetitors({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.competitors = data.items;
                $scope.totalCompetitors = data.totalItems;
            });
        }

        $scope.loadHallOfFame = function () {
            competitorResource.halloffame({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.hallOfFame = data.items;
            });
        }

        $scope.loadHallOFShame = function () {
            competitorResource.hallofshame({ page: 1, count: 5 }, function (data, status, headers, config) {
                $scope.hallOfShame = data.items;
            });
        }
        
        $scope.loadMatches();
        $scope.loadCompetitors();
        $scope.loadHallOfFame();
        $scope.loadHallOFShame();
        */

        var hub = $.connection.livescores;
        hub.client.refreshMatches = function (match, totalMathces) {
            $scope.totalMatches = totalMathces;
            $scope.matches.unshift(match);

            if ($scope.matches.length > 5) {
                $scope.matches.pop();
            }

            $scope.$digest();

            // reload top lists
            $scope.loadHallOfFame();
            $scope.loadHallOFShame();
        };

        $.connection.hub.start()
        .then(function () {
            hub.server.getTotalMatches();
        })
        .done(function () {
            console.log('connection started');
        }).fail(function () {
            console.log('failure starting hub');
        });
    }]);