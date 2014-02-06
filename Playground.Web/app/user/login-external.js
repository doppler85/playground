'use strict';
angular.module('Playground.login-external', ['ng', 'ngResource', 'ui.router']).
controller('ExternalLoginController', [
'$location',
'$http',
'$scope',
'$window',
'$state',
'$stateParams',
'$rootScope',
'security',
function ($location, $http, $scope, $window, $state, $stateParams, $rootScope, security) {

    $scope.cleanUpLocation = function (){
        //window.location.hash = "";

        if (typeof (history.pushState) !== "undefined") {
            //history.pushState("", document.title, location.pathname);
        }
    }

    $scope.getFragment = function (){
        if ($window.location.hash.indexOf("#/") === 0) {
            return $scope.parseQueryString($window.location.hash.substr(2));
        } else {
            return {};
        }
    }

    $scope.parseQueryString = function(queryString) {
        var data = {},
            pairs, pair, separatorIndex, escapedKey, escapedValue, key, value;

        if (queryString === null) {
            return data;
        }

        pairs = queryString.split("&");

        for (var i = 0; i < pairs.length; i++) {
            pair = pairs[i];
            separatorIndex = pair.indexOf("=");

            if (separatorIndex === -1) {
                escapedKey = pair;
                escapedValue = null;
            } else {
                escapedKey = pair.substr(0, separatorIndex);
                escapedValue = pair.substr(separatorIndex + 1);
            }

            key = decodeURIComponent(escapedKey);
            value = decodeURIComponent(escapedValue);

            data[key] = value;
        }

        return data;
    }

    $scope.verifyStateMatch = function (fragment) {
        var state;

        if (typeof (fragment.access_token) !== "undefined") {
            state = $window.sessionStorage["state"];
            $window.sessionStorage.removeItem("state");

            if (state === null || fragment.state !== state) {
                fragment.error = "invalid_state";
            }
        }
    }

    $scope.init = function () {
        var fragment = $scope.getFragment(),
            externalAccessToken, externalError, loginUrl;

        $scope.restoreSessionStorageFromLocalStorage();
        $scope.verifyStateMatch(fragment);

        if (sessionStorage["associatingExternalLogin"]) {
            sessionStorage.removeItem("associatingExternalLogin");

            if (typeof (fragment.error) !== "undefined") {
                externalAccessToken = null;
                externalError = fragment.error;
                $scope.cleanUpLocation();
            } else if (typeof (fragment.access_token) !== "undefined") {
                externalAccessToken = fragment.access_token;
                externalError = null;
                $scope.cleanUpLocation();
            } else {
                externalAccessToken = null;
                externalError = null;
                $scope.cleanUpLocation();
            }

            security.refreshCurrentUser().then(
                function (data) {
                    if (data) {
                        security.addExternalLogin(externalAccessToken).then(
                            function (data) {
                                $state.transitionTo('profile.info');
                            },
                            function (error) {
                                $state.transitionTo('login');
                            }
                        )
                    }
                    else {
                        $state.transitionTo('login');
                    }
                },
                function (error) {
                    $state.transitionTo('login');
                }
            );
        } else if (typeof (fragment.error) !== "undefined") {
            $scope.cleanUpLocation();
            $state.transitionTo('login');
            // self.errors.push("External login failed.");
        } else if (typeof (fragment.access_token) !== "undefined") {
            $scope.cleanUpLocation();
            // $state.transitionTo('home');
            $scope.setAccessToken(fragment.access_token, false);
            security.getUserInfo().then(
                function (data) {
                    if (data) {
                        if (data.hasRegistered) {
                            $state.transitionTo('profile.info');
                        }
                        else {
                            $scope.setState(fragment.state, false);
                            $state.transitionTo('register-external');
                        }
                    }
                    else {
                        $scope.clearAccessToken();
                        $state.transitionTo('login');
                    }
                },
                function (error) {
                    $scope.clearAccessToken();
                    $state.transitionTo('login');
                }
            );
        }
    }

    $scope.init();
}]);