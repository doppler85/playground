'use strict';
angular.module('Playground.user-profile', ['ngResource', 'ui.router', 'ui.bootstrap.tabs'])
    .directive('fileUpload', function () {
        return {
            scope: true,        //create a new scope
            link: function (scope, el, attrs) {
                el.bind('change', function (event) {
                    var files = event.target.files;
                    //iterate files since 'multiple' may be specified on the element
                    for (var i = 0;i<files.length;i++) {
                        //emit event upward
                        scope.$emit("fileSelected", { file: files[i] });
                    }                                       
                });
            }
        };
    })
    .directive('imgCropped', function () {
        return {
            restrict: 'E',
            replace: true,
            scope: { src: '@', selected: '&' },
            link: function (scope, element, attr) {
                var myImg;
                var clear = function () {
                    if (myImg) {
                        myImg.next().remove();
                        myImg.remove();
                        myImg = undefined;
                    }
                };
                scope.$watch('src', function (nv) {
                    clear();
                    if (nv) {
                        element.after('<img />');
                        myImg = element.next();
                        myImg.attr('src', nv);
                        $(myImg).Jcrop({
                            aspectRatio: 1,
                            minSize: [100, 100],
                            maxSize: [300, 300],
                            setSelect: [0, 0, 100, 100],
                            trackDocument: true,
                            onRelease: function (rect) {
                                scope.$emit("cropingSelectionReleased", { coords: rect });
                            },
                            onSelect: function (rect) {
                                scope.$emit("cropingSelectionChanged", { coords: rect });
                            }
                        });
                    }
                });

                scope.$on('$destroy', clear);
            }
        };
    })
    .filter('userfull', function () {
        return function (user) {
            return user ? user.firstName + ' ' + user.lastName : '';
        };
    })
    .controller('ProfileController', [
    '$http',
    '$scope',
    '$stateParams',
    '$rootScope',
    'security',
    'UserResource',
    'enums',
    function ($http, $scope, $stateParams, $rootScope, security, UserResource, enums) {
        $scope.players = {};
        $scope.teams = {};
        $scope.matches = [];
        $scope.matchStatuses = enums.matchStatus;
        $scope.playerAlerts = [];
        $scope.teamAlerts = [];
        $scope.profile = {};
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

        $scope.logout = function () {
            // Try to login
            security.logout("home");
        };

        $scope.loadPlayers = function () {
            UserResource.allPlayers(function (data, status, headers, config) {
                $scope.players = data;
            });
        }

        $scope.loadTeams = function () {
            UserResource.allTeams(function (data, status, headers, config) {
                $scope.teams = data;
            });
        }

        $scope.loadMatches = function () {
            UserResource.allMatches({count: 5}, function (data, status, headers, config) {
                $scope.matches = data;
            });
        }

        $scope.deleteCompetitor = function (competitor, collection, index, msgCollection) {
            UserResource.deletecompetitor({ id: competitor.competitorID },
                function (data, status, headers, config) {
                    collection.splice(index, 1);
                }, function () {
                    msgCollection.push({ type:'error', msg:'Error deleting competitor: ' + competitor.name });
                });
        };

        $scope.addAlert = function (collection, msg) {
            collection.push(msg);
        };

        $scope.closeAlert = function (collection, index) {
            collection.splice(index, 1);
        };

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
                    msgCollection.push({ type: 'error', msg: 'Error deleting competitor: ' + competitor.name });
                });
        };

        $scope.loadAutomaticConfirmations = function () {
            UserResource.automaticmatchconfirmations(function(data, status, headers, config) {
                $scope.automaticMatchConfirmations = data;
            });
        };

        $scope.searchAutomaticConfirmationUsers = function () {
            UserResource.automaticmatchconfirmationsusers({ search: $scope.searchQuery }, function (data, status, headers, config) {
                $scope.automaticMatchConfirmationsUsers = data;
            });
        };

        $scope.addAutomaticConfirmation = function (user, index) {
            UserResource.addautomaticconfirmation(user,
                function (data, status, headers, config) {
                    $scope.automaticMatchConfirmationsUsers.splice(index, 1);
                    $scope.loadAutomaticConfirmations();
                } 
            );
        };

        $scope.deleteAutomaticConfirmation = function (user, index) {
            UserResource.deleteautomaticconfirmation({ confirmeeID: user.userID },
                function (data, status, headers, config) {
                    $scope.automaticMatchConfirmations.splice(index, 1);
                }
            );
        };

        $scope.toggleAddConfirmation = function(show) {
            $scope.addingconfirmation = show;
            if (!show) {
                $scope.automaticMatchConfirmationsUsers = [];
            }
        };

        $scope.loadPlayers();
        $scope.loadTeams();
        $scope.loadMatches();
        $scope.getProfile();
        $scope.loadAutomaticConfirmations();


        // cropping -> move to directive one day

        //an array of files selected
        $scope.changeProfilePictureStep = 0;
        $scope.cropingCoords = null;
        $scope.files = [];

        //listen for the file selected event
        $scope.$on("fileSelected", function (event, args) {
            $scope.$apply(function () {
                $scope.files[0] = args.file;
                $scope.uploadProfilePicture();
            });
        });

        $scope.$on("cropingSelectionChanged", function (event, args) {
            $scope.cropingCoords = args.coords;
        });

        $scope.$on("cropingSelectionReleased", function (event, args) {
            $scope.cropingCoords = null;
        });

        $scope.cropProfileImage = function () {
            if ($scope.cropingCoords != null) {
                $http(
                {
                    method: 'POST',
                    url: 'api/user/cropprofilepicture',
                    data: $scope.cropingCoords,
                }).success(function (data, status, headers, config) {
                    $scope.croppingUrl = '';
                    $scope.profile.profilePictureUrl = data;
                    $scope.changeProfilePictureStep = 0;
                    security.refreshCurrentUser();
                });
            }
        }

        // upload profile picture and show crop screen 
        $scope.uploadProfilePicture = function () {
            console.log('uploading profile picture...');

            var formData = new FormData();
            formData.append("image", $scope.files[0]);
            $http(
            {
                method: 'POST',
                url: 'api/user/uploadprofilepicture',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).success(function (data, status, headers, config) {
                $scope.croppingUrl = data;
                $scope.changeProfilePictureStep = 2;
            });
        };
    }]);