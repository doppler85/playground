﻿using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Playground.Web.Controllers
{  
    public class GameController : ApiBaseController
    {
        public GameController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        private List<GameCompetitionType> GetByGameId(int gameID)
        {
            return Uow.GameCompetitionTypes
                                        .GetAll(ct => ct.CompetitionType)
                                        .Where(ct => ct.GameID == gameID)
                                        .ToList();
        }

        [HttpGet]
        [ActionName("details")]
        public Game GameDetails(int id)
        {
            Game retVal = Uow.Games.GetById(id);
            retVal.Category = Uow.GameCategories.GetById(retVal.GameCategoryID);
            // retVal.CompetitionTypes = Uow.GameCompetitionTypes.GetByGameId(retVal.GameID).ToList();
            retVal.CompetitionTypes = GetByGameId(retVal.GameID);
                                            

            return retVal;
        }

        [HttpGet]
        [ActionName("availablecomptypes")]
        public List<GameCompetitionType> AvailableCompetitionTypes(int id)
        {
            List<GameCompetitionType> retVal = new List<GameCompetitionType>();
            IQueryable<CompetitionType> availableCompetitionTypes = Uow.CompetitionTypes
                .GetAll()
                .Where(ct => !ct.Games.Any(g => g.GameID == id))
                .Distinct();
            foreach (CompetitionType ct in availableCompetitionTypes)
            {
                retVal.Add(new GameCompetitionType()
                {
                    CompetitionType = ct,
                    CompetitionTypeID = ct.CompetitionTypeID,
                    GameID = id
                });
            }
            
            return retVal;
        }


        // Create a new Game
        // POST /api/Game
        [HttpPost]
        public HttpResponseMessage AddGame(Game game)
        {
            Uow.Games.Add(game);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.Created, game);

            // Compose location header that tells how to get this game 
            // e.g. ~/api/game/5

            response.Headers.Location =
                new Uri(Url.Link(RouteConfig.ControllerAndId, new { id = game.GameID }));

            return response;
        }

        // Create a new Game
        // POST /api/game/updategame
        [HttpPut]
        [ActionName("updategame")]
        public HttpResponseMessage UpdateGame(Game game)
        {
            // clear prvious competition types
            List<GameCompetitionType> currentCompetitionTypes = GetByGameId(game.GameID); 
            foreach (GameCompetitionType ct in currentCompetitionTypes)
            {
                Uow.GameCompetitionTypes.Delete(ct);
            }
            foreach (GameCompetitionType ct in game.CompetitionTypes)
            {
                Uow.GameCompetitionTypes.Add(ct);
            }

            Uow.Games.Update(game, game.GameID);

            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.OK, game);

            return response;
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            Uow.Games.Delete(id);
            Uow.Commit();
            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }
    }
}
