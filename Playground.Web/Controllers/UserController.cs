using Playground.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Playground.Model;
using System.Web.Http;
using System.Net.Http;

namespace Playground.Web.Controllers
{
    [Authorize]
    public class UserController : ApiBaseController
    {
        public UserController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        // api/user/getplayers
        [HttpGet]
        [ActionName("getplayers")]
        public List<Player> GetPlayers()
        {
            User currentUser = Uow.Users.GetUserByEmail(User.Identity.Name);
            List<Player> players = Uow.Competitors
                                        .GetAll(p => p.Game)
                                        .OfType<Player>()
                                        .Where(p => p.UserID == currentUser.UserID)
                                        .OrderBy(p => p.Game.Title)
                                        .ThenBy(p => p.Name)
                                        .ToList();

            return players;
        }

        public HttpResponseMessage AddPlayer()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage UpdatePlayer()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage DeletePlayer()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("getteams")]
        public List<Team> GetTeams()
        {
            User currentUser = Uow.Users.GetUserByEmail(User.Identity.Name);
            List<Team> teams = Uow.Competitors
                                        .GetAll(p => p.Game)
                                        .OfType<Team>()
                                        .Where(t => t.Players.Any(p => p.PlayerID == currentUser.UserID))
                                        .Distinct()
                                        .OrderBy(t => t.Game.Title)
                                        .ThenBy(t => t.Name)
                                        .ToList();
            return teams;
        }

        public HttpResponseMessage AddTeam()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage UpdateTeam()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage DeleteTeam()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("gettmatches")]
        public List<Match> GetMatches(int count)
        {
            User currentUser = Uow.Users.GetUserByEmail(User.Identity.Name);
            List<long> teamsIds = Uow.Competitors
                                        .GetAll()
                                        .OfType<Team>()
                                        .Where(t => t.Players.Any(p => p.PlayerID == currentUser.UserID))
                                        .Select(t => t.CompetitorID)
                                        .Distinct()
                                        .ToList();
            List<long> playerIds = Uow.Competitors
                                        .GetAll()
                                        .OfType<Player>()
                                        .Where(p => p.UserID == currentUser.UserID)
                                        .Select(p => p.CompetitorID)
                                        .ToList();

            List<long> ids = teamsIds.Concat(playerIds).ToList();

            List<Match> matches = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Scores
                                        .Any(s => ids.Contains(s.CompetitorID)))
                                        .OrderByDescending(s => s.Date)
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
            
            return matches;
        }
    }
}