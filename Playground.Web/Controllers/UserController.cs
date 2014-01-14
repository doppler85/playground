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

namespace Playground.Web.Controllers
{
    [Authorize]
    public class UserController : ApiBaseController
    {
        public UserController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        [HttpGet]
        [ActionName("users")]
        public PagedResult<User> GetUsers(int page, int count)
        {
            List<User> users = Uow.Users
                .GetAll()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            int totalItems = Uow.Users
                .GetAll()
                .Count();

            foreach (User user in users)
            {
                if (!String.IsNullOrEmpty(user.PictureUrl))
                {
                    user.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
                }
                else
                {
                    user.PictureUrl = user.Gender == Gender.Male ?
                        Util.Constants.Images.DefaultProfileMale :
                        Util.Constants.Images.DefaultProfileFemale;
                }
            }

            PagedResult<User> retVal = new PagedResult<User>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = users
            };

            return retVal;
        }

        // api/user/getplayers
        [HttpGet]
        [ActionName("players")]
        public PagedResult<Player> GetPlayers(int page, int count)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Player> players = Uow.Competitors
                                        .GetAll(p => ((Player)p).Games)
                                        .OfType<Player>()
                                        .Where(p => p.UserID == currentUser.UserID)
                                        .OrderBy(p => p.Name)
                                        .Skip((page - 1) * count)
                                        .Take(count)
                                        .ToList();


            int totalItems = Uow.Competitors
                .GetAll()
                .OfType<Player>()
                .Where(p => p.UserID == currentUser.UserID)
                .Count();
            
            // load only relevat data to increase eficiency
            foreach (Player player in players)
            {
                // we dont need whole user object here
                player.User = null; 
                if (player.Games.Count > 0)
                {
                    int gameID = player.Games[0].GameID;

                    player.Games[0].Game = new Game() 
                    {
                        GameID = gameID
                    };

                    player.Games[0].Game.Category = Uow.GameCategories
                        .GetAll()
                        .FirstOrDefault(gc => gc.Games.Any(g => g.GameID == gameID));
                }
            }

