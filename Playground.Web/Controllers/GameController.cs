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
        public List<GameCategory> Categories()
        {
            List<GameCategory> games = Uow.GameCategories.GetAll("Games").ToList();
            return games;
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

    }
}
