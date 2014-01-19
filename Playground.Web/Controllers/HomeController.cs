using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Playground.Web.Controllers
{
    public class HomeController : ApiBaseController
    {
        private IMatchBusiness matchBusiness;

        public HomeController(IPlaygroundUow uow, IMatchBusiness mBusiness)
        {
            this.Uow = uow;
            this.matchBusiness = mBusiness;
        }

        [HttpGet]
        [ActionName("matches")]
        public HttpResponseMessage GetMatches(int page, int count)
        {
            Result<PagedResult<Match>> res =
                matchBusiness.FilterByStatus(page, count, MatchStatus.Confirmed);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("competitors")]
        public PagedResult<Competitor> GetCompetitors(int page, int count)
        {
            List<Competitor> competitors = Uow.Competitors
                                                .GetAll(c => c.Games)
                                                .OrderByDescending(c => c.CreationDate)
                                                .Skip((page - 1) * count)
                                                .Take(count)
                                                .ToList();

            int totalItems = Uow.Competitors
                                        .GetAll()
                                        .Count();

            List<int> gameIds = competitors
                                                .SelectMany(c => c.Games)
                                                .ToList()
                                                .Select(g => g.GameID)
                                                .ToList();

            List<Game> categories = Uow.Games
                                                .GetAll(g => g.Category)
                                                .Where(g => gameIds.Contains(g.GameID))
                                                .ToList();

            PagedResult<Competitor> retVal = new PagedResult<Competitor>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = competitors
            };

            return retVal;
        }
    }
}
