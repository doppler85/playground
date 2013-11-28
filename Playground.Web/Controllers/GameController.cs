using Playground.Data.Contracts;
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

        [HttpGet]
        [ActionName("details")]
        public Game GameDetails(int id)
        {
            Game retVal = Uow.Games.GetById(id);
            retVal.Category = Uow.GameCategories.GetById(retVal.GameCategoryID);
            retVal.CompetitionTypes = Uow.GameCompetitionTypes.GetByGameId(retVal.GameID).ToList();

            return retVal;
        }

        [HttpGet]
        [ActionName("availablecomptypes")]
        public List<GameCompetitionType> AvailableCompetitionTypes(int id)
        {
            List<GameCompetitionType> retVal = new List<GameCompetitionType>();
            IQueryable<CompetitionType> availableCompetitionTypes = Uow.CompetitionTypes.GetAvailableByGameId(id);
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
        // POST /api/Game/SaveGame
        [HttpPut]
        [ActionName("savegame")]
        public HttpResponseMessage SaveGame(Game game)
        {
            // clear prvious competition types
            List<GameCompetitionType> currentCompetitionTypes = Uow.GameCompetitionTypes.GetByGameId(game.GameID).ToList();
            foreach (GameCompetitionType ct in currentCompetitionTypes)
            {
                Uow.GameCompetitionTypes.Delete(ct);
            }
            foreach (GameCompetitionType ct in game.CompetitionTypes)
            {
                Uow.GameCompetitionTypes.Add(ct);
            }

            Uow.Games.Update(game);

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
