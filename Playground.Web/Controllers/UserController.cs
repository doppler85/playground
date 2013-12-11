using Playground.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Playground.Model;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace Playground.Web.Controllers
{
    [Authorize]
    public class UserController : ApiBaseController
    {
        public UserController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        private User GetUserByEmail(string email)
        {
            return Uow.Users
                        .GetAll()
                        .FirstOrDefault(u => u.EmailAddress == email);
        }

        // api/user/getplayers
        [HttpGet]
        [ActionName("players")]
        public List<Player> GetPlayers()
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Player> players = Uow.Competitors
                                        .GetAll(p => p.Game, p => p.Game.Category)
                                        .OfType<Player>()
                                        .Where(p => p.UserID == currentUser.UserID)
                                        .OrderBy(p => p.Game.Title)
                                        .ThenBy(p => p.Name)
                                        .ToList();

            return players;
        }

        [HttpPost]
        [ActionName("addplayer")]
        public HttpResponseMessage AddPlayer(Player player)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            player.UserID = currentUser.UserID;
            player.CreationDate = DateTime.Now;
            Uow.Competitors.Add(player);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.Created, player);

            // Compose location header that tells how to get this game 

            response.Headers.Location =
                new Uri(Url.Link(RouteConfig.ControllerAndId, new { id = player.CompetitorID }));

            return response;
        }

        // api/user/getindividualgames
        [HttpGet]
        [ActionName("individualgames")]
        public List<Game> GetIndividualGames()
        {
            List<Game> games = Uow.Games
                                .GetAll(g => g.Category)
                                .Where(g => g.CompetitionTypes.Any(ct => ct.CompetitionType.CompetitorType == CompetitorType.Individual))
                                .OrderBy(g => g.Category.Title)
                                .ThenBy(g => g.Title)
                                .ToList();
            return games;
        }

        public HttpResponseMessage UpdatePlayer()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage DeleteCompetitor(long id)
        {
            List<TeamPlayer> players = Uow.TeamPlayers
                .GetAll()
                .Where(p => p.TeamID == id)
                .ToList();

            foreach (TeamPlayer player in players)
            {
                Uow.TeamPlayers.Delete(tp => tp.PlayerID == player.PlayerID && tp.TeamID == player.TeamID);
            }
            
            Uow.Competitors.Delete(id);
            Uow.Commit();
            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        [HttpGet]
        [ActionName("teams")]
        public List<Team> GetTeams()
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Team> teams = Uow.Competitors
                                        .GetAll(p => p.Game, p => p.Game.Category)
                                        .OfType<Team>()
                                        .Where(t => t.Players.Any(p => p.Player.UserID == currentUser.UserID))
                                        .Distinct()
                                        .OrderBy(t => t.Game.Title)
                                        .ThenBy(t => t.Name)
                                        .ToList();
            return teams;
        }

        [HttpPost]
        [ActionName("addteam")]
        public HttpResponseMessage AddTeam(Team team)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            foreach (TeamPlayer tp in team.Players)
            {
                // put to null to avoid adding of additional players
                tp.Player = null;
            }
            team.CreatorID = currentUser.UserID;
            team.CreationDate = DateTime.Now;
            Uow.Competitors.Add(team);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.Created, team);

            // Compose location header that tells how to get this game 
            response.Headers.Location =
                new Uri(Url.Link(RouteConfig.ControllerAndId, new { id = team.CompetitorID }));

            return response;
        }

        // api/user/teamgames
        [HttpGet]
        [ActionName("teamgames")]
        public List<Game> GetTeamGames()
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Game> games = Uow.Games
                                .GetAll(g => g.Category)
                                .Where(g => g.CompetitionTypes.Any(ct => ct.CompetitionType.CompetitorType == CompetitorType.Team) &&
                                            g.Competitors.Count > 0)
                                .OrderBy(g => g.Category.Title)
                                .ThenBy(g => g.Title)
                                .ToList();

            return games;
        }

        // api/user/myteamplayer
        [HttpGet]
        [ActionName("myteamplayer")]
        public Player MyTeamPlayer(int gameID)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            Player player = Uow.Competitors
                .GetAll()
                .OfType<Player>()
                .Where(p => p.GameID == gameID &&
                    p.User.UserID == currentUser.UserID)
                .FirstOrDefault();

            return player;
        }

        // api/user/searchplayers
        [HttpGet]
        [ActionName("searchplayers")]
        public List<Player> SearchPlayers(int gameID, string search) {
            if (search == null)
            {
                search = String.Empty;

            }
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Player> players = Uow.Competitors
                .GetAll(p => ((Player)p).User)
                .OfType<Player>()
                .Where(p => p.GameID == gameID && 
                            p.User.UserID != currentUser.UserID && 
                            (p.Name.Contains(search) || p.User.FirstName.Contains(search) || p.User.LastName.Contains(search)))
                .ToList();
            

            return players;
        }

        public HttpResponseMessage UpdateTeam()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("matches")]
        public List<Match> GetMatches(int count)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
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

        // api/user/mycompeatinggames
        [HttpGet]
        [ActionName("mycompeatinggames")]
        public List<Game> MyCompeatingGames()
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Competitor> allMycompetitors = new List<Competitor>();
            allMycompetitors.AddRange(GetPlayers());
            allMycompetitors.AddRange(GetTeams());

            List<Game> games = allMycompetitors.Select(g => g.Game).Distinct().ToList();

            return games;
        }
    }
}