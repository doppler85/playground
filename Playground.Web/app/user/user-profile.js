'use strict';
angular.module('Playground.user-profile', ['ngResource', 'ui.router', 'ui.bootstrap.tabs', 'Playground.imageupload'])
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
                            trackDocument: true,
                            onSelect: function (x) {
                                scope.$apply(function () {
                                    scope.selected({ cords: x });
                                });
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

        $scope.single = function (image) {
            var formData = new FormData();
            formData.append('image', image);

            /*
            $http.post('api/user/uploadprofilepicture', {
                data: formData,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            }).success(function (result) {
                $scope.uploadedImgSrc = result.src;
                $scope.sizeInBytes = result.size;
            });
            */
            $http({
                method: 'POST',
                url: "api/user/uploadprofilepicture2",
                transformRequest: angular.identity,
                data: formData,
                headers: { 'Content-Type': undefined }
            });
        };

        $scope.loadPlayers();
        $scope.loadTeams();
        $scope.loadMatches();
        $scope.getProfile();
        $scope.loadAutomaticConfirmations();

        $scope.model = {
            name: "gggg",
            comments: "mmm"
        };

        //an array of files selected
        $scope.files = [];

        //listen for the file selected event
        $scope.$on("fileSelected", function (event, args) {
            $scope.$apply(function () {
                //add the file object to the scope's files collection
                $scope.files.push(args.file);
            });
        });

        $scope.save = function () {
            /*
            var formData = new FormData();
            //need to convert our json object to a string version of json otherwise
            // the browser will do a 'toString()' on the object which will result 
            // in the value '[Object object]' on the server.
            formData.append("model", angular.toJson($scope.model));
            //now add all of the assigned files
            for (var i = 0; i < $scope.files.length; i++) {
                //add each file to the form data and iteratively name them
                formData.append("file" + i, $scope.files[i]);
            }

            $http({ method: 'POST', url: 'api/user/uploadprofilepicture', data: formData, headers: { 'Content-Type': undefined }, transformRequest: angular.identity })
                .success(function (data, status, headers, config) {
            });
            return

            $.ajax({
                type: "POST",
                url: "api/user/uploadprofilepicture",
                contentType: false,
                processData: false,
                data: formData,
                success: function (res) {
                    //do something with our ressponse
                }
            });
            return;
            */
            $http({
                method: 'POST',
                url: "api/user/uploadprofilepicture",
                //IMPORTANT!!! You might think this should be set to 'multipart/form-data' 
                // but this is not true because when we are sending up files the request 
                // needs to include a 'boundary' parameter which identifies the boundary 
                // name between parts in this multi-part request and setting the Content-type 
                // manually will not set this boundary parameter. For whatever reason, 
                // setting the Content-type to 'false' will force the request to automatically
                // populate the headers properly including the boundary parameter.
                headers: { 'Content-Type': undefined },
                //headers: { 'Content-Type': false },
                //This method will allow us to change how the data is sent up to the server
                // for which we'll need to encapsulate the model data in 'FormData'
                transformRequest: function (data) {
                    var formData = new FormData();
                    //need to convert our json object to a string version of json otherwise
                    // the browser will do a 'toString()' on the object which will result 
                    // in the value '[Object object]' on the server.
                    formData.append("model", angular.toJson(data.model));
                    //now add all of the assigned files
                    for (var i = 0; i < data.files.length; i++) {
                        //add each file to the form data and iteratively name them
                        formData.append("file" + i, data.files[i]);
                    }
                    return formData;
                },
                //Create an object that contains the model and files which will be transformed
                // in the above transformRequest method
                data: { model: $scope.model, files: $scope.files }
            }).
            success(function (data, status, headers, config) {
                alert("success!");
            }).
            error(function (data, status, headers, config) {
                alert("failed!");
            });
        };

        
        $scope.single2 = function (image) {
            var formData = new FormData();
            formData.append('image', image, "ggg");

            var xhr = new XMLHttpRequest()
            xhr.open("POST", "api/user/uploadprofilepicture2")
            xhr.send(formData);
        };
        //var xhr = new XMLHttpRequest()
        //xhr.upload.addEventListener("progress", uploadProgress, false)
        //xhr.addEventListener("load", uploadComplete, false)
        //xhr.addEventListener("error", uploadFailed, false)
        //xhr.addEventListener("abort", uploadCanceled, false)
        $scope.setcropping = function (src) {
            $scope.ggg = src;
        };

    }]);