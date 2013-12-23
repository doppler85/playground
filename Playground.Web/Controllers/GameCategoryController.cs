using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Util;
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
        [HttpGet]
        public List<GameCategory> Get()
        {
            List<GameCategory> retVal = Uow.GameCategories.GetAll(p => p.Games).OrderBy(gc => gc.Title).ToList();
            foreach (GameCategory gameCategory in retVal)
            {
                foreach (Game game in gameCategory.Games)
                {
                    game.GamePictureUrl = String.Format("{0}{1}_{2}.{3}?nocache={3}",
                        Constants.Images.GamePictureRoot,
                        Constants.Images.GamePicturePrefix,
                        game.GameID,
                        Constants.Images.GamePictureExtension,
                        DateTime.Now.Ticks);

                }
            }
            return retVal;
        }

        // // api/gamecategory/5
        public GameCategory Get(int id)
        {
            return Uow.GameCategories.GetById(id);
        }

        // POST /api/gamecategory
        [HttpPost]
        public HttpResponseMessage AddGameCategory(GameCategory gameCategory)
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

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            Uow.GameCategories.Delete(id);

            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }
    }
}