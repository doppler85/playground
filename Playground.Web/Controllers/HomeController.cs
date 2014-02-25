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
        private ICompetitorBusiness competitorBusiness;
        private IPlaygroundBusiness playgroundBusiness;
        private IGameBusiness gameBusiness;
        private IUserBusiness userBusiness;

        public HomeController(IMatchBusiness mBusiness, 
            ICompetitorBusiness cBusiness,
            IPlaygroundBusiness pgBusiness,
            IGameBusiness gBusiness,
            IUserBusiness uBusiness)
        {
            this.matchBusiness = mBusiness;
            this.competitorBusiness = cBusiness;
            this.playgroundBusiness = pgBusiness;
            this.gameBusiness = gBusiness;
            this.userBusiness = uBusiness;
        }

        [HttpGet]
        [ActionName("getstats")]
        public HttpResponseMessage GetStats()
        {
            Playground.Model.ViewModel.PlaygroundStats res = new Model.ViewModel.PlaygroundStats()
            {
                TotalPlaygrounds = playgroundBusiness.TotalPlaygroundsCound(),
                TotalGames = gameBusiness.TotalGamesCount(),
                TotalUsers = userBusiness.TotalUsersCount(),
                TotalPlayers = competitorBusiness.TotalCompetitorsCount(),
                TotalMatches = matchBusiness.TotalMatchesByStatus(MatchStatus.Confirmed)
            };
            
            
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);

            return response;
        }

        [HttpGet]
        [ActionName("matches")]
        public HttpResponseMessage GetMatches(int page, int count)
        {
            Result<PagedResult<Match>> res =
                matchBusiness.FilterByStatus(page, count, MatchStatus.Confirmed);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("competitors")]
        public HttpResponseMessage GetCompetitors(int page, int count)
        {
            Result<PagedResult<Competitor>> res = competitorBusiness.GetCompetitors(page, count);
            if (res.Success)
            {
                competitorBusiness.LoadCategories(res.Data.Items);
            }

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }
    }
}
