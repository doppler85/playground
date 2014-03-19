'use strict';
angular.module('Playground.playground-edit', [
    'ngResource',
    'ui.router'])
    .config(['$compileProvider',
        '$stateProvider',
        '$injector',
        function ($compileProvider, $stateProvider, $injector) {
            $stateProvider
            .state('playground-edit', {
                abstract: true,
                url: '/playground/edit',
                templateUrl: 'app/templates/playground/playground-edit.tpl.html',
                data: { pageTitle: 'Edit playground' },
                resolve: {
                    authenticaated: function (securityAuthorization) {
                        return securityAuthorization.requireAuthenticatedUser();
                    }
                }
            }).state('playground-edit.info', {
                url: '/info/:id',
                controller: 'PlaygroundEditInfoController',
                templateUrl: 'app/templates/playground/playground-edit-info.tpl.html'
            }).state('playground-edit.games', {
                url: '/games/:id',
                controller: 'PlaygroundEditGamesController',
                templateUrl: 'app/templates/playground/playground-edit-games.tpl.html'
            }).state('playground-edit.users', {
                url: '/users/:id',
                controller: 'PlaygroundEditUsersController',
                templateUrl: 'app/templates/playground/playground-edit-users.tpl.html'
            })
        }
    ])
    .controller('PlaygroundEditInfoController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'PlaygroundResource',
    function ($scope, $state, $stateParams, $rootScope, PlaygroundResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'info';
        $scope.playgroundID = $stateParams.id;
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
                        $scope.playground.latitude = results[0].geometry.location.k;
                        $scope.playground.longitude = results[0].geometry.location.A;
                        $scope.searchLocationMarker.coords.latitude = results[0].geometry.location.k;
                        $scope.searchLocationMarker.coords.longitude = results[0].geometry.location.A;

                        $scope.map.control.getGMap().setCenter(results[0].geometry.location);

                        $scope.$apply();
                    } else {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Geocode was not successful for the following reason: ' + status });
                    }
                }
            );
        }

        $scope.loadPlayground = function () {
            PlaygroundResource.getupdateplayground({ playgroundID: $scope.playgroundID },
                function (data, status, headers, config) {
                    $scope.playground = data;

                    $scope.searchLocationMarker.coords.latitude = data.latitude;
                    $scope.searchLocationMarker.coords.longitude = data.longitude;

                    var center = new google.maps.LatLng(data.latitude, data.longitude);

                    $scope.map.control.getGMap().setCenter(center);
                },
                function () {
                    $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Error loading playground' });
                }
            );
        }

        $scope.updatePlayground = function () {
            PlaygroundResource.updateplayground($scope.playground,
                function (data, status, headers, config) {
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'Playground sucessfully updated' });
                }, function (err) {
                    var msgs = $scope.getErrorsFromResponse(err);
                    for (var key in msgs) {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        };

        $scope.loadPlayground();
    }])
    .controller('PlaygroundEditGamesController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'PlaygroundResource',
    'GameResource',
    function ($scope, $state, $stateParams, $rootScope, PlaygroundResource, GameResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'games';
        $scope.playgroundID = $stateParams.id;
        $scope.addinggame = false;
        $scope.alerts = [];

        $scope.loadGames = function (page) {
            GameResource.playgroundgames({
                    id: $scope.playgroundID,
                    page: page,
                    count: 5
                },
                function (data, status, headers, config) {
                    $scope.games = data;
                }
            );
        }

        $scope.onGamePageSelect = function (page) {
            $scope.loadGames(page);
        }

        $scope.searchAvailableGames = function (page) {
            PlaygroundResource.availablegames({
                playgroundId: $scope.playgroundID,
                search: $scope.searchQuery,
                page: page,
                count: 5
            }, function (data, status, headers, config) {
                $scope.availableGames = data;
            });
        }

        $scope.onAvailableGamesPageSelect = function (page) {
            $scope.searchAvailableGames(page);
        }

        $scope.addGame = function (game, index) {
            $scope.alerts = [];
            PlaygroundResource.addgame({
                    playgroundID: $scope.playgroundID,
                    gameID: game.gameID
                },
                function (data, status, headers, config) {
                    $scope.availableGames.items.splice(index, 1);
                    $scope.loadGames($scope.games.currentPage);
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'Game sucessffully added' });
                }, function (err) {
                    var msgs = $scope.getErrorsFromResponse(err);
                    for (var key in msgs) {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        }

        $scope.removeGame = function (game) {
            $scope.alerts = [];
            PlaygroundResource.removegame({
                playgroundID: $scope.playgroundID,
                gameID: game.gameID
            },
                function (data, status, headers, config) {
                    $scope.loadGames($scope.games.currentPage);
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'Game sucessffully removed' });
                }, function (err) {
                    var msgs = $scope.getErrorsFromResponse(err);
                    for (var key in msgs) {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        }

        $scope.loadGames(1);
    }])
    .controller('PlaygroundEditUsersController', [
    '$scope',
    '$state',
    '$stateParams',
    '$rootScope',
    'PlaygroundResource',
    'UserResource',
    function ($scope, $state, $stateParams, $rootScope, PlaygroundResource, UserResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'users';
        $scope.playgroundID = $stateParams.id;
        $scope.addinguser = false;
        $scope.alerts = [];

        $scope.loadUsers = function (page) {
            UserResource.playgroundusers({
                    id: $scope.playgroundID,
                    page: page,
                    count: 5
                },
                function (data, status, headers, config) {
                    $scope.users = data;
                }
            );
        }

        $scope.onUsersPageSelect = function (page) {
            $scope.loadUsers(page);
        }

        $scope.searchAvailableUsers = function (page) {
            PlaygroundResource.availableusers({
                playgroundId: $scope.playgroundID,
                search: $scope.searchQuery,
                page: page,
                count: 5
            }, function (data, status, headers, config) {
                $scope.availableUsers = data;
            });
        }

        $scope.onAvailableUsersPageSelect = function (page) {
            $scope.searchAvailableUsers(page);
        }

        $scope.addUser = function (user, index) {
            $scope.alerts = [];
            PlaygroundResource.adduser({
                playgroundID: $scope.playgroundID,
                userID: user.userID
            },
                function (data, status, headers, config) {
                    $scope.availableUsers.items.splice(index, 1);
                    $scope.loadUsers($scope.users.currentPage);
                    $scope.addAlert($scope.alerts, { type: 'success', msg: 'User sucessffully added' });
                }, function (err) {
                    var msgs = $scope.getErrorsFromResponse(err);
                    for (var key in msgs) {
                        $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        }

        $scope.deleteUser = function (user) {
            PlaygroundResource.removeuser({
                playgroundID: $scope.playgroundID,
                userID: user.userID
            }, function (data, status, headers, config) {
                $scope.loadUsers($scope.users.currentPage);
            }, function (err) {
                var msgs = $scope.getErrorsFromResponse(err);
                for (var key in msgs) {
                    $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                }
            });
        };

        $scope.loadUsers(1);
    }]);