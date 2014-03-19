// Based loosely around work by Witold Szczerba - https://github.com/witoldsz/angular-http-auth
angular.module('Playground.security.security-service', [
  'ng',
  'ngRoute',
  'ui.router'
])

.factory('security', [
    '$http',
    '$q',
    '$state',
    '$window',
    'settings',
    function ($http, $q, $state, $window, settings) {
        // Redirect to the given url (defaults to '/')
        // todo: change location to use another provider (ui-router)
        function redirect(state) {
            state = state || 'home';
            //$location.path(url);
            // changeLocation(url);
            $state.transitionTo(state);
        }
        var config = {
            loginUrl: '/Token',
            getExternalLoginsUrl: '/api/account/externallogins',
            externalLoginReturnUrl: '/',
            loginInfoUrl: '/api/account/manageinfo',
            removeLoginUrl: '/api/account/removelogin',
            setPasswordUrl: '/api/account/setpassword',
            resetPasswordUrl: '/api/account/removelogin',
            changePasswordUrl: '/api/account/changepassword',
            registerUrl: '/api/account/register',
            registerExternalUrl: '/api/account/registerexternal',
            logoutUrl: '/api/account/logout',
            currentUserUrl: '/api/account/currentuser',
            addExternalLoginUrl: '/api/account/addexternallogin',
            userInfoUrl: '/api/account/userinfo'
        };
        // The public API of the service
        var service = {

            getExternalLogins: function () {
                var request = $http({
                    method: 'GET',
                    url: settings.apiUrl + config.getExternalLoginsUrl,
                    params: {
                        returnurl: config.externalLoginReturnUrl,
                        generatestate: true
                    }
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            getLoginInfo: function () {
                var request = $http({
                    method: 'GET',
                    url: settings.apiUrl + config.loginInfoUrl,
                    params: {
                        returnurl: config.externalLoginReturnUrl,
                        generatestate: true
                    }
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            // Attempt to authenticate a user by the given username and password
            login: function (loginModel) {
                var xsrf = $.param(loginModel);
                var request = $http({
                    method: 'POST',
                    url: settings.apiUrl + config.loginUrl,
                    data: xsrf,
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded; '
                    }
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            removeLogin: function (login) {
                var request = $http({
                    method: 'POST',
                    url: settings.apiUrl + config.removeLoginUrl,
                    data: login
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            setPassword: function (changePassModel) {
                var request = $http({
                    method: 'POST',
                    url: settings.apiUrl + config.setPasswordUrl,
                    data: changePassModel
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            resetPassword: function (resetPassModel) {
                var request = $http({
                    method: 'POST',
                    url: settings.apiUrl + config.resetPasswordUrl,
                    data: resetPassModel
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            changePassword: function (changePassModel) {
                var request = $http({
                    method: 'POST',
                    url: settings.apiUrl + config.changePasswordUrl,
                    data: changePassModel
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            // Attempt to authenticate a user by the given email and password
            register: function (registerModel) {
                var request = $http({
                    method: 'POST',
                    url: settings.apiUrl + config.registerUrl,
                    data: registerModel
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            registerExternal: function (registerModel) {
                var request = $http({
                    method: 'POST',
                    url: settings.apiUrl + config.registerExternalUrl,
                    data: registerModel
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            // Give up trying to login and clear the retry queue
            cancelLogin: function () {
                redirect();
            },

            // Logout the current user and redirect
            logout: function (redirectTo) {
                $window.localStorage.removeItem("accessToken");
                $window.sessionStorage.removeItem("accessToken");

                $http.post(settings.apiUrl + config.logoutUrl).then(
                    function () {
                        service.currentUser = null;
                        redirect(redirectTo);
                    }
                );
            },

            // Ask the backend to see if a user is already authenticated - this may be from a previous session.
            requestCurrentUser: function () {
                if (service.isAuthenticated()) {
                    return $q.when(service.currentUser);
                } else {
                    return $http.get(settings.apiUrl + config.currentUserUrl).then(
                        function (response) {
                            service.currentUser = response.data == "null" ? null : response.data;
                            return service.currentUser;
                        }
                    );
                }
            },

            refreshCurrentUser: function () {
                return $http.get(settings.apiUrl + config.currentUserUrl).then(
                    function (response) {
                        service.currentUser = response.data == "null" ? null : response.data;
                        return service.currentUser;
                    }
                );
            },

            addExternalLogin: function (externalAccessToken) {
                var request = $http({
                    method: 'POST',
                    url: settings.apiUrl + config.addExternalLoginUrl,
                    data: {
                        externalAccessToken: externalAccessToken
                    }
                });

                return request.then(function (response) {
                    return response.data;
                });
            },

            getUserInfo: function () {
                var request = $http({
                    method: 'GET',
                    url: settings.apiUrl + config.userInfoUrl
                });

                return request.then(function (response) {
                    return response.data;
                });

            },

            redirect: function (state) {
                $state.transitionTo(state);
            },

            // Information about the current user
            currentUser: null,

            // Is the current user authenticated?
            isAuthenticated: function () {
                return service.currentUser != null;
            },

            // Is the current user an adminstrator?
            isAdmin: function () {
                return !!(service.currentUser && service.currentUser.admin);
            }
        };

        return service;
    }]);