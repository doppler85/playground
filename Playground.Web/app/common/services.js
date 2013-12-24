'use strict';
Playground.service('Playground.services', ['ngResource', 'ngSanitize', function () {
    var PlaygroundServices = {};
    PlaygroundServices.version = '@VERSION@';

    return PlaygroundServices;
}]).
factory('GameCategoryResource', ['$resource', function (resource) {
    return resource('api/gamecategory/:actionname/:id', {}, {
        getall: { method: 'GET', isArray: true },
        add: { method: 'POST' },
        remove: { method: 'DELETE' }
    });
}]).
factory('GameResource', ['$resource', function (resource) {
    return resource('api/game/:actionname/:id', {}, {
        details: {
            method: 'GET',
            isArray: false,
            params: {
                actionname: 'details'
            }
        },
        availablecomptypes: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'availablecomptypes'
            }
        },
        add: { method: 'POST' },
        update: { method: 'PUT' },
        remove: { method: 'DELETE' },
    });
}]).
factory('CompetitionTypeResource', ['$resource', function (resource) {
    return resource('api/competitiontype/:actionname/:id', {}, {
        all: { method: 'GET', isArray: true },
        add: { method: 'POST' }
    });
}]).
factory('UserResource', ['$resource', function (resource) {
    return resource('api/user/:actionname/:id', {}, {
        allPlayers: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'players'
            }
        },
        allTeams: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'teams'
            }
        },
        allMatches: {
            method: 'GET',
            params: {
                actionname: 'matches'
            }
        },
        individualGames: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'individualgames'
            }
        },
        teamGames: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'teamgames'
            }
        },
        myteamplayer: {
            method: 'GET',
            params: {
                actionname: 'myteamplayer'
            }
        },
        searchplayers: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'searchplayers'
            }
        },
        addPlayer: {
            method: 'POST',
            params: {
                actionname: 'addplayer'
            }
        },
        addTeam: {
            method: 'POST',
            params: {
                actionname: 'addteam'
            }
        },
        deletecompetitor: {
            method: 'DELETE',
            params: {
                actionname: 'delete'
            }
        },
        mycompeatinggames: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'mycompeatinggames'
            }
        },
        mycompeatitors: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'mycompeatitors'
            }
        },
        searchcompetitors: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'searchcompetitors'
            }
        },
        addmatch: {
            method: 'POST',
            params: {
                actionname: 'addmatch'
            }
        },
        getprofile: {
            method: 'GET',
            params: {
                actionname: 'getprofile'
            }
        },
        updateprofile: {
            method: 'PUT',
            params: {
                actionname: 'updateprofile'
            }
        },
        confirmscore: {
            method: 'PUT',
            params: {
                actionname: 'confirmscore'
            }
        },
        automaticmatchconfirmations: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'automaticmatchconfirmations'
            }
        },
        automaticmatchconfirmationsusers: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'automaticmatchconfirmationsusers'
            }
        },
        addautomaticconfirmation: {
            method: 'POST',
            params: {
                actionname: 'addautomaticconfirmation'
            }
        },
        deleteautomaticconfirmation: {
            method: 'DELETE',
            params: {
                actionname: 'deleteautomaticconfirmation'
            }
        }
    });
}]).
factory('HomeResource', ['$resource', function (resource) {
    return resource('api/home/:actionname/:id', {}, {
        lastMatches: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'matches'
            }
        },
        lastCompetitors: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'competitors'
            }
        }
    });
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