'use strict';
Playground.service('Playground.services', ['ngResource', 'ngSanitize', function () {
    var PlaygroundServices = {};
    PlaygroundServices.version = '@VERSION@';

    return PlaygroundServices;
}]).
factory('GameCategoryResource', ['$resource', function (resource) {
    return resource('api/gamecategory/:actionname/:id', {}, {
        getall: {
            method: 'GET',
            params: {
                actionname: 'allcategories'
            }
        },
        getcategory: {
            method: 'GET',
            params: {
                actionname: 'getcategory'
            }
        },
        getgames: {
            method: 'GET',
            params: {
                actionname: 'games'
            }
        },
        add: {
            method: 'POST',
            params: {
                actionname: 'addgamecategory'
            }
        },
        update: {
            method: 'PUT',
            params: {
                actionname: 'updategamecategory'
            }
        },
        stats: {
            method: 'GET',
            params: {
                actionname: 'getcategorystats'
            }
        },
        remove: { method: 'DELETE' },
    });
}]).
factory('CompetitorResource', ['$resource', function (resource) {
    return resource('api/competitor/:actionname/:id', {}, {
        playerstats: {
            method: 'GET',
            params: {
                actionname: 'getplayerstats'
            }
        },
        teamstats: {
            method: 'GET',
            params: {
                actionname: 'getteamstats'
            }
        }
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
        add: {
            method: 'POST',
            params: {
                actionname: 'addgame'
            }
        },
        update: {
            method: 'PUT', params: {
                actionname: 'updategame'
            }
        },
        stats: {
            method: 'GET',
            params: {
                actionname: 'getgamestats'
            }
        },
        remove: { method: 'DELETE' },
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
        }
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
            params: {
                actionname: 'players'
            }
        },
        allTeams: {
            method: 'GET',
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
        searchteamplayers: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'searchteamplayers'
            }
        },
        addPlayer: {
            method: 'POST',
            params: {
                actionname: 'addplayer'
            }
        },
        updatePlayer: {
            method: 'PUT',
            params: {
                actionname: 'updateplayer'
            }
        },
        getUpdatePlayer: {
            method: 'GET',
            params: {
                actionname: 'getupdateplayer'
            }
        },
        addTeam: {
            method: 'POST',
            params: {
                actionname: 'addteam'
            }
        },
        updateTeam: {
            method: 'PUT',
            params: {
                actionname: 'updateteam'
            }
        },
        addteamplayer: {
            method: 'POST',
            params: {
                actionname: 'addteamplayer'
            }
        },
        deleteteamplayer: {
            method: 'DELETE',
            params: {
                actionname: 'deleteteamplayer'
            }
        },
        getUpdateTeam: {
            method: 'GET',
            params: {
                actionname: 'getupdateteam'
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
            params: {
                actionname: 'automaticmatchconfirmations'
            }
        },
        automaticmatchconfirmationsusers: {
            method: 'GET',
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
        },
        getuserstats: {
            method: 'GET',
            params: {
                actionname: 'getuserstats'
            }
        }
    });
}]).
factory('HomeResource', ['$resource', function (resource) {
    return resource('api/home/:actionname/:id', {}, {
        lastMatches: {
            method: 'GET',
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