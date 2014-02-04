//angular.module('Playground.security.interceptor', ['security.retryQueue'])
angular.module('Playground.security.interceptor', ['ng', 'Playground.security.retry-queue'])

// This http interceptor listens for authentication failures
// .factory('securityInterceptor', ['$injector', 'securityRetryQueue', function ($injector, queue) {
.factory('securityInterceptor', ['$injector', 'securityRetryQueue', '$window', function ($injector, queue, $window) {
    return function (promise) {
        // Intercept failed requests
        //return promise.then(null, function(originalResponse) {
        return promise.then(function (orginalResponse) {
            console.log('this request is ok');
            return orginalResponse;
        },
        function (originalResponse) {
            if (originalResponse.status === 401) {
                // The request bounced because it was not authorized - add a new request to the retry queue
                $window.localStorage.removeItem("accessToken");
                $window.sessionStorage.removeItem("accessToken");

                promise = queue.pushRetryFn('unauthorized-server', function retryRequest() {
                    // We must use $injector to get the $http service to prevent circular dependency
                    return $injector.get('$http')(originalResponse.config);
                });
            }
            return promise;
        });
    };
}])

.factory('authInterceptor', function ($rootScope, $q, $window) {
    return {
        request: function (config) {
            config.headers = config.headers || {};
            var token = $window.localStorage["accessToken"] || $window.sessionStorage["accessToken"];
            if (token) {
                config.headers.Authorization = 'Bearer ' + token;
            }
            return config;
        },
        response: function (response) {
            if (response.status === 401) {
                // handle the case where the user is not authenticated
                $window.localStorage.removeItem("accessToken");
                $window.sessionStorage.removeItem("accessToken");
            }
            return response || $q.when(response);
        }
    };
})

// We have to add the interceptor to the queue as a string because the interceptor depends upon service instances that are not available in the config block.
.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptor');
    $httpProvider.responseInterceptors.push('securityInterceptor');
}]);