'use strict';
Playground.service('Playground.services', ['ngResource', 'ngSanitize', function () {
    var PlaygroundServices = {};
    PlaygroundServices.version = '@VERSION@';

    return PlaygroundServices;
}]);
// examples
/*
factory('ChatResource', ['$resource', function (resource) {
    return resource('api/chat/:actionname/:id', {}, {
        get: { method: 'GET' },
        post: { 
            method: 'POST',
            params: {
                actionname: 'postinfo',
                id: 21
            }
        },
        run: {
            method: 'POST',
            params: {
                actionname: 'startprocess'
            }
        }
    });
}]).
factory('CameraDemoResource', ['$resource', function (resource) {
    return resource('api/camera/:actionname/:id', {}, {
        connect: {
            method: 'GET',
            params: {
                actionname: 'connectcamera'
            }
        },
        getimage: {
            method: 'GET',
            params: {
                actionname: 'getimage'
            }
        }
    });
}]);
*/