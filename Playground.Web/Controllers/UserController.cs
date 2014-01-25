using Playground.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Playground.Model;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Web.Hosting;
using System.Net.Http.Headers;
using Playground.Web.Util;
using Playground.Web.Models;
using Playground.Web.Hubs;
using Playground.Business.Contracts;
using Playground.Common;

namespace Playground.Web.Controllers
{
    [Authorize]
    public class UserController : ApiBaseController
    {
        private IMatchBusiness matchBusiness;
        private ICompetitorBusiness competitorBusiness;
        private IUserBusiness userBusiness;

        public UserController(IPlaygroundUow uow, 
            IMatchBusiness mBusiness,
            ICompetitorBusiness cBusiness,
            IUserBusiness uBusiness)
        {
            this.Uow = uow;
            this.matchBusiness = mBusiness;
            this.competitorBusiness = cBusiness;
            this.userBusiness = uBusiness;
        }

        private string GetPlayerPicturesRootFolder()
        {
            return HttpContext.Current.Server.MapPath(String.Format("~{0}", Constants.Images.PlayerPictureRoot));
        }

        private string GetTeamPicturesRootFolder()
        {
            return HttpContext.Current.Server.MapPath(String.Format("~{0}", Constants.Images.TeamPictureRoot));
        }

        private string GetProfilePicturesRootFolder()
        {
            return HttpContext.Current.Server.MapPath(String.Format("~{0}", Constants.Images.ProfilePictureRoot));
        }

