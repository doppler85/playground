// Based loosely around work by Witold Szczerba - https://github.com/witoldsz/angular-http-auth
angular.module('Playground.security.security-service', [
  'ngRoute',
  'ui.router',
  'ui.bootstrap.modal',
  'Playground.security.retry-queue',    // Keeps track of failed requests that need to be retried once the user logs in
])

.factory('security', ['$http', '$q', '$location', '$state', 'securityRetryQueue', '$modal', function ($http, $q, $location, $state, queue, $modal) {

    // Redirect to the given url (defaults to '/')
    // todo: change location to use another provider (ui-router)
    function redirect(state) {
        state = state || 'home';
        //$location.path(url);
        // changeLocation(url);
        $state.go(state);
    }

    // Login form dialog stuff
    var loginDialog = null;
    function openLoginDialog() {
        if (!loginDialog) {
            loginDialog = $modal.open(
                {
                    templateUrl: 'app/user/login.tpl.html',
                    controller: 'LoginController'
                });

            loginDialog.result.then(onLoginDialogClose, closeLoginDialog);
        }
    }

    function closeLoginDialog(success) {
        if (loginDialog) {
            loginDialog.close(success);
            loginDialog = null;
        }
    }
    function onLoginDialogClose(success) {
        if (success) {
            queue.retryAll();
        } else {
            queue.cancelAll();
            redirect();
        }
    }

    // Register a handler for when an item is added to the retry queue
    queue.onItemAddedCallbacks.push(function (retryItem) {
        if (queue.hasMore()) {
            service.showLogin();
        }
    });

    // The public API of the service
    var service = {
        // Get the first reason for needing a login
        getLoginReason: function () {
            return queue.retryReason();
        },

        // Show the modal login dialog
        showLogin: function () {
            openLoginDialog();
        },

        // Attempt to authenticate a user by the given email and password
        login: function (loginModel) {
            var request = $http.post('/api/account/login', loginModel);
            return request.then(function (response) {
                service.currentUser = response.data.user;
                if (service.isAuthenticated()) {
                    closeLoginDialog(true);
                }
                return response.data;
            });
        },

        onSucessLogin: function (success) {
            if (success) {
                queue.retryAll();
            } else {
                queue.cancelAll();
                redirect();
            }
        },

        // Give up trying to login and clear the retry queue
        cancelLogin: function () {
            closeLoginDialog(false);
            redirect();
        },

        // Logout the current user and redirect
        logout: function (redirectTo) {
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

        // Information about the current user
        currentUser: null,

        // Is the current user authenticated?
        isAuthenticated: function () {
            return service.currentUser != null;
        },

        // Is the current user an adminstrator?
        isAdmin: function () {
            return !!(service.currentUser && service.currentUser.admin);
        },

        // Information about redirect state
        redirectSateName: null
    };

    return service;
}]);