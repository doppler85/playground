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
                                        .GetAll(p => p.Games)
                                        .OfType<Player>()
                                        .Where(p => p.UserID == currentUser.UserID)
                                        .OrderBy(p => p.Name)
                                        .ToList();

            List<int> gameIds = players
                .SelectMany(p => p.Games)
                .ToList()
                .Select(g => g.GameID)
                .ToList();

            // fetch game categories for players
            List<Game> games = Uow.Games
                .GetAll(g => g.Category)
                .Where(g => gameIds.Contains(g.GameID))
                .ToList();

            return players;
        }

        [HttpPost]
        [ActionName("addplayer")]
        public HttpResponseMessage AddPlayer(Player player)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            player.UserID = currentUser.UserID;
            player.CompetitorType = CompetitorType.Individual;
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
        public List<GameCategory> GetIndividualGames()
        {
            List<Game> games = Uow.Games
                                .GetAll(g => g.Category)
                                .Where(g => g.CompetitionTypes.Any(ct => ct.CompetitionType.CompetitorType == CompetitorType.Individual))
                                .OrderBy(g => g.Category.Title)
                                .ThenBy(g => g.Title)
                                .ToList();

            List<GameCategory> categories = games
                .Select(g => g.Category)
                .Distinct()
                .ToList();
            
            return categories;
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
                Uow.TeamPlayers.Delete(player);
            }

            List<GameCompetitor> gameCompetitors = Uow.GameCompetitors
                .GetAll()
                .Where(gc => gc.CompetitorID == id)
                .ToList();
            foreach (GameCompetitor gameCompetitor in gameCompetitors)
            {
                Uow.GameCompetitors.Delete(gameCompetitor);
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
                                        .GetAll(t => t.Games)
                                        .OfType<Team>()
                                        .Where(t => t.Players.Any(p => p.Player.UserID == currentUser.UserID))
                                        .Distinct()
                                        .OrderBy(t => t.Name)
                                        .ToList();
            
            List<int> gameIds = teams
                .SelectMany(t => t.Games)
                .ToList()
                .Select(g => g.GameID)
                .ToList();

            // fetch game categories for players
            List<Game> games = Uow.Games
                .GetAll(g => g.Category)
                .Where(g => gameIds.Contains(g.GameID))
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
            team.CompetitorType = CompetitorType.Team;
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
        public List<GameCategory> GetTeamGames()
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Game> games = Uow.Games
                                .GetAll(g => g.Category)
                                .Where(g => g.CompetitionTypes.Any(ct => ct.CompetitionType.CompetitorType == CompetitorType.Team) &&
                                            g.Competitors.Count > 0)
                                .OrderBy(g => g.Category.Title)
                                .ThenBy(g => g.Title)
                                .ToList();

            List<GameCategory> categories = games
                .Select(g => g.Category)
                .Distinct()
                .ToList();

            return categories;
        }

        // api/user/myteamplayer
        [HttpGet]
        [ActionName("myteamplayer")]
        public Player MyTeamPlayer(int gameCategoryID)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            Player player = Uow.Competitors
                .GetAll()
                .OfType<Player>()
                .Where(p => p.Games.Any(g => g.Game.GameCategoryID == gameCategoryID) &&
                    p.User.UserID == currentUser.UserID)
                .FirstOrDefault();

            return player;
        }

        // api/user/searchplayers
        [HttpGet]
        [ActionName("searchplayers")]
        public List<Player> SearchPlayers(int gameCategoryID, string search) {
            if (search == null)
            {
                search = String.Empty;
            }

            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Player> players = Uow.Competitors
                .GetAll(p => ((Player)p).User)
                .OfType<Player>()
                .Where(p => p.Games.Any(g => g.Game.Category.GameCategoryID == gameCategoryID) &&
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
                                        .Where(t => t.Players.Any(p => p.Player.User.UserID == currentUser.UserID))
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

            foreach (Competitor competitor in competitors)
            {
                if (ids.Contains(competitor.CompetitorID))
                {
                    competitor.IsCurrentUserCompetitor = true;
                }
            }
            
            return matches;
        }

        // api/user/mycompeatinggames
        [HttpGet]
        [ActionName("mycompeatinggames")]
        public List<GameCategory> MyCompeatingGames()
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Competitor> allMycompetitors = new List<Competitor>();
            allMycompetitors.AddRange(GetPlayers());
            allMycompetitors.AddRange(GetTeams());

            List<int> gameIds = allMycompetitors
                .SelectMany(c => c.Games)
                .ToList()
                .Select(g => g.GameID)
                .Distinct()
                .ToList();

            List<Game> games = Uow.Games
                .GetAll(g => g.Category)
                .Where(g => gameIds.Contains(g.GameID))
                .ToList();

            List<GameCategory> categories = games
                .Select(g => g.Category)
                .Distinct()
                .ToList();

            return categories;
        }

        // api/user/mycompeatinggames
        [HttpGet]
        [ActionName("mycompeatitors")]
        public List<Competitor> MyCompetitors(int gameCategoryID)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Competitor> allMycompetitors = new List<Competitor>();

            List<Player> players = Uow.Competitors
                                        .GetAll(p => p.Games, p => ((Player)p).Teams)
                                        .OfType<Player>()
                                        .Where(p => p.UserID == currentUser.UserID && 
                                                    p.Games.Any(g => g.Game.GameCategoryID == gameCategoryID))
                                        .OrderBy(p => p.Name)
                                        .ToList();
            List<long> teamIds = players
                .SelectMany(p => p.Teams)
                .ToList()
                .Select(t => t.TeamID)
                .ToList();

            List<Team> teams = Uow.Competitors
                                        .GetAll(t => t.Games)
                                        .OfType<Team>()
                                        .Where(t => teamIds.Contains(t.CompetitorID))
                                        .ToList();

            foreach (Player p in players)
            {
                allMycompetitors.Add(p);
            }

            foreach (Team t in teams)
            {
                allMycompetitors.Add(t);
            }

            List<int> gameIds = allMycompetitors
                .SelectMany(p => p.Games)
                .ToList()
                .Select(g => g.GameID)
                .ToList();

            List<Game> games = Uow.Games
                                    .GetAll(g => g.CompetitionTypes)
                                    .Where(g => gameIds.Contains(g.GameID))
                                    .ToList();

            List<GameCompetitionType> competitionTypes = Uow.GameCompetitionTypes.GetAll(gc => gc.CompetitionType)
                .Where(gc => gameIds.Contains(gc.GameID))
                .ToList();

            return allMycompetitors;
        }

        // api/user/searchcompetitors
        [HttpGet]
        [ActionName("searchcompetitors")]
        public List<Competitor> SearchCompetitors(int gameCategoryID, int competitorType, string search)
        {
            if (search == null)
            {
                search = String.Empty;
            }

            List<GameCompetitor> gameCompetitors = Uow.GameCompetitors
                .GetAll(gc => gc.Competitor)
                .Where(g => g.Game.GameCategoryID == gameCategoryID && 
                            g.Competitor.CompetitorType == (CompetitorType)competitorType && 
                            g.Competitor.Name.Contains(search))
                .Distinct()
                .ToList();

            List<Competitor> retVal = gameCompetitors
                                        .Select(gc => gc.Competitor)
                                        .Distinct()
                                        .ToList();

            return retVal;
        }

        private bool CheckMyCompetitor(long competitorID)
        {
            bool retVal = false;
            User currentUser = GetUserByEmail(User.Identity.Name);
            Competitor competitor = Uow.Competitors.GetById(competitorID);
            retVal = (competitor is Player && ((Player)competitor).UserID == currentUser.UserID);
            if (!retVal && competitor is Team)
            {
                retVal = Uow.Competitors
                    .GetAll()
                    .OfType<Team>()
                    .Where(t => t.CompetitorID == competitorID && 
                        t.Players.Any(p => p.Player.UserID == currentUser.UserID))
                    .Count() > 0;
            }

            return retVal;
        }

        private bool CheckConfirmation(long competitorID)
        {
            bool retVal = false;
            User currentUser = GetUserByEmail(User.Identity.Name);
            Competitor competitor = Uow.Competitors.GetById(competitorID);
            if (competitor is Player)
            {
                int competitorUserid = ((Player)competitor).UserID;
                AutomaticMatchConfirmation amc = Uow.AutomaticMatchConfirmations
                    .GetAll()
                    .FirstOrDefault(a => a.ConfirmeeID == currentUser.UserID && 
                                         a.ConfirmerID == competitorID);
                
                retVal = amc != null;
            }
            else
            {
                List<int> userIds = Uow.TeamPlayers
                    .GetAll(tp => tp.Player)
                    .Where(tp => tp.TeamID == competitorID)
                    .Select(tp => tp.Player.UserID)
                    .ToList();

                AutomaticMatchConfirmation amc = Uow.AutomaticMatchConfirmations
                    .GetAll()
                    .FirstOrDefault(a => a.ConfirmeeID == currentUser.UserID &&
                                         userIds.Contains(a.ConfirmerID));

                retVal = amc != null;
            }
            return retVal;
        }

        [HttpPost]
        [ActionName("addmatch")]
        public HttpResponseMessage AddMath(Match match)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            foreach (CompetitorScore competitorScore in match.Scores)
            {
                if (CheckMyCompetitor(competitorScore.CompetitorID))
                {
                    competitorScore.Confirmed = true;
                }
                else if (CheckConfirmation(competitorScore.CompetitorID))
                {
                    competitorScore.Confirmed = true;
                }
            }

            match.CreatorID = currentUser.UserID;
            match.WinnerID = match.Scores.OrderByDescending(s => s.Score).First().CompetitorID;
            match.Status = match.Scores.Count(s => !s.Confirmed) > 0 ? MatchStatus.Submited : MatchStatus.Confirmed;
            Uow.Matches.Add(match);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.Created, match);

            // Compose location header that tells how to get this game 
            response.Headers.Location =
                new Uri(Url.Link(RouteConfig.ControllerAndId, new { id = match.MatchID }));

            return response;
        }

        [HttpGet]
        [ActionName("getprofile")]
        public User GetProfile()
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            return currentUser;
        }

        [HttpPut]
        [ActionName("updateprofile")]
        public HttpResponseMessage UpdateProfile(User user)
        {
            User userToUpdate = Uow.Users.GetById(user.UserID);
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Gender = user.Gender;

            Uow.Users.Update(userToUpdate);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.OK, userToUpdate);
            return response;
        }

        [HttpGet]
        [ActionName("automaticmatchconfirmations")]
        public List<AutomaticMatchConfirmation> AutomaticMatchConfirmations()
        {
            User currentUser = GetUserByEmail(User.Identity.Name);

            List<AutomaticMatchConfirmation> retVal = Uow.AutomaticMatchConfirmations
                .GetAll(ac => ac.Confirmee, ac => ac.Confirmer)
                .Where(ac => ac.ConfirmerID == currentUser.UserID)
                .ToList();

            return retVal;
        }

        [HttpGet]
        [ActionName("automaticmatchconfirmationsusers")]
        public List<User> AutomaticMatchConfirmationUsers(string search)
        {
            if (search == null)
            {
                search = String.Empty;
            }
            
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<User> retVal = Uow.Users
                .GetAll()
                .Except(
                    Uow.AutomaticMatchConfirmations.GetAll()
                    .Where(ac => ac.ConfirmerID == currentUser.UserID)
                    .Select(ac => ac.Confirmee))
                .Where(u => u.UserID != currentUser.UserID && 
                        (u.FirstName.Contains(search) || u.LastName.Contains(search)))
                .ToList();
                
            return retVal;
        }

        [HttpPost]
        [ActionName("addautomaticconfirmation")]
        public HttpResponseMessage AddAutomaticConfirmation(User user)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            AutomaticMatchConfirmation amc = new AutomaticMatchConfirmation()
            {
                ConfirmeeID = user.UserID,
                ConfirmerID = currentUser.UserID
            };
            Uow.AutomaticMatchConfirmations.Add(amc);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.Created, amc);

            return response;
        }

        [HttpDelete]
        [ActionName("deleteautomaticconfirmation")]
        public HttpResponseMessage DeleteAutomaticConfirmation(int confirmeeID)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            Uow.AutomaticMatchConfirmations.Delete(ac => ac.ConfirmeeID == confirmeeID &&
                                                         ac.ConfirmerID == currentUser.UserID);
            Uow.Commit();
                
            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        [HttpPut]
        [ActionName("confirmscore")]
        public HttpResponseMessage ConfirmScore(CompetitorScore competitorScore)
        {
            Uow.CompetitorScores.Update(competitorScore, competitorScore.CompetitorID, competitorScore.MatchID);
            Uow.Commit();

            bool matchConfirmed = !Uow.CompetitorScores
                .GetAll()
                .Any(cs => cs.MatchID == competitorScore.MatchID &&
                           !cs.Confirmed);
            Match match = Uow.Matches.GetById(competitorScore.MatchID);

            if (matchConfirmed)
            {
                match.Status = MatchStatus.Confirmed;
                Uow.Matches.Update(match, match.MatchID);
                Uow.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, match);

            return response;
        }
    }
}