            PagedResult<Player> retVal = new PagedResult<Player>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = players
            };

            return retVal;
        }

        private string GetPlayerPicturesRootFolder()
        {
            return HttpContext.Current.Server.MapPath(String.Format("~{0}", Constants.Images.PlayerPictureRoot));
        }

        private string GetTeamPicturesRootFolder()
        {
            return HttpContext.Current.Server.MapPath(String.Format("~{0}", Constants.Images.TeamPictureRoot));
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
                User currentUser = GetUserByEmail(User.Identity.Name);


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
            User currentUser = GetUserByEmail(User.Identity.Name);
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
                User currentUser = GetUserByEmail(User.Identity.Name);


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
            User currentUser = GetUserByEmail(User.Identity.Name);
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

        private void AssignImage(Competitor competitor, string root, string prefix, string extension)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            string fileSystemRoot = HttpContext.Current.Server.MapPath(String.Format("~{0}", root));
            // temp profile picture
            string sourceFilePath = String.Format("{0}{1}_{2}_{3}.{4}",
                                                                    fileSystemRoot,
                                                                    prefix,
                                                                    currentUser.UserID,
                                                                    0,
                                                                    extension);

            string destFilePath = String.Format("{0}{1}_{2}.{3}",
                                                                    fileSystemRoot,
                                                                    prefix,
                                                                    competitor.CompetitorID,
                                                                    extension);
            
            string destUrl = String.Format("{0}{1}_{2}.{3}",
                                                                    root,
                                                                    prefix,
                                                                    competitor.CompetitorID,
                                                                    extension);

            if (File.Exists(sourceFilePath))
            {
                competitor.PictureUrl = destUrl;
                File.Move(sourceFilePath, destFilePath);
                File.Delete(sourceFilePath);
            }
            else
            {
                competitor.PictureUrl = String.Empty;
            }
            Uow.Competitors.Update(competitor, competitor.CompetitorID);
            Uow.Commit();
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
            AssignImage(player, 
                Constants.Images.PlayerPictureRoot,
                Constants.Images.PlayerPicturePrefix, 
                Constants.Images.PlayerPictureExtension);

            var response = Request.CreateResponse(HttpStatusCode.Created, player);

            // Compose location header that tells how to get this game 

            response.Headers.Location =
                new Uri(Url.Link(RouteConfig.ControllerAndId, new { id = player.CompetitorID }));

            return response;
        }

        [HttpPut]
        [ActionName("updateplayer")]
        public HttpResponseMessage UpdatePlayer(Player player)
        {
            List<GameCompetitor> gameCompetitors = Uow.GameCompetitors
                .GetAll()
                .Where(gc => gc.CompetitorID == player.CompetitorID)
                .ToList();
            foreach (GameCompetitor gc in gameCompetitors)
            {
                Uow.GameCompetitors.Delete(gc);
            }
            foreach (GameCompetitor gc in player.Games.Where(g => g.Selected))
            {
                gc.Competitor = null;
                gc.Game = null;
                Uow.GameCompetitors.Add(gc);
            }
            Uow.Competitors.Update(player, player.CompetitorID);
            Uow.Commit();
            var response = Request.CreateResponse(HttpStatusCode.OK, player);

            return response;
        }

        [HttpGet]
        [ActionName("getupdateplayer")]
        public Player GetUpdatePlayer(long id)
        {
            Player retVal = Uow.Competitors
                .GetAll(c => c.Games)
                .OfType<Player>()
                .Where(p => p.CompetitorID == id)
                .First();
            List<int> gameIds = retVal.Games.Select(g => g.GameID).ToList();
            List<Game> games = Uow.Games
                .GetAll(g => g.Category)
                .Where(g => gameIds.Contains(g.GameID))
                .ToList();
            
            // assume that there should always be at least one game that player comeptes in
            int gameCategory = games[0].GameCategoryID;
            List<Game> allGames = Uow.Games
                .GetAll(g => g.Category)
                .Where(g => g.GameCategoryID == gameCategory)
                .ToList();

            foreach (Game game in allGames)
            {
                GameCompetitor gameCompetitor = retVal.Games.FirstOrDefault(gc => gc.GameID == game.GameID);
                if (gameCompetitor != null)
                {
                    gameCompetitor.Selected = true;
                }
                else
                {
                    retVal.Games.Add(new GameCompetitor()
                    {
                        CompetitorID = retVal.CompetitorID,
                        Competitor = retVal,
                        Game = game,
                        GameID = game.GameID
                    });
                }
            }
            retVal.Games = retVal.Games.OrderBy(g => g.Game.Title).ToList();

             return retVal;
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
        public PagedResult<Team> GetTeams(int page, int count)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<Team> teams = Uow.Competitors
                .GetAll(t => t.Games)
                .OfType<Team>()
                .Where(t => t.Players.Any(p => p.Player.UserID == currentUser.UserID))
                .Distinct()
                .OrderBy(t => t.Name)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            int totalItems = Uow.Competitors
                .GetAll()
                .OfType<Team>()
                .Where(t => t.Players.Any(p => p.Player.UserID == currentUser.UserID))
                .Distinct()
                .Count();

            // load only relevat data to increase eficiency
            foreach (Team team in teams)
            {
                if (team.Games.Count > 0)
                {
                    int gameID = team.Games[0].GameID;

                    team.Games[0].Game = new Game()
                    {
                        GameID = gameID
                    };

                    team.Games[0].Game.Category = Uow.GameCategories
                        .GetAll()
                        .FirstOrDefault(gc => gc.Games.Any(g => g.GameID == gameID));
                }
            }


            PagedResult<Team> retVal = new PagedResult<Team>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = teams
            }; 

            return retVal;
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
            AssignImage(team,
                Constants.Images.TeamPictureRoot,
                Constants.Images.TeamPicturePrefix,
                Constants.Images.TeamPictureExtension);

            var response = Request.CreateResponse(HttpStatusCode.Created, team);

            // Compose location header that tells how to get this game 
            response.Headers.Location =
                new Uri(Url.Link(RouteConfig.ControllerAndId, new { id = team.CompetitorID }));

            return response;
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

        // api/user/searchplayers
        [HttpGet]
        [ActionName("searchteamplayers")]
        public List<Player> SearchPlayers(long teamId, string search)
        {
            if (search == null)
            {
                search = String.Empty;
            }
            User currentUser = GetUserByEmail(User.Identity.Name);
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

        [HttpPut]
        [ActionName("updateteam")]
        public HttpResponseMessage UpdateTeam(Team team)
        {
            List<GameCompetitor> gameCompetitors = Uow.GameCompetitors
                .GetAll()
                .Where(gc => gc.CompetitorID == team.CompetitorID)
                .ToList();
            foreach (GameCompetitor gc in gameCompetitors)
            {
                Uow.GameCompetitors.Delete(gc);
            }
            foreach (GameCompetitor gc in team.Games.Where(g => g.Selected))
            {
                gc.Competitor = null;
                gc.Game = null;
                Uow.GameCompetitors.Add(gc);
            }
            Uow.Competitors.Update(team, team.CompetitorID);
            Uow.Commit();
            var response = Request.CreateResponse(HttpStatusCode.OK, team);

            return response;
        }

        [HttpGet]
        [ActionName("getupdateteam")]
        public Team GetUpdateTeam(long id)
        {
            User currentUser = GetUserByEmail(User.Identity.Name);

            Team retVal = Uow.Competitors
                .GetAll(c => c.Games, c => ((Team)c).Players)
                .OfType<Team>()
                .Where(p => p.CompetitorID == id)
                .First();
            List<int> gameIds = retVal.Games.Select(g => g.GameID).ToList();

            List<Game> games = Uow.Games
                .GetAll(g => g.Category)
                .Where(g => gameIds.Contains(g.GameID))
                .ToList();

            // assume that there should always be at least one game that player comeptes in
            int gameCategory = games[0].GameCategoryID;
            List<Game> allGames = Uow.Games
                .GetAll(g => g.Category)
                .Where(g => g.GameCategoryID == gameCategory)
                .ToList();

            foreach (Game game in allGames)
            {
                GameCompetitor gameCompetitor = retVal.Games.FirstOrDefault(gc => gc.GameID == game.GameID);
                if (gameCompetitor != null)
                {
                    gameCompetitor.Selected = true;
                }
                else
                {
                    retVal.Games.Add(new GameCompetitor()
                    {
                        CompetitorID = retVal.CompetitorID,
                        Competitor = retVal,
                        Game = game,
                        GameID = game.GameID
                    });
                }
            }

            // load players
            foreach (TeamPlayer teamPlayer in retVal.Players)
            {
                teamPlayer.Player = Uow.Competitors
                    .GetAll(p => ((Player)p).User)
                    .OfType<Player>()
                    .FirstOrDefault(p => p.CompetitorID == teamPlayer.PlayerID);
                    
                teamPlayer.Player.IsCurrentUserCompetitor = teamPlayer.Player.UserID == currentUser.UserID;
            }

            retVal.Games = retVal.Games.OrderBy(g => g.Game.Title).ToList();


            return retVal;
        }


        [HttpGet]
        [ActionName("matches")]
        public PagedResult<Match> GetMatches(int page, int count)
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

            int totalItems = Uow.Matches
                                        .GetAll()
                                        .Where(m => m.Scores
                                                        .Any(s => ids.Contains(s.CompetitorID)))
                                        .Count();

            List<Match> matches = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Scores
                                                        .Any(s => ids.Contains(s.CompetitorID)))
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

            foreach (Competitor competitor in competitors)
            {
                if (ids.Contains(competitor.CompetitorID))
                {
                    competitor.IsCurrentUserCompetitor = true;
                }
            }

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
        [AllowAnonymous]
        [ActionName("publicmatches")]
        public PagedResult<Match> GetMatches(string id, int page, int count)
        {
            int userId = Int32.Parse(id);
            List<long> teamsIds = Uow.Competitors
                                        .GetAll()
                                        .OfType<Team>()
                                        .Where(t => t.Players.Any(p => p.Player.User.UserID == userId))
                                        .Select(t => t.CompetitorID)
                                        .Distinct()
                                        .ToList();
            List<long> playerIds = Uow.Competitors
                                        .GetAll()
                                        .OfType<Player>()
                                        .Where(p => p.UserID == userId)
                                        .Select(p => p.CompetitorID)
                                        .ToList();

            List<long> ids = teamsIds.Concat(playerIds).ToList();

            int totalItems = Uow.Matches
                                        .GetAll()
                                        .Where(m => m.Status == MatchStatus.Confirmed &&
                                                    m.Scores
                                                        .Any(s => ids.Contains(s.CompetitorID)))
                                        .Count();

            List<Match> matches = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Status == MatchStatus.Confirmed && 
                                                    m.Scores
                                                        .Any(s => ids.Contains(s.CompetitorID)))
                                        .OrderByDescending(s => s.Date)
                                        .Skip((page - 1) * count)
                                        .Take(count)
                                        .ToList();
            
            foreach (Match match in matches)
            {
                foreach (CompetitorScore cs in match.Scores)
                {
                    cs.Competitor = Uow.Competitors.GetById(cs.CompetitorID);
                }
            }

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
        [AllowAnonymous]
        [ActionName("publicplayers")]
        public PagedResult<Player> GetPlayers(string id, int page, int count)
        {
            int userId = Int32.Parse(id);
            int totalItems = Uow.Competitors
                                        .GetAll(c => ((Player)c).User)
                                        .OfType<Player>()
                                        .Where(c => c.UserID == userId)
                                        .Count();

            List<Player> players = Uow.Competitors
                                        .GetAll(c => ((Player)c).User, c => c.Games)
                                        .OfType<Player>()
                                        .Where(c => c.UserID == userId)
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
        [AllowAnonymous]
        [ActionName("publicteams")]
        public PagedResult<Team> GetTeams(string id, int page, int count)
        {
            int userId = Int32.Parse(id);
            int totalItems = Uow.Competitors
                                        .GetAll()
                                        .OfType<Team>()
                                        .Where(c => c.Players.Any(p => p.Player.UserID == userId))
                                        .Count();

            List<Team> teams = Uow.Competitors
                                        .GetAll(c => c.Games)
                                        .OfType<Team>()
                                        .Where(c => c.Players.Any(p => p.Player.UserID == userId))
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

        // api/user/mycompeatinggames
        [HttpGet]
        [ActionName("mycompeatinggames")]
        public List<GameCategory> MyCompeatingGames()
        {
            User currentUser = GetUserByEmail(User.Identity.Name);
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
            User currentUser = GetUserByEmail(User.Identity.Name);
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
            User currentUser = GetUserByEmail(User.Identity.Name);
            if (!String.IsNullOrEmpty(currentUser.PictureUrl))
            {
                currentUser.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
            }
            else
            {
                currentUser.PictureUrl = currentUser.Gender == Gender.Male ?
                    Util.Constants.Images.DefaultProfileMale :
                    Util.Constants.Images.DefaultProfileFemale;
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
                    Util.Constants.Images.DefaultProfileMale :
                    Util.Constants.Images.DefaultProfileFemale;
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
            User currentUser = GetUserByEmail(User.Identity.Name);

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
                        Util.Constants.Images.DefaultProfileMale :
                        Util.Constants.Images.DefaultProfileFemale;
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
        public PagedResult<User> AutomaticMatchConfirmationUsers(int page, int count, string search)
        {
            if (search == null)
            {
                search = String.Empty;
            }
            
            User currentUser = GetUserByEmail(User.Identity.Name);
            List<User> users = Uow.Users
                .GetAll()
                .Except(
                    Uow.AutomaticMatchConfirmations.GetAll()
                    .Where(ac => ac.ConfirmerID == currentUser.UserID)
                    .Select(ac => ac.Confirmee))
                .Where(u => u.UserID != currentUser.UserID && 
                        (u.FirstName.Contains(search) || u.LastName.Contains(search)))
                .OrderBy(u => u.FirstName)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            int totalItems = Uow.Users
                .GetAll()
                .Except(
                    Uow.AutomaticMatchConfirmations.GetAll()
                    .Where(ac => ac.ConfirmerID == currentUser.UserID)
                    .Select(ac => ac.Confirmee))
                .Where(u => u.UserID != currentUser.UserID &&
                        (u.FirstName.Contains(search) || u.LastName.Contains(search)))
                .Count();

            PagedResult<User> retVal = new PagedResult<User>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = users
            };
                
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

        private string GetProfilePicturesRootFolder()
        {
            return HttpContext.Current.Server.MapPath(String.Format("~{0}", Constants.Images.ProfilePictureRoot));
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
                User currentUser = GetUserByEmail(User.Identity.Name);

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
            User currentUser = GetUserByEmail(User.Identity.Name);
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