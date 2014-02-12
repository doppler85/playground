using Playground.Business.Contracts;
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
    public class CompetitorController : ApiBaseController
    {
        private ICompetitorBusiness competitorBusiness;
        private IGameCategoryBusiness gameCategoryBusiness;
        private IMatchBusiness matchBusiness;
        private IUserBusiness userBusiness;

        public CompetitorController(IPlaygroundUow uow, 
            ICompetitorBusiness cBusiness, 
            IGameCategoryBusiness gcBusiness,
            IMatchBusiness mBusiness,
            IUserBusiness uBusiness)
        {
            this.Uow = uow;
            this.competitorBusiness = cBusiness;
            this.gameCategoryBusiness = gcBusiness;
            this.matchBusiness = mBusiness;
            this.userBusiness = uBusiness;
        }

        [HttpGet]
        [ActionName("getplayerstats")]
        public HttpResponseMessage GetStats(long id)
        {
            Result<Player> playerRes = competitorBusiness.GetPlayerById(id);
            PlayerStats result = null;
            if (playerRes.Sucess)
            {
                result = new PlayerStats(playerRes.Data);
                result.GameCategory = gameCategoryBusiness.GetByCompetitorId(id);
                result.TotalMatches = competitorBusiness.TotalMatchesCount(id);
            }

            HttpResponseMessage response = playerRes.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, result) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, playerRes.Message);

            return response;
        }

        [HttpGet]
        [ActionName("teamdetails")]
        public Team TeamDetails(long id)
        {
            Team retVal = Uow.Competitors
                .GetAll(t => ((Team)t).Games)
                .OfType<Team>()
                .FirstOrDefault(p => p.CompetitorID == id);

            return retVal;
        }

        [HttpGet]
        [ActionName("getteamstats")]
        public HttpResponseMessage GetTeamStats(long id)
        {
            Result<Team> teamRes = competitorBusiness.GetTeamById(id);
            TeamStats result = null;
            if (teamRes.Sucess)
            {
                result = new TeamStats(teamRes.Data);
                result.GameCategory = gameCategoryBusiness.GetByCompetitorId(id);
                result.TotalMatches = competitorBusiness.TotalMatchesCount(id);
            }

            HttpResponseMessage response = teamRes.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, result) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, teamRes.Message);

            return response;
        }

        [HttpGet]
        [ActionName("matches")]
        public HttpResponseMessage GetMatches(int id, int page, int count)
        {
            Result<PagedResult<Match>> res = matchBusiness.FilterByCompetitor(page, count, id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("players")]
        public HttpResponseMessage GetPlayers(long id, int page, int count)
        {
            Result<PagedResult<Player>> res = competitorBusiness.FilterPlayersByTeam(page, count, id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("teams")]
        public HttpResponseMessage GetTeams(long id, int page, int count)
        {
            Result<PagedResult<Team>> res = competitorBusiness.FilterTeamsByPlayer(page, count, id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("allplayers")]
        public HttpResponseMessage GetAllPlayers(int page, int count)
        {
            Result<PagedResult<Player>> res = competitorBusiness.GetPlayers(page, count);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("allteams")]
        public HttpResponseMessage GetAllTeams(int page, int count)
        {
            Result<PagedResult<Team>> res = competitorBusiness.GetTeams(page, count);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("halloffame")]
        public HttpResponseMessage GetHallOFFame(int page, int count)
        {
            DateTime startDate = DateTime.Today.AddDays(-Common.Constants.TopListDays);

            Result<PagedResult<Competitor>> res = competitorBusiness.GetTopCompetitorsByDate(page, count, startDate);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("hallofshame")]
        public HttpResponseMessage GetHallOFShame(int page, int count)
        {
            DateTime startDate = DateTime.Today.AddDays(-Common.Constants.TopListDays);

            Result<PagedResult<Competitor>> res = competitorBusiness.GetBottomCompetitorsByDate(page, count, startDate);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }
    }
}