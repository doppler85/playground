'use strict';
angular.module('Playground.user-profile', [
    'ng',
    'ngResource',
    'ui.router',
    'ui.bootstrap.alert',
    'ui.bootstrap.pagination',
    'Playground.imageupload',
    'Playground.matches'])
    .filter('userfull', function () {
        return function (user) {
            return user ? user.firstName + ' ' + user.lastName : '';
        };
    })

    .config([
        '$stateProvider',
        function ($stateProvider, security) {
            $stateProvider
            .state('profile', {
                url: '/profile',
                abstract: true,
                templateUrl: 'app/templates/user/user-profile.tpl.html',
                controller: 'ProfileController',
                data: { pageTitle: 'My profile' },
                resolve: {
                    authenticaated: function (securityAuthorization, $location, $state) {
                        return securityAuthorization.requireAuthenticatedUser();
                    }
                }
            }).state('profile.info', {
                url: '/info',
                controller: 'ProfileInfoController',
                templateUrl: 'app/templates/user/user-profile.info.tpl.html'
            }).state('profile.players', {
                url: '/players',
                controller: 'ProfilePlayersController',
                templateUrl: 'app/templates/user/user-profile.players.tpl.html'
            }).state('profile.teams', {
                url: '/teams',
                controller: 'ProfileTeamsController',
                templateUrl: 'app/templates/user/user-profile.teams.tpl.html'
            }).state('profile.matches', {
                url: '/matches',
                controller: 'ProfileMatchesController',
                templateUrl: 'app/templates/user/user-profile.matches.tpl.html'
            }).state('profile.automaticconfirmations', {
                url: '/automaticconfirmations',
                controller: 'ProfileAutomaticConfirmationsController',
                templateUrl: 'app/templates/user/user-profile.automatic-confirmations.tpl.html'
            })
        }
    ])
    .controller('ProfileInfoController', [
    '$scope',
    '$window',
    'security',
    'UserResource',
    'enums',
    function ($scope, $window, security, UserResource, enums) {
        $scope.$parent.tab = 'info';
        $scope.genders = enums.gender;
        $scope.currentUser = security.currentUser;
        $scope.profile =
        {
            gender: 1
        };
        $scope.changePassModel = {};

        $scope.loginInfo = {};
        $scope.localAccountAlerts = [];
        $scope.externalAccountAlerts = [];

        $scope.$watch(function () {
            $scope.isAuthenticated = security.isAuthenticated();
            return security.currentUser;
        }, function (currentUser) {
            $scope.currentUser = currentUser;
        });

        $scope.getProfile = function () {
            UserResource.getprofile(function (data, status, headers, config) {
                $scope.profile = data;
            });
        };

        $scope.updateProfile = function () {
            UserResource.updateprofile($scope.profile, function (data, status, headers, config) {
                $scope.profile = data;
                security.refreshCurrentUser();
            });
        };

        $scope.getLoginInfo = function () {
            security.getLoginInfo().then(
                function (data) {
                    $scope.loginInfo = data;
                },
                function (error) {
                    var msgs = $scope.getErrorsFromResponse(error.data);
                    for (var key in msgs) {
                        $scope.addAlert($scope.externalAccountAlerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        };

        $scope.removeLogin = function (login) {
            security.removeLogin(login).then(
                function (data) {
                    $scope.getLoginInfo();
                    $scope.addAlert($scope.externalAccountAlerts, { type: 'success', msg: "Account successfully removed" });
                },
                function (error) {
                    var msgs = $scope.getErrorsFromResponse(error.data);
                    for (var key in msgs) {
                        $scope.addAlert($scope.externalAccountAlerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        };

        $scope.setPassword = function () {
            security.setPassword($scope.changePassModel).then(
                function (data) {
                    $scope.changePassModel = {};
                    $scope.addAlert($scope.localAccountAlerts, { type: 'success', msg: "Password successfully changed" });
                    security.refreshCurrentUser();
                },
                function (error) {
                    var msgs = $scope.getErrorsFromResponse(error.data);
                    for (var key in msgs) {
                        $scope.addAlert($scope.localAccountAlerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        };

        $scope.resetPassword = function () {
            var resetPassModel = {
                loginProvider: $scope.loginInfo.localLoginProvider,
                providerKey: $scope.currentUser.userName
            };
            security.resetPassword(resetPassModel).then(
                function (data) {
                    $scope.changePassModel = {};
                    $scope.addAlert($scope.localAccountAlerts, { type: 'success', msg: "Password successfully removed" });
                    security.refreshCurrentUser();
                },
                function (error) {
                    var msgs = $scope.getErrorsFromResponse(error.data);
                    for (var key in msgs) {
                        $scope.addAlert($scope.localAccountAlerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        };

        $scope.changePass = function () {
            security.changePassword($scope.changePassModel).then(
                function (data) {
                    $scope.changePassModel = {};
                    $scope.addAlert($scope.localAccountAlerts, { type: 'success', msg: "Password successfully changed" });
                    security.refreshCurrentUser();
                },
                function (error) {
                    var msgs = $scope.getErrorsFromResponse(error.data);
                    for (var key in msgs) {
                        $scope.addAlert($scope.localAccountAlerts, { type: 'danger', msg: msgs[key] });
                    }
                }
            );
        };

        $scope.addLogin = function (loginprovider) {
            // TODO: add (sessionStorage["associatingExternalLogin"] and redirect to fb login 
            $window.sessionStorage["state"] = loginprovider.state;
            $window.sessionStorage["associatingExternalLogin"] = true;

            $scope.archiveSessionStorageToLocalStorage();
            $window.location = loginprovider.url;
        };

        $scope.onImageCropped = function (data) {
            console.log('image cropped', data);
            security.refreshCurrentUser();
        };

        $scope.getProfile();
        $scope.getLoginInfo();

    }])
    .controller('ProfilePlayersController', [
    '$scope',
    '$state',
    'UserResource',
    function ($scope, $state, UserResource) {
        $scope.$parent.tab = 'players';
        $scope.players = {};
        $scope.playerAlerts = [];

        $scope.loadPlayers = function (page) {
            UserResource.allPlayers({
                    page: page,
                    count: 5
                }, function (data, status, headers, config) {
                    $scope.players = data;
                }
            );
        }

        $scope.onPlayerPageSelect = function (page) {
            $scope.loadPlayers(page);
        }

        $scope.deletePlayer = function (player) {
            UserResource.deletecompetitor({ id: player.competitorID },
                function (data, status, headers, config) {
                    $scope.loadPlayers($scope.players.currentPage);
                }, function () {
                    $scope.playerAlerts.push({ type: 'danger', msg: 'Error deleting competitor: ' + player.name });
                }
            );
        };

        $scope.loadPlayers(1);
    }])
    .controller('ProfileTeamsController', [
    '$scope',
    '$state',
    'UserResource',
    function ($scope, $state, UserResource) {
        $scope.$parent.tab = 'teams';
        $scope.teams = {};
        $scope.teamAlerts = [];

        $scope.loadTeams = function (page) {
            UserResource.allTeams({
                    page: page,
                    count: 5
                }, function (data, status, headers, config) {
                    $scope.teams = data;
                }
            );
        }

        $scope.onTeamPageSelect = function (page) {
            $scope.loadTeams(page);
        }

        $scope.deleteTeam = function (team) {
            UserResource.deletecompetitor({ id: team.competitorID },
                function (data, status, headers, config) {
                    $scope.loadTeams($scope.teams.currentPage);
                }, function () {
                    $scope.teamAlerts.push({ type: 'danger', msg: 'Error deleting competitor: ' + team.name });
                }
            );
        };

        $scope.loadTeams(1);
    }])
    .controller('ProfileMatchesController', [
    '$scope',
    '$state',
    'UserResource',
    function ($scope, $state, UserResource) {
        $scope.$parent.tab = 'matches';

        $scope.confirmScore = function (match, score, $index, msgCollection) {
            UserResource.confirmscore({
                matchID: match.matchID,
                competitorID: score.competitorID,
                confirmed: true
            },
                function (data, status, headers, config) {
                    score.confirmed = true;
                    match.status = data.status;
                }, function () {
                    msgCollection.push({ type: 'danger', msg: 'Error deleting competitor: ' + competitor.name });
                });
        };
    }])
    .controller('ProfileAutomaticConfirmationsController', [
    '$scope',
    '$state',
    'UserResource',
    function ($scope, $state, UserResource) {
        $scope.$parent.tab = 'automaticconfirmations';

        $scope.loadAutomaticConfirmations = function (page) {
            UserResource.automaticmatchconfirmations({
                page: page,
                count: 5
            }, function (data, status, headers, config) {
                $scope.automaticMatchConfirmations = data;
            });
        };

        $scope.onAutomaticMatchConfirmationsPageSelect = function (page) {
            $scope.loadAutomaticConfirmations(page);
        };

        $scope.searchAutomaticConfirmationUsers = function (page) {
            UserResource.automaticmatchconfirmationsusers({
                search: $scope.searchQuery,
                page: page,
                count: 5
            }, function (data, status, headers, config) {
                $scope.automaticMatchConfirmationsUsers = data;
            });
        };

        $scope.onUsersPageSelect = function (page) {
            $scope.searchAutomaticConfirmationUsers(page);
        };

        $scope.addAutomaticConfirmation = function (user, index) {
            UserResource.addautomaticconfirmation(user,
                function (data, status, headers, config) {
                    $scope.automaticMatchConfirmationsUsers.items.splice(index, 1);
                    $scope.loadAutomaticConfirmations($scope.automaticMatchConfirmations.currentPage);
                }
            );
        };

        $scope.deleteAutomaticConfirmation = function (user, index) {
            UserResource.deleteautomaticconfirmation({ confirmeeID: user.userID },
                function (data, status, headers, config) {
                    $scope.automaticMatchConfirmations.items.splice(index, 1);
                }
            );
        };

        $scope.toggleAddConfirmation = function (show) {
            $scope.addingconfirmation = show;
            if (!show) {
                $scope.automaticMatchConfirmationsUsers = [];
            }
        };

        $scope.loadAutomaticConfirmations(1);
    }])
    .controller('ProfileController', [
    '$scope',
    '$state',
    'security',
    'UserResource',
    function ($scope, $state, security, UserResource) {
        $scope.pageTitle = $state.current.data.pageTitle;
        $scope.tab = 'info';
        $scope.automaticMatchConfirmations = [];
        $scope.automaticMatchConfirmationsUsers = [];
        $scope.searchQuery = '';
        $scope.addingconfirmation = false;
 
        $scope.$watch(function () {
            $scope.isAuthenticated = security.isAuthenticated();
            return security.currentUser;
        }, function (currentUser) {
            $scope.currentUser = currentUser;
            $scope.isAuthenticated = security.isAuthenticated();
        });
    }]);