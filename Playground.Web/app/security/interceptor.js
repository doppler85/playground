//angular.module('Playground.security.interceptor', ['security.retryQueue'])
angular.module('Playground.security.interceptor', ['ng'])
.config(function ($provide, $httpProvider) {

    // Intercept http calls.
    $provide.factory('SecurityInterceptor', function ($injector, $q, $location, $window) {
        return {
            // On request success
            request: function (config) {
                // console.log(config); // Contains the data about the request before it is sent.
                config.headers = config.headers || {};
                var token = $window.localStorage["accessToken"] || $window.sessionStorage["accessToken"];
                if (token) {
                    config.headers.Authorization = 'Bearer ' + token;
                }
                return config;

                // Return the config or wrap it in a promise if blank.
                return config || $q.when(config);
            },

            // On request failure
            requestError: function (rejection) {
                // console.log(rejection); // Contains the data about the error on the request.

                // Return the promise rejection.
                return $q.reject(rejection);
            },

            // On response success
            response: function (response) {
                // console.log(response); // Contains the data from the response.

                // Return the response or promise.
                return response || $q.when(response);
            },

            // On response failture
            responseError: function (rejection) {
                // console.log(rejection); // Contains the data about the error.
                if (rejection.status === 401) {
                    // The request bounced because it was not authorized - add a new request to the retry queue
                    $window.localStorage.removeItem("accessToken");
                    $window.sessionStorage.removeItem("accessToken");
                    $location.path('/login');
                }

                // Return the promise rejection.
                return $q.reject(rejection);
            }
        };
    });

    // Add the interceptor to the $httpProvider.
    $httpProvider.interceptors.push('SecurityInterceptor');
});