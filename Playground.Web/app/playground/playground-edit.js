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
    function ($scope, $state, $stateParams, $rootScope, GameCategoryResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.$parent.tab = 'info';
        //$scope.gameCategory = {};
        //$scope.alerts = [];

        //$scope.loadGameCategory = function () {
        //    GameCategoryResource.getcategory({ id: $stateParams.id },
        //        function (data, status, headers, config) {
        //            $scope.gameCategory = data;
        //        },
        //        function () {
        //            $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Error saving game' });
        //        }
        //    );
        //}

        //$scope.updateGameCategory = function () {
        //    GameCategoryResource.update($scope.gameCategory,
        //        function (data, status, headers, config) {
        //            $scope.addAlert($scope.alerts, { type: 'success', msg: 'Game category sucessfully updated' });
        //        }, function (err) {
        //            var msg = err.data ? err.data.replace(/"/g, "") : "Error updating game category";
        //            $scope.addAlert($scope.alerts, { type: 'danger', msg: msg });
        //        }
        //    );
        //};

        //$scope.loadGameCategory();
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
                    $scope.games = data;
                }
            );
        }

        $scope.onGamePageSelect = function (page) {
            $scope.loadGames(page);
        }

        $scope.loadUsers(1);

        //$scope.gameCategoryID = $stateParams.id;
        //$scope.alerts = [];

        //$scope.loadGames = function (page) {
        //    GameCategoryResource.getgames({
        //            id: $scope.gameCategoryID,
        //            page: page,
        //            count: 5
        //        },
        //        function (data, status, headers, config) {
        //            $scope.games = data;
        //        }
        //    );
        //}

        //$scope.onGamePageSelect = function (page) {
        //    $scope.loadGames(page);
        //}

        //$scope.addinggame = false;
        //$scope.newGame = {
        //    title: ''
        //};

        //$scope.toggleAddGame = function (show) {
        //    $scope.addinggame = show;
        //    if (!show) {
        //        $scope.newGame.title = '';
        //    }
        //};

        //$scope.addGame = function (gameCategory) {
        //    $scope.alerts = [];
        //    $scope.newGame.gameCategoryID = $scope.gameCategoryID;

        //    GameResource.add($scope.newGame,
        //        function (data, status, headers, config) {
        //            $state.go('game-edit', { id: data.gameID });
        //        }, function (err) {
        //            var msgs = $scope.getErrorsFromResponse(err);
        //            for (var key in msgs) {
        //                $scope.addAlert($scope.alerts, { type: 'danger', msg: msgs[key] });
        //            }
        //        });
        //};

        //$scope.deleteGame = function (game, collection, index) {
        //    GameResource.remove({
        //        id: game.gameID
        //    }, function (data, status, headers, config) {
        //        collection.splice(index, 1);
        //    }, function (err) {
        //        $scope.addAlert($scope.alerts, { type: 'danger', msg: 'Error deleting game' });
        //    });
        //};

        //$scope.loadGames(1);
    }]);