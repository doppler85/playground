'use strict';
angular.module('Playground.playground-home', [
    'ngResource',
    'ui.router',
    'google-maps',
    'ui.bootstrap.pagination'])

    .config([
        '$stateProvider',
        function ($stateProvider, security) {
            $stateProvider
            .state('playground', {
                url: '/playground/:id',
                abstract: true,
                templateUrl: 'app/templates/playground/playground.tpl.html',
                controller: 'PlaygroundController',
                data: { pageTitle: 'Playground' },
                resolve: {
                    currentUser: function (securityAuthorization) {
                        return securityAuthorization.requireCurrentUser();
                    }
                }
            }).state('playground.home', {
                url: '/home',
                controller: 'PlaygroundHomeController',
                data: { pageTitle: 'Playground' },
                templateUrl: 'app/templates/playground/playground.home.tpl.html'
            }).state('playground.games', {
                url: '/games',
                controller: 'PlaygroundGamesController',
                data: { pageTitle: 'Games' },
                templateUrl: 'app/templates/playground/playground.games.tpl.html'
            }).state('playground.users', {
                url: '/users',
                controller: 'PlaygroundUsersController',
                data: { pageTitle: 'Users' },
                templateUrl: 'app/templates/playground/playground.users.tpl.html'
            }).state('playground.matches', {
                url: '/matches',
                controller: 'PlaygroundMatchesController',
                data: { pageTitle: 'Matches' },
                templateUrl: 'app/templates/playground/playground.matches.tpl.html'
            })
        }
    ])
    .controller('PlaygroundController', [
    '$scope',
    '$rootScope',
    '$state',
    '$stateParams',
    'PlaygroundResource',
    'security',
        function ($scope, $rootScope, $state, $stateParams, PlaygroundResource, security) {
            $rootScope.$state = $state;
            $rootScope.$stateParams = $stateParams;

            $scope.isAuthenticated = security.isAuthenticated();
            $scope.playground = {
                isMember: true
            };

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

            $scope.loadStats = function () {
                PlaygroundResource.getstats({ id: $stateParams.id },
                    function (data, status, headers, config) {
                        $scope.stats = data;
                    }
                );
            }

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
                PlaygroundResource.joinplayground({ playgroundID: $stateParams.id },
                    function () {
                        $scope.playground.isMember = true;
                        $scope.loadPlaygroundUsers();
                        $scope.loadStats();
                    },
                    function (err) {
                        var msgs = $scope.getErrorsFromResponse(err);
                        for (var key in msgs) {
                            $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                        }
                    });
            };

            $scope.loadPlayground();
            $scope.loadStats();
        }
    ])
    .controller('PlaygroundHomeController', [
    '$scope',
    '$stateParams',
    '$rootScope',
    '$state',
    'PlaygroundResource',
    'GameResource',
    'UserResource',
    'enums',
    '$log',

    function ($scope, $stateParams, $rootScope, $state, PlaygroundResource, GameResource, UserResource, enums, $log) {
        $scope.$parent.pageTitle = $state.current.data.pageTitle;
        $scope.games = {};
        $scope.users = {};
        $scope.alerts = [];

        $scope.loadPlaygroundGames = function () {
            GameResource.playgroundgames({
                count: 5,
                page: 1,
                id: $stateParams.id
            },
            function (data, status, headers, config) {
                $scope.games = data;
            },
            function (err) {
                var msgs = $scope.getErrorsFromResponse(err);
                for (var key in msgs) {
                    $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
                }
            });
        };

        $scope.loadPlaygroundUsers = function() {
            UserResource.playgroundusers({
                id: $stateParams.id,
                page: 1,
                count: 5
            },
                function (data, status, headers, config) {
                    $scope.users = data;
                }
            );
        };

        $scope.loadPlaygroundGames();
        $scope.loadPlaygroundUsers();
    }])
    .controller('PlaygroundGamesController', [
    '$scope',
    '$state',
    '$stateParams',
    function ($scope, $state, $stateParams) {
        $scope.$parent.pageTitle = $state.current.data.pageTitle;

    }])
    .controller('PlaygroundUsersController', [
    '$scope',
    '$state',
    '$stateParams',
    function ($scope, $state, $stateParams) {
        $scope.$parent.pageTitle = $state.current.data.pageTitle;

    }])
    .controller('PlaygroundMatchesController', [
    '$scope',
    '$state',
    '$stateParams',
    function ($scope, $state, $stateParams) {
        $scope.$parent.pageTitle = $state.current.data.pageTitle;

    }]);