using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Models;
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


        [HttpGet]
        [ActionName("matches")]
        public PagedResult<Match> GetMatches(string id, int page, int count)
        {
            int gameCategoryId = Int32.Parse(id);
            User currentUser = GetUserByEmail(User.Identity.Name);
            int totalItems = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Game.GameCategoryID == gameCategoryId && m.Status == MatchStatus.Confirmed)
                                        .Count();

            List<Match> matches = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Game.GameCategoryID == gameCategoryId && m.Status == MatchStatus.Confirmed)
                                        .OrderByDescending(s => s.Date)
                                        .Skip((page - 1) * count)
                                        .Take(count)
                                        .ToList();


            List<long> competitorIds = matches
                .SelectMany(m => m.Scores)
                .ToList()
                .Select(s => s.CompetitorID)
                .ToList();

            List<Competitor> competitors = Uow.Competitors
                .GetAll()
                .Where(c => competitorIds.Contains(c.CompetitorID))
                .ToList();

            PagedResult<Match> retVal = new PagedResult<Match>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = matches
            };

            return retVal;
        }

    }
}