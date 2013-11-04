using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Playground.Web.Controllers
{
    public class GameCategoryController : ApiBaseController
    {
        public GameCategoryController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        // api/gamecategory
        public IEnumerable<GameCategory> Get()
        {
            return Uow.GameCategories.GetAll().OrderBy(gc => gc.Title);
        }

        // // api/gamecategory/5
        public GameCategory Get(int id)
        {
            return Uow.GameCategories.GetById(id);
        }

        // Create a new Session
        // POST /api/session
        public HttpResponseMessage Post(GameCategory gameCategory)
        {
            Uow.GameCategories.Add(gameCategory);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.Created, gameCategory);

            // Compose location header that tells how to get this game category
            // e.g. ~/api/gamecategory/5

            response.Headers.Location =
                new Uri(Url.Link(RouteConfig.ControllerAndId, new { id = gameCategory.GameCategoryID }));

            return response;
        }

    }
}