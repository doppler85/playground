using Microsoft.AspNet.Identity;
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
        private IGameCategoryBusiness gameCategoryBusiness;
        private IGameBusiness gameBusiness;
        private IUserBusiness userBusiness;
        private IAutomaticConfirmationBusiness automaticConfirmationBusiness;

        public UserController(IPlaygroundUow uow, 
            IMatchBusiness mBusiness,
            ICompetitorBusiness cBusiness,
            IGameBusiness gBusiness,
            IGameCategoryBusiness gcBusiness,
            IUserBusiness uBusiness,
            IAutomaticConfirmationBusiness iacBusiness)
        {
            this.Uow = uow;
            this.matchBusiness = mBusiness;
            this.competitorBusiness = cBusiness;
            this.gameCategoryBusiness = gcBusiness;
            this.gameBusiness = gBusiness;
            this.userBusiness = uBusiness;
            this.automaticConfirmationBusiness = iacBusiness;
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
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;

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

            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
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
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;

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
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
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
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            
            Result<Team> res = competitorBusiness.GetUpdateTeam(id, currentUser.UserID);
            
            if (res.Sucess)
            {
                List<Player> players = res.Data.Players.Select(p => p.Player).ToList();
                competitorBusiness.LoadUsers(players);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/user/myteamplayer
        [HttpGet]
        [ActionName("myteamplayer")]
        public HttpResponseMessage MyTeamPlayer(int gameCategoryID)
        {
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            Result<Player> res = competitorBusiness.GetPlayerForGameCategory(currentUser.UserID, gameCategoryID);

            if (res.Sucess)
            {
                competitorBusiness.LoadUsers(new List<Player>() { res.Data });
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/user/searchplayers
        [HttpGet]
        [ActionName("searchplayersbycategory")]
        public HttpResponseMessage SearchPlayers([FromUri]SearchCompetitorArgs args)
        {
            if (args.Search == null)
            {
                args.Search = String.Empty;
            }

            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            Result<PagedResult<Player>> res = competitorBusiness.SearchPlayersForGameCategory(args.Page, args.Count, currentUser.UserID, args.GameCategoryID, args.Ids, args.Search);

            if (res.Sucess)
            {
                competitorBusiness.LoadUsers(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/user/searchplayers
        [HttpGet]
        [ActionName("searchteamplayers")]
        public HttpResponseMessage SearchPlayers([FromUri]AdvancedSearchArgs args)
        {
            if (args.Search == null)
            {
                args.Search = String.Empty;
            }
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            GameCategory gameCategory = gameCategoryBusiness.GetByCompetitorId(args.ID);
            List<long> playerIDs = competitorBusiness.GetPlayerIdsForTeam(args.ID);

            Result<PagedResult<Player>> res = competitorBusiness.SearchPlayersForGameCategory(args.Page, 
                args.Count, 
                currentUser.UserID, 
                gameCategory.GameCategoryID,
                playerIDs,
                args.Search);

            if (res.Sucess)
            {
                competitorBusiness.LoadUsers(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
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
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;

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
        public HttpResponseMessage MyCompeatingGames()
        {
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            Result<List<GameCategory>> res = gameCategoryBusiness.FilterByUser(currentUser.UserID);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/user/mycompeatinggames
        [HttpGet]
        [ActionName("mycompeatitors")]
        public HttpResponseMessage MyCompetitors(int gameCategoryID)
        {
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            Result<List<Competitor>> res = competitorBusiness.FilterByUserAndCategory(currentUser.UserID, gameCategoryID);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/user/searchcompetitors
        [HttpGet]
        [ActionName("searchcompetitors")]
        public HttpResponseMessage SearchCompetitors([FromUri]SearchCompetitorArgs args)
        {
            if (args.Search == null)
            {
                args.Search = String.Empty;
            }
            Result<PagedResult<Competitor>> res = competitorBusiness.SearchAndExcludeByCategoryAndCompetitorType(
                args.Page,
                args.Count,
                args.Ids,
                args.GameCategoryID,
                (CompetitorType)args.CompetitorType,
                args.Search);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }



        [HttpGet]
        [AllowAnonymous]
        [ActionName("getprofile")]
        public HttpResponseMessage GetProfile()
        {
            HttpResponseMessage response;
            if (User.Identity.IsAuthenticated)
            {
                string userID = User.Identity.GetUserId();
                Result<User> res = userBusiness.GetUserByExternalId(userID);

                response = res.Sucess ?
                    Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                    Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK);
            }

            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("getdetails")]
        public HttpResponseMessage GetDetails(int id)
        {
            Result<User> res = userBusiness.GetUserById(id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("getuserstats")]
        public UserStats GetUserStats(int id)
        {
            UserStats retVal = null;
            Result<User> userRes = userBusiness.GetUserById(id);

            if (userRes.Sucess)
            {
                retVal = new UserStats(userRes.Data);

                retVal.TotalGames = userBusiness.TotalGamesCount(id);
                retVal.TotalPlayers = userBusiness.TotalPlayersCount(id);
                retVal.TotalTeams = userBusiness.TotalTeamsCount(id);
                retVal.TotalMatches = userBusiness.TotalMatchesCount(id);
            }

            return retVal;
        }

        [HttpPut]
        [ActionName("updateprofile")]
        public HttpResponseMessage UpdateProfile(User user)
        {
            Result<User> res = userBusiness.UpdateUser(user);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);
            
            return response;
        }

        [HttpGet]
        [ActionName("automaticmatchconfirmations")]
        public HttpResponseMessage AutomaticMatchConfirmations(int page, int count)
        {
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;        
            Result<PagedResult<AutomaticMatchConfirmation>> res = 
                automaticConfirmationBusiness.FilterByUser(page, count, currentUser.UserID);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("automaticmatchconfirmationsusers")]
        public HttpResponseMessage AutomaticMatchConfirmationUsers([FromUri]SearchArgs args)
        {
            if (args.Search == null)
            {
                args.Search = String.Empty;
            }

            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            Result<PagedResult<User>> res =
                userBusiness.SearchAndExcludeByAutomaticConfirmation(args.Page, args.Count, currentUser.UserID, args.Search);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);
                
            return response;
        }

        [HttpPost]
        [ActionName("addautomaticconfirmation")]
        public HttpResponseMessage AddAutomaticConfirmation(User user)
        {
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            Result<AutomaticMatchConfirmation> res = automaticConfirmationBusiness.AddConfirmation(currentUser.UserID, user.UserID);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpDelete]
        [ActionName("deleteautomaticconfirmation")]
        public HttpResponseMessage DeleteAutomaticConfirmation(int confirmeeID)
        {
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            bool res = automaticConfirmationBusiness.DeleteConfirmation(confirmeeID, currentUser.UserID);

            HttpResponseMessage response = res ?
                Request.CreateResponse(HttpStatusCode.OK) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Error deleting automatic confirmation");

            return response;
        }

        [HttpPut]
        [ActionName("confirmscore")]
        public HttpResponseMessage ConfirmScore(CompetitorScore competitorScore)
        {
            Result<bool> res = competitorBusiness.ConfirmScore(competitorScore);

            if (res.Sucess && res.Data)
            {
                int totalMatches = matchBusiness.TotalMatchesByStatus(MatchStatus.Confirmed);
                Match match = matchBusiness.GetMatchById(competitorScore.MatchID).Data;
                if (match != null)
                {
                    matchBusiness.LoadScores(match);
                    match.Game = gameBusiness.GetById(match.GameID).Data;
                }
                LiveScores.Instance.BroadcastTotalMatches(match, totalMatches);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Error confirming score");

            return response;
        }

        [HttpPost]
        [ActionName("addmatch")]
        public HttpResponseMessage AddMath(Match match)
        {
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            Result<Match> res = matchBusiness.AddMatch(currentUser.UserID, match);

            if (res.Sucess && res.Data.Status == MatchStatus.Confirmed)
            {
                int totalMatches = matchBusiness.TotalMatchesByStatus(MatchStatus.Confirmed);
                Match m = matchBusiness.GetMatchById(match.MatchID).Data;
                if (m != null)
                {
                    matchBusiness.LoadScores(m);
                }
                LiveScores.Instance.BroadcastTotalMatches(m, totalMatches);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

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
                User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;


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
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
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
                User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;


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
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
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
                User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;

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
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
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