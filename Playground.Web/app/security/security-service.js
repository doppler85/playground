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
    function ($http, $q, $state, $window) {

    // Redirect to the given url (defaults to '/')
    // todo: change location to use another provider (ui-router)
    function redirect(state) {
        state = state || 'home';
        //$location.path(url);
        // changeLocation(url);
        $state.transitionTo(state);
    }

    // The public API of the service
    var service = {

        // Attempt to authenticate a user by the given email and password
        login: function (loginModel) {
            var request = $http.post('/api/account/login', loginModel);
            return request.then(function (response) {
                service.currentUser = response.data.user;
                return response.data;
            });
        },

        // Attempt to authenticate a user by the given email and password
        register: function (registerModel, userModel) {
            var request = $http.post('/api/account/register', registerModel, userModel);
            return request.then(function (response) {
                service.currentUser = response.data.user;
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

            $http.post('api/account/logout').then(function () {
                service.currentUser = null;
                redirect(redirectTo);
            });
        },

        // Ask the backend to see if a user is already authenticated - this may be from a previous session.
        requestCurrentUser: function () {
            if (service.isAuthenticated()) {
                return $q.when(service.currentUser);
            } else {
                return $http.get('/api/account/currentuser').then(function (response) {
                    service.currentUser = response.data == "null" ? null : response.data;
                    return service.currentUser;
                });
            }
        },

        refreshCurrentUser: function () {
            return $http.get('/api/account/currentuser').then(function (response) {
                service.currentUser = response.data == "null" ? null : response.data;
                return service.currentUser;
            });
        },

        redirect: function(state) {
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