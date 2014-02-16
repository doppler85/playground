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
        },
        halloffame: {
            method: 'GET',
            params: {
                actionname: 'halloffame'
            }
        },
        hallofshame: {
            method: 'GET',
            params: {
                actionname: 'hallofshame'
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
        getupdategame: {
            method: 'GET',
            isArray: false,
            params: {
                actionname: 'getupdategame'
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
        remove: { method: 'DELETE',
            params: {
                actionname: 'deletegame'
            }
        },
        individualcategories: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'individualcategories'
            }
        },
        individualgames: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'individualgames'
            }
        },
        teamcategories: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'teamcategories'
            }
        },
        teamgames: {
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
        getcompetitiontype: {
            method: 'GET',
            params: {
                actionname: 'getcompetitiontype'
            }
        },
        getallcompetitiontypes: {
            method: 'GET',
            isArray: true,
            params: {
                actionname: 'gealltcompetitiontypes'
            }
        },
        getcompetitiontypes: {
            method: 'GET',
            params: {
                actionname: 'getcompetitiontypes'
            }
        },
        add: {
            method: 'POST',
            params: {
                actionname: 'addcompetitiontype'
            }
        },
        update: {
            method: 'PUT',
            params: {
                actionname: 'updatecompetitiontype'
            }
        },
        remove: {
            method: 'DELETE',
            params: {
                actionname: 'deletecompetitiontype'
            }
        }
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
        searchplayersbycategory: {
            method: 'GET',
            params: {
                actionname: 'searchplayersbycategory'
            }
        },
        searchteamplayers: {
            method: 'GET',
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
        },
        users: {
            method: 'GET',
            params: {
                actionname: 'users'
            }
        }
    });
}]).
factory('HomeResource', ['$resource', function (resource) {
    return resource('api/home/:actionname/:id', {}, {
        lastmatches: {
            method: 'GET',
            params: {
                actionname: 'matches'
            }
        },
        lastcompetitors: {
            method: 'GET',
            params: {
                actionname: 'competitors'
            }
        }
    });
}]).
factory('PlaygroundResource', ['$resource', function (resource) {
    return resource('api/playground/:actionname/:id', {}, {
        getplaygrounds: {
            method: 'GET',
            params: {
                actionname: 'getplaygrounds'
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