        // api/user/users
        [HttpGet]
        [ActionName("users")]
        public HttpResponseMessage GetUsers(int page, int count)
        {
            Result<PagedResult<User>> res =
                userBusiness.GetUsers(page, count);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/user/players
        [HttpGet]
        [ActionName("players")]
        public HttpResponseMessage GetPlayers(int page, int count)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;

            Result<PagedResult<Player>> res = competitorBusiness.GetPlayersForUser(page, count, currentUser.UserID);
            if (res.Sucess)
            {
                competitorBusiness.LoadCategories(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/user/addplayer
        [HttpPost]
        [ActionName("addplayer")]
        public HttpResponseMessage AddPlayer(Player player)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            player.UserID = currentUser.UserID;
            Result<Player> res = competitorBusiness.AddPlayer(player);

            if (res.Sucess)
            {
                competitorBusiness.AssignImage(player,
                    currentUser.UserID,
                    GetPlayerPicturesRootFolder(),
                    Constants.Images.PlayerPictureRoot,
                    Constants.Images.PlayerPicturePrefix,
                    Constants.Images.PlayerPictureExtension);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpPut]
        [ActionName("updateplayer")]
        public HttpResponseMessage UpdatePlayer(Player player)
        {
            Result<Player> res = competitorBusiness.UpdatePlayer(player);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("getupdateplayer")]
        public HttpResponseMessage GetUpdatePlayer(long id)
        {
            Result<Player> res = competitorBusiness.GetUpdatePlayer(id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage DeleteCompetitor(long id)
        {
            bool res = competitorBusiness.DeleteCompetitor(id);

            HttpResponseMessage response = res ?
                 Request.CreateResponse(HttpStatusCode.OK) :
                 Request.CreateResponse(HttpStatusCode.InternalServerError, "Error deleting competitor");

            return response;
        }

        [HttpGet]
        [ActionName("teams")]
        public HttpResponseMessage GetTeams(int page, int count)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;

            Result<PagedResult<Team>> res = competitorBusiness.GetTeamsForUser(page, count, currentUser.UserID);
            if (res.Sucess)
            {
                competitorBusiness.LoadCategories(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpPost]
        [ActionName("addteam")]
        public HttpResponseMessage AddTeam(Team team)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            team.CreatorID = currentUser.UserID;

            Result<Team> res = competitorBusiness.AddTeam(team);

            if (res.Sucess)
            {
                competitorBusiness.AssignImage(team,
                    currentUser.UserID,
                    GetTeamPicturesRootFolder(),
                    Constants.Images.TeamPictureRoot,
                    Constants.Images.TeamPicturePrefix,
                    Constants.Images.TeamPictureExtension);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpPut]
        [ActionName("updateteam")]
        public HttpResponseMessage UpdateTeam(Team team)
        {
            Result<Team> res = competitorBusiness.UpdateTeam(team);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("getupdateteam")]
        public HttpResponseMessage GetUpdateTeam(long id)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            
            Result<Team> res = competitorBusiness.GetUpdateTeam(id, currentUser.UserID);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/user/myteamplayer
        [HttpGet]
        [ActionName("myteamplayer")]
        public Player MyTeamPlayer(int gameCategoryID)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
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

            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            List<Player> players = Uow.Competitors
                .GetAll(p => ((Player)p).User)
                .OfType<Player>()
                .Where(p => p.Games.Any(g => g.Game.Category.GameCategoryID == gameCategoryID) &&
                            p.User.UserID != currentUser.UserID &&
                            (p.Name.Contains(search) || p.User.FirstName.Contains(search) || p.User.LastName.Contains(search)))
                .ToList();

            return players;
        }

        // api/user/searchplayers
        [HttpGet]
        [ActionName("searchteamplayers")]
        public List<Player> SearchPlayers([FromUri]long teamId, [FromUri]string search)
        {
            if (search == null)
            {
                search = String.Empty;
            }
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            Team team = Uow.Competitors
                .GetAll(t => ((Team)t).Players, t => t.Games)
                .OfType<Team>()
                .First(t => t.CompetitorID == teamId);
            List<int> gameIds = team.Games.Select(g => g.GameID).ToList();
            List<Game> games = Uow.Games
                .GetAll(g => g.Category)
                .Where(g => gameIds.Contains(g.GameID))
                .ToList();

            int gameCategoryID = games[0].GameCategoryID;
            List<long> teamPlayerIds = team.Players.Select(tp => tp.PlayerID).ToList();
            List<Player> players = Uow.Competitors
                .GetAll(p => ((Player)p).User)
                .OfType<Player>()
                .Where(p => p.Games.Any(g => g.Game.Category.GameCategoryID == gameCategoryID) &&
                            p.User.UserID != currentUser.UserID &&
                            !teamPlayerIds.Contains(p.CompetitorID) &&
                            (p.Name.Contains(search) || p.User.FirstName.Contains(search) || p.User.LastName.Contains(search)))
                .ToList();

            return players;
        }

        [HttpPost]
        [ActionName("addteamplayer")]
        public HttpResponseMessage AddTeamPlayer(TeamPlayer teamPlayer)
        {
            Uow.TeamPlayers.Add(teamPlayer);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.Created, teamPlayer);
            return response;
        }

        [HttpDelete]
        [ActionName("deleteteamplayer")]
        public HttpResponseMessage DeleteTeamPlayer(long teamID, long playerID)
        {
            Uow.TeamPlayers.Delete(tp => tp.TeamID == teamID && tp.PlayerID == playerID);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }



        [HttpGet]
        [ActionName("matches")]
        public HttpResponseMessage GetMatches(int page, int count)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;

            Result<PagedResult<Match>> res =
                matchBusiness.FilterByUser(page, count, currentUser.UserID);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        
        [HttpGet]
        [AllowAnonymous]
        [ActionName("publicmatches")]
        public HttpResponseMessage GetMatches(int id, int page, int count)
        {
            Result<PagedResult<Match>> res =
                matchBusiness.FilterByStatusAndUser(page, count, MatchStatus.Confirmed, id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("publicplayers")]
        public HttpResponseMessage GetPlayers(int id, int page, int count)
        {
            Result<PagedResult<Player>> res =
                competitorBusiness.GetPlayersForUser(page, count, id);

            if (res.Sucess)
            {
                competitorBusiness.LoadUsers(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("publicteams")]
        public HttpResponseMessage GetTeams(int id, int page, int count)
        {
            Result<PagedResult<Team>> res =
                competitorBusiness.GetTeamsForUser(page, count, id);

            if (res.Sucess)
            {
                competitorBusiness.LoadCategories(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/user/mycompeatinggames
        [HttpGet]
        [ActionName("mycompeatinggames")]
        public List<GameCategory> MyCompeatingGames()
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            List<Competitor> allMycompetitors = new List<Competitor>();
            List<GameCategory> categories = Uow.Competitors
                                        .GetAll(p => p.Games)
                                        .OfType<Player>()
                                        .Where(p => p.UserID == currentUser.UserID)
                                        .SelectMany(p => p.Games)
                                        .Select(g => g.Game.Category)
                                        .Distinct()
                                        .ToList();
            return categories;
        }

        // api/user/mycompeatinggames
        [HttpGet]
        [ActionName("mycompeatitors")]
        public List<Competitor> MyCompetitors(int gameCategoryID)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            List<Competitor> allMycompetitors = new List<Competitor>();

            List<Player> players = Uow.Competitors
                                        .GetAll(p => p.Games, p => ((Player)p).Teams)
                                        .OfType<Player>()
                                        .Where(p => p.UserID == currentUser.UserID && 
                                                    p.Games.Any(g => g.Game.GameCategoryID == gameCategoryID))
                                        .OrderBy(p => p.Name)
                                        .ToList();

            foreach (Player player in players)
            {
                // serialization issue (we dont nee whole user here);
                player.User = null;
                allMycompetitors.Add(player);
                foreach (TeamPlayer teamPlayer in player.Teams)
                {
                    Team team = (Team)Uow.Competitors.GetById(t => t.CompetitorID == teamPlayer.TeamID, t => t.Games);
                    allMycompetitors.Add(team);
                }
            }

            // explicitelly load only relevant data
            foreach (Competitor competitor in allMycompetitors)
            {
                foreach (GameCompetitor gameCompetitor in competitor.Games)
                {
                    Game game = Uow.Games.GetById(g => g.GameID == gameCompetitor.GameID, g => g.CompetitionTypes);
                    gameCompetitor.Game = new Game()
                    {
                        GameID = game.GameID,
                        GameCategoryID = game.GameCategoryID,
                        Title = game.Title,
                        CompetitionTypes = new List<GameCompetitionType>()
                    };
                    foreach (GameCompetitionType gct in game.CompetitionTypes)
                    {
                        CompetitionType competitonType = Uow.CompetitionTypes.GetById(ct => ct.CompetitionTypeID == gct.CompetitionTypeID);
                        gameCompetitor.Game.CompetitionTypes.Add(new GameCompetitionType()
                        {
                            CompetitionTypeID = competitonType.CompetitionTypeID,
                            CompetitionType = new CompetitionType()
                            {
                                CompetitionTypeID = competitonType.CompetitionTypeID,
                                CompetitorType = competitonType.CompetitorType,
                                Name = competitonType.Name,
                                CompetitorsCount = competitonType.CompetitorsCount
                            }
                        });
                    }
                }
            }

            return allMycompetitors;
        }

        // api/user/searchcompetitors
        [HttpGet]
        [ActionName("searchcompetitors")]
        public PagedResult<Competitor> SearchCompetitors([FromUri]SearchCompetitorArgs args)
        {
            if (args.Search == null)
            {
                args.Search = String.Empty;
            }
            
            List<Competitor> competitors = Uow.GameCompetitors
                .GetAll(gc => gc.Competitor)
                .Where(g => !args.Ids.Contains(g.CompetitorID) &&
                            g.Game.GameCategoryID == args.GameCategoryID && 
                            g.Competitor.CompetitorType == (CompetitorType)args.CompetitorType && 
                            g.Competitor.Name.Contains(args.Search))
                .OrderBy(g => g.Competitor.Name)
                .Select(g => g.Competitor)
                .Skip((args.Page - 1) * args.Count)
                .Take(args.Count)
                .ToList();

            int totalItems = Uow.GameCompetitors
                .GetAll(gc => gc.Competitor)
                .Where(g => !args.Ids.Contains(g.CompetitorID) &&
                            g.Game.GameCategoryID == args.GameCategoryID &&
                            g.Competitor.CompetitorType == (CompetitorType)args.CompetitorType &&
                            g.Competitor.Name.Contains(args.Search))
                .Select(g => g.Competitor)
                .Count();


            PagedResult<Competitor> retVal = new PagedResult<Competitor>()
            {
                CurrentPage = args.Page,
                TotalPages = (totalItems + args.Count - 1) / args.Count,
                TotalItems = totalItems,
                Items = competitors
            };

            return retVal;
        }

        private bool CheckMyCompetitor(long competitorID)
        {
            bool retVal = false;
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
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
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            Competitor competitor = Uow.Competitors.GetById(competitorID);
            if (competitor is Player)
            {
                int competitorUserid = ((Player)competitor).UserID;
                AutomaticMatchConfirmation amc = Uow.AutomaticMatchConfirmations
                    .GetAll()
                    .FirstOrDefault(a => a.ConfirmeeID == currentUser.UserID && 
                                         a.ConfirmerID == competitorUserid);
                
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
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
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

            int totalMatches = Uow.Matches.GetAll().Where(m => m.Status == MatchStatus.Confirmed).Count();
            LiveScores.Instance.BroadcastTotalMatches(totalMatches);

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
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            if (!String.IsNullOrEmpty(currentUser.PictureUrl))
            {
                currentUser.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
            }
            else
            {
                currentUser.PictureUrl = currentUser.Gender == Gender.Male ?
                    Constants.Images.DefaultProfileMale :
                    Constants.Images.DefaultProfileFemale;
            }
            return currentUser;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("getdetails")]
        public User GetDetails(long id)
        {
            User user = Uow.Users.GetById(id);
            if (!String.IsNullOrEmpty(user.PictureUrl))
            {
                user.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
            }
            else
            {
                user.PictureUrl = user.Gender == Gender.Male ?
                    Constants.Images.DefaultProfileMale :
                    Constants.Images.DefaultProfileFemale;
            }
            return user;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("getuserstats")]
        public UserStats GetUserStats(long id)
        {
            User user = GetDetails(id);
            UserStats retVal = new UserStats(user);
            retVal.TotalGames = Uow.Competitors
                .GetAll()
                .OfType<Player>()
                .Where(c => c.UserID == user.UserID)
                .SelectMany(c => c.Games)
                .Select(g => g.Game.GameCategoryID)
                .Distinct()
                .Count();

            List<int> gameids = Uow.Competitors
                .GetAll()
                .OfType<Player>()
                .Where(c => c.UserID == user.UserID)
                .SelectMany(c => c.Games)
                .Select(g => g.GameID)
                .Distinct()
                .ToList();

            retVal.TotalPlayers = Uow.Competitors
                .GetAll()
                .OfType<Player>()
                .Where(p => p.UserID == user.UserID)
                .Count();

            retVal.TotalTeams = Uow.Competitors
                .GetAll()
                .OfType<Team>()
                .Where(t => t.Players.Any(p => p.Player.UserID == user.UserID))
                .Count();

            List<Player> players = Uow.Competitors
                .GetAll(p => ((Player)p).Teams)
                .OfType<Player>()
                .Where(p => p.UserID == user.UserID)
                .ToList();

            List<long> matchids = new List<long>();
            foreach (Player player in players) 
            {
                matchids.AddRange(Uow.Matches
                    .GetAll()
                    .Where(m => m.Status == MatchStatus.Confirmed &&
                        m.Scores.Any(p => p.CompetitorID == player.CompetitorID))
                    .Select(m => m.MatchID));
                    
                foreach (TeamPlayer tp in player.Teams)
                {
                    matchids.AddRange(Uow.Matches
                    .GetAll()
                    .Where(m => m.Status == MatchStatus.Confirmed && 
                        m.Scores.Any(t => t.CompetitorID == tp.TeamID))
                    .Select(m => m.MatchID));
                }
            }
            retVal.TotalMatches = matchids.Distinct().Count();


            return retVal;
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
        public PagedResult<AutomaticMatchConfirmation> AutomaticMatchConfirmations(int page, int count)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;

            List<AutomaticMatchConfirmation> confirmations = Uow.AutomaticMatchConfirmations
                .GetAll(ac => ac.Confirmee, ac => ac.Confirmer)
                .Where(ac => ac.ConfirmerID == currentUser.UserID)
                .OrderBy(ac => ac.Confirmee.FirstName)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            int totalItems = Uow.AutomaticMatchConfirmations
                .GetAll()
                .Where(ac => ac.ConfirmerID == currentUser.UserID)
                .Count();

            foreach (AutomaticMatchConfirmation confirmation in confirmations)
            {
                if (!String.IsNullOrEmpty(confirmation.Confirmee.PictureUrl))
                {
                    confirmation.Confirmee.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
                }
                else
                {
                    confirmation.Confirmee.PictureUrl = confirmation.Confirmee.Gender == Gender.Male ?
                        Constants.Images.DefaultProfileMale :
                        Constants.Images.DefaultProfileFemale;
                }
            }

            PagedResult<AutomaticMatchConfirmation> retVal = new PagedResult<AutomaticMatchConfirmation>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = confirmations
            };

            return retVal;
        }

        [HttpGet]
        [ActionName("automaticmatchconfirmationsusers")]
        public PagedResult<User> AutomaticMatchConfirmationUsers([FromUri]SearchArgs args)
        {
            if (args.Search == null)
            {
                args.Search = String.Empty;
            }

            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            List<User> users = Uow.Users
                .GetAll()
                .Except(
                    Uow.AutomaticMatchConfirmations.GetAll()
                    .Where(ac => ac.ConfirmerID == currentUser.UserID)
                    .Select(ac => ac.Confirmee))
                .Where(u => u.UserID != currentUser.UserID && 
                        (u.FirstName.Contains(args.Search) || u.LastName.Contains(args.Search)))
                .OrderBy(u => u.FirstName)
                .Skip((args.Page - 1) * args.Count)
                .Take(args.Count)
                .ToList();

            int totalItems = Uow.Users
                .GetAll()
                .Except(
                    Uow.AutomaticMatchConfirmations.GetAll()
                    .Where(ac => ac.ConfirmerID == currentUser.UserID)
                    .Select(ac => ac.Confirmee))
                .Where(u => u.UserID != currentUser.UserID &&
                        (u.FirstName.Contains(args.Search) || u.LastName.Contains(args.Search)))
                .Count();

            PagedResult<User> retVal = new PagedResult<User>()
            {
                CurrentPage = args.Page,
                TotalPages = (totalItems + args.Count - 1) / args.Count,
                TotalItems = totalItems,
                Items = users
            };
                
            return retVal;
        }

        [HttpPost]
        [ActionName("addautomaticconfirmation")]
        public HttpResponseMessage AddAutomaticConfirmation(User user)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
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
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
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

        [HttpPost]
        [ActionName("uploadplayerpicture")]
        public async Task<HttpResponseMessage> UploadPlayerPicture()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = GetPlayerPicturesRootFolder();
            var provider = new UniqueMultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names for uploaded files.
                User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;


                if (String.IsNullOrEmpty(provider.FormData["ID"]))
                {
                    throw new Exception("No ID for the game specified");
                }
                int playerId = Int32.Parse(provider.FormData["ID"]);
                string retUrl = "";
                if (provider.FileData.Count > 0)
                {
                    string localName = provider.FileData[0].LocalFileName;
                    FileInfo fileInfo = new FileInfo(localName);
                    string filePath = String.Format("{0}{1}_{2}_{3}_temp.{4}",
                                            root,
                                            Constants.Images.PlayerPicturePrefix,
                                            currentUser.UserID,
                                            playerId,
                                            Constants.Images.PlayerPictureExtension);

                    ImageUtil.ScaleImage(fileInfo.FullName,
                                        filePath,
                                        Constants.Images.PlayerImageFormat,
                                        Constants.Images.PlayerImageMaxSize,
                                        Constants.Images.PlayerImageMaxSize);

                    fileInfo.Delete();

                    fileInfo = new FileInfo(filePath);
                    retUrl = String.Format("{0}{1}", Constants.Images.PlayerPictureRoot, fileInfo.Name);
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(String.Format("{0}?nocache={1}", retUrl, DateTime.Now.Ticks))
                };
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        [ActionName("cropplayerpicture")]
        public HttpResponseMessage CropPlayerPicture(CropingArgs coords)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            string root = GetPlayerPicturesRootFolder();
            string filePath = String.Format("{0}{1}_{2}_{3}_temp.{4}",
                                                        root,
                                                        Constants.Images.PlayerPicturePrefix,
                                                        currentUser.UserID,
                                                        coords.ID,
                                                        Constants.Images.PlayerPictureExtension);

            string destFilePath = String.Format("{0}{1}_{2}_{3}.{4}",
                                                                    root,
                                                                    Constants.Images.PlayerPicturePrefix,
                                                                    currentUser.UserID,
                                                                    coords.ID,
                                                                    Constants.Images.PlayerPictureExtension);
            try
            {
                ImageUtil.CropImage(filePath,
                                    destFilePath,
                                    Constants.Images.PlayerImageFormat,
                                    coords);

                // delete temporary file
                FileInfo fileInfo = new FileInfo(filePath);
                fileInfo.Delete();

                fileInfo = new FileInfo(destFilePath);
                string retUrl = String.Format("{0}{1}", Constants.Images.PlayerPictureRoot, fileInfo.Name);
                // if all ok update user
                Competitor player = Uow.Competitors.GetById(coords.ID);
                if (player != null)
                {
                    player.PictureUrl = retUrl;
                    Uow.Competitors.Update(player, player.CompetitorID);
                    Uow.Commit();
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(String.Format("{0}?nocache={1}", retUrl, DateTime.Now.Ticks))
                };
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        [ActionName("uploadteampicture")]
        public async Task<HttpResponseMessage> UploadTeamPicture()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = GetTeamPicturesRootFolder();
            var provider = new UniqueMultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names for uploaded files.
                User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;


                if (String.IsNullOrEmpty(provider.FormData["ID"]))
                {
                    throw new Exception("No ID for the game specified");
                }
                int teamId = Int32.Parse(provider.FormData["ID"]);
                string retUrl = "";
                if (provider.FileData.Count > 0)
                {
                    string localName = provider.FileData[0].LocalFileName;
                    FileInfo fileInfo = new FileInfo(localName);
                    string filePath = String.Format("{0}{1}_{2}_{3}_temp.{4}",
                                            root,
                                            Constants.Images.TeamPicturePrefix,
                                            currentUser.UserID,
                                            teamId,
                                            Constants.Images.PlayerPictureExtension);

                    ImageUtil.ScaleImage(fileInfo.FullName,
                                        filePath,
                                        Constants.Images.TeamImageFormat,
                                        Constants.Images.TeamImageMaxSize,
                                        Constants.Images.TeamImageMaxSize);

                    fileInfo.Delete();

                    fileInfo = new FileInfo(filePath);
                    retUrl = String.Format("{0}{1}", Constants.Images.TeamPictureRoot, fileInfo.Name);
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(String.Format("{0}?nocache={1}", retUrl, DateTime.Now.Ticks))
                };
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        [ActionName("cropteampicture")]
        public HttpResponseMessage CropTeamPicture(CropingArgs coords)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            string root = GetTeamPicturesRootFolder();
            string filePath = String.Format("{0}{1}_{2}_{3}_temp.{4}",
                                                        root,
                                                        Constants.Images.TeamPicturePrefix,
                                                        currentUser.UserID,
                                                        coords.ID,
                                                        Constants.Images.TeamPictureExtension);

            string destFilePath = String.Format("{0}{1}_{2}_{3}.{4}",
                                                                    root,
                                                                    Constants.Images.TeamPicturePrefix,
                                                                    currentUser.UserID,
                                                                    coords.ID,
                                                                    Constants.Images.TeamPictureExtension);
            try
            {
                ImageUtil.CropImage(filePath,
                                    destFilePath,
                                    Constants.Images.TeamImageFormat,
                                    coords);

                // delete temporary file
                FileInfo fileInfo = new FileInfo(filePath);
                fileInfo.Delete();

                fileInfo = new FileInfo(destFilePath);
                string retUrl = String.Format("{0}{1}", Constants.Images.TeamPictureRoot, fileInfo.Name);
                // if all ok update user
                Competitor team = Uow.Competitors.GetById(coords.ID);
                if (team != null)
                {
                    team.PictureUrl = retUrl;
                    Uow.Competitors.Update(team, team.CompetitorID);
                    Uow.Commit();
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(String.Format("{0}?nocache={1}", retUrl, DateTime.Now.Ticks))
                };
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        [ActionName("uploadprofilepicture")]
        public async Task<HttpResponseMessage> UploadProfilePicture()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = GetProfilePicturesRootFolder();
            var provider = new UniqueMultipartFormDataStreamProvider(root);
            
            try
            {
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names for uploaded files.
                User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;

                string retUrl = "";
                if (provider.FileData.Count > 0)
                {
                    string localName = provider.FileData[0].LocalFileName;
                    FileInfo fileInfo = new FileInfo(localName);
                    string filePath = String.Format("{0}{1}_{2}_temp.{3}", 
                                            root, 
                                            Constants.Images.ProfilePicturePrefix,
                                            currentUser.UserID,
                                            Constants.Images.ProfilePictureExtension);

                    ImageUtil.ScaleImage(fileInfo.FullName, 
                                        filePath, 
                                        Constants.Images.ProfileImageFormat,
                                        Constants.Images.ProfileImageMaxSize, 
                                        Constants.Images.ProfileImageMaxSize);
                    
                    fileInfo.Delete();

                    fileInfo = new FileInfo(filePath);
                    retUrl = String.Format("{0}{1}", Constants.Images.ProfilePictureRoot, fileInfo.Name);
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(String.Format("{0}?nocache={1}", retUrl, DateTime.Now.Ticks))
                };
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        [ActionName("cropprofilepicture")]
        public HttpResponseMessage CropProfilePicture(CropingArgs coords)
        {
            User currentUser = userBusiness.GetUserByEmail(User.Identity.Name).Data;
            string root = GetProfilePicturesRootFolder();
            string filePath = String.Format("{0}{1}_{2}_temp.{3}",
                                                        root,
                                                        Constants.Images.ProfilePicturePrefix,
                                                        currentUser.UserID,
                                                        Constants.Images.ProfilePictureExtension);

            string destFilePath = String.Format("{0}{1}_{2}.{3}",
                                                                    root,
                                                                    Constants.Images.ProfilePicturePrefix,
                                                                    currentUser.UserID,
                                                                    Constants.Images.ProfilePictureExtension);
            try 
            {
                ImageUtil.CropImage(filePath,
                                    destFilePath,
                                    Constants.Images.ProfileImageFormat,
                                    coords);
                
                // delete temporary file
                FileInfo fileInfo = new FileInfo(filePath);
                fileInfo.Delete();

                fileInfo = new FileInfo(destFilePath);
                string retUrl = String.Format("{0}{1}", Constants.Images.ProfilePictureRoot, fileInfo.Name);
                // if all ok update user picture
                currentUser.PictureUrl = retUrl;
                Uow.Users.Update(currentUser, currentUser.UserID);
                Uow.Commit();

                return new HttpResponseMessage()
                {
                    Content = new StringContent(String.Format("{0}?nocache={1}", retUrl, DateTime.Now.Ticks))
                };
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }

    public class UniqueMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public UniqueMultipartFormDataStreamProvider(string path)
            : base(path)
        { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
            name = name.Insert(0, Guid.NewGuid().ToString());

            //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
            return name.Replace("\"", string.Empty);
        }
    }
}