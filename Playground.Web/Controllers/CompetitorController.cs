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
        private IUserBusiness userBusiness;

        public CompetitorController(IPlaygroundUow uow, 
            ICompetitorBusiness cBusiness, 
            IGameCategoryBusiness gcBusiness,
            IUserBusiness uBusiness)
        {
            this.Uow = uow;
            this.competitorBusiness = cBusiness;
            this.gameCategoryBusiness = gcBusiness;
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
        public PagedResult<Match> GetMatches(int id, int page, int count)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            int totalItems = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Status == MatchStatus.Confirmed && 
                                                    m.Scores.Any(s => s.CompetitorID == id))
                                        .Count();

            List<Match> matches = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Status == MatchStatus.Confirmed && 
                                                    m.Scores.Any(s => s.CompetitorID == id))
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

        [HttpGet]
        [ActionName("players")]
        public PagedResult<Player> GetPlayers(long id, int page, int count)
        {
            int totalItems = Uow.Competitors
                                        .GetAll(c => ((Player)c).User)
                                        .OfType<Player>()
                                        .Where(c => c.Teams.Any(t => t.TeamID == id))
                                        .Count();

            List<Player> players = Uow.Competitors
                                        .GetAll(c => ((Player)c).User, c => c.Games)
                                        .OfType<Player>()
                                        .Where(c => c.Teams.Any(t => t.TeamID == id))
                                        .OrderByDescending(c => c.CreationDate)
                                        .Skip((page - 1) * count)
                                        .Take(count)
                                        .ToList();
            
            PagedResult<Player> retVal = new PagedResult<Player>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = players
            };

            return retVal;
        }

        [HttpGet]
        [ActionName("teams")]
        public PagedResult<Team> GetTeams(long id, int page, int count)
        {
            int totalItems = Uow.Competitors
                                        .GetAll()
                                        .OfType<Team>()
                                        .Where(c => c.Players.Any(p => p.PlayerID == id))
                                        .Count();

            List<Team> teams = Uow.Competitors
                                        .GetAll(c => c.Games)
                                        .OfType<Team>()
                                        .Where(c => c.Players.Any(p => p.PlayerID == id))
                                        .OrderByDescending(c => c.CreationDate)
                                        .Skip((page - 1) * count)
                                        .Take(count)
                                        .ToList();

            List<int> gameIds = teams.SelectMany(t => t.Games).Select(g => g.GameID).ToList();
            List<Game> games = Uow.Games
                                        .GetAll(g => g.Category)
                                        .Where(g => gameIds.Contains(g.GameID))
                                        .ToList();

            PagedResult<Team> retVal = new PagedResult<Team>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = teams
            };

            return retVal;
        }

        [HttpGet]
        [ActionName("allplayers")]
        public PagedResult<Player> GetAllPlayers(int page, int count)
        {
            int totalItems = Uow.Competitors
                                        .GetAll(c => ((Player)c).User)
                                        .OfType<Player>()
                                        .Count();

            List<Player> players = Uow.Competitors
                                        .GetAll(c => ((Player)c).User, c => c.Games)
                                        .OfType<Player>()
                                        .OrderByDescending(c => c.CreationDate)
                                        .Skip((page - 1) * count)
                                        .Take(count)
                                        .ToList();

            PagedResult<Player> retVal = new PagedResult<Player>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = players
            };

            return retVal;
        }

        [HttpGet]
        [ActionName("allteams")]
        public PagedResult<Team> GetAllTeams(int page, int count)
        {
            int totalItems = Uow.Competitors
                                        .GetAll()
                                        .OfType<Team>()
                                        .Count();

            List<Team> teams = Uow.Competitors
                                        .GetAll(c => c.Games)
                                        .OfType<Team>()
                                        .OrderByDescending(c => c.CreationDate)
                                        .Skip((page - 1) * count)
                                        .Take(count)
                                        .ToList();

            List<int> gameIds = teams.SelectMany(t => t.Games).Select(g => g.GameID).ToList();
            List<Game> games = Uow.Games
                                        .GetAll(g => g.Category)
                                        .Where(g => gameIds.Contains(g.GameID))
                                        .ToList();

            PagedResult<Team> retVal = new PagedResult<Team>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = teams
            };

            return retVal;
        }
    }
}