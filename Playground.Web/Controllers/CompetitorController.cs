using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Models;
using Playground.Web.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Playground.Web.Controllers
{
    public class CompetitorController : ApiBaseController
    {
        public CompetitorController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        [HttpGet]
        [ActionName("details")]
        public Player PlayerDetails(long id)
        {
            Player retVal = Uow.Competitors
                .GetAll(p => ((Player)p).Games, p => ((Player)p).User)
                .OfType<Player>()
                .Where(p => p.CompetitorID == id)
                .FirstOrDefault();
           
            return retVal;
        }

        [HttpGet]
        [ActionName("getplayerstats")]
        public PlayerStats GetStats(long id)
        {
            Player palyer = PlayerDetails(id);
            PlayerStats retVal = new PlayerStats(palyer);
            List<int> gameIds = retVal.Games
                .Select(p => p.GameID)
                .ToList();
            GameCategory gameCategory = Uow.GameCategories
                .GetAll(gc => gc.Games)
                .FirstOrDefault(gc => gc.Games.Any(g => gameIds.Contains(g.GameID)));
            retVal.GameCategory = gameCategory;
            retVal.TotalMatches = Uow.Matches
                .GetAll()
                .Where(m => m.Scores.Any(s => s.CompetitorID == id))
                .Count();

            return retVal;
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
        public TeamStats GetTeamStats(long id)
        {
            Team team = TeamDetails(id);
            TeamStats retVal = new TeamStats(team);
            List<int> gameIds = retVal.Games
                .Select(p => p.GameID)
                .ToList();
            GameCategory gameCategory = Uow.GameCategories
                .GetAll(gc => gc.Games)
                .FirstOrDefault(gc => gc.Games.Any(g => gameIds.Contains(g.GameID)));
            retVal.GameCategory = gameCategory;
            retVal.TotalMatches = Uow.Matches
                .GetAll()
                .Where(m => m.Scores.Any(s => s.CompetitorID == id))
                .Count();

            return retVal;
        }

        [HttpGet]
        [ActionName("matches")]
        public PagedResult<Match> GetMatches(string id, int page, int count)
        {
            int competitorId = Int32.Parse(id);
            User currentUser = GetUserByEmail(User.Identity.Name);
            int totalItems = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Status == MatchStatus.Confirmed && 
                                                    m.Scores.Any(s => s.CompetitorID == competitorId))
                                        .Count();

            List<Match> matches = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Status == MatchStatus.Confirmed && 
                                                    m.Scores.Any(s => s.CompetitorID == competitorId))
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
        public PagedResult<Player> GetPlayers(string id, int page, int count)
        {
            int teamId = Int32.Parse(id);
            int totalItems = Uow.Competitors
                                        .GetAll(c => ((Player)c).User)
                                        .OfType<Player>()
                                        .Where(c => c.Teams.Any(t => t.TeamID == teamId))
                                        .Count();

            List<Player> players = Uow.Competitors
                                        .GetAll(c => ((Player)c).User, c => c.Games)
                                        .OfType<Player>()
                                        .Where(c => c.Teams.Any(t => t.TeamID == teamId))
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
        public PagedResult<Team> GetTeams(string id, int page, int count)
        {
            int playerId = Int32.Parse(id);
            int totalItems = Uow.Competitors
                                        .GetAll()
                                        .OfType<Team>()
                                        .Where(c => c.Players.Any(p => p.PlayerID == playerId))
                                        .Count();

            List<Team> teams = Uow.Competitors
                                        .GetAll(c => c.Games)
                                        .OfType<Team>()
                                        .Where(c => c.Players.Any(p => p.PlayerID == playerId))
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