using Playground.Business.Contracts;
using Playground.Common;
using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Models;
using Playground.Web.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Playground.Web.Controllers
{  
    public class GameController : ApiBaseController
    {
        private ICompetitorBusiness competitorBusiness;
        private ICompetitionTypeBusiness competitionTypeBusiness;
        private IMatchBusiness matchBusiness;
        private IGameCategoryBusiness gameCategoryBusiness;
        private IGameBusiness gameBusiness;

        public GameController(IPlaygroundUow uow, 
            IMatchBusiness mBusiness,
            ICompetitionTypeBusiness ctBusiness,
            IGameCategoryBusiness gcBusiness,
            IGameBusiness gBusiness,
            ICompetitorBusiness cBusiness)
        {
            this.Uow = uow;
            this.matchBusiness = mBusiness;
            this.competitionTypeBusiness = ctBusiness;
            this.gameCategoryBusiness = gcBusiness;
            this.gameBusiness = gBusiness;
            this.competitorBusiness = cBusiness;
        }

        private string GetGamePicturesRootFolder()
        {
            return HttpContext.Current.Server.MapPath(String.Format("~{0}", Constants.Images.GamePictureRoot));
        }

        [HttpGet]
        [ActionName("getgamestats")]
        public HttpResponseMessage GetStats(int id)
        {
            Result<Game> gameRes = gameBusiness.GetById(id);
            GameStats stats = null;
            if (gameRes.Sucess)
            {
                gameRes.Data.Category = gameCategoryBusiness.GetById(gameRes.Data.GameCategoryID).Data;
                stats = new GameStats(gameRes.Data);
                stats.TotalCompetitors = gameBusiness.TotalCompetitorsCount(stats.GameID);
                stats.TotalMatches = gameBusiness.TotalMatchesCount(stats.GameID);
            }

            HttpResponseMessage response = gameRes.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, stats) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, gameRes.Message);

            return response;
        }

        [HttpGet]
        [ActionName("getupdategame")]
        public HttpResponseMessage GetUpdateGame(int id)
        {
            Result<Game> res = gameBusiness.GetById(id);
            if (res.Sucess)
            {
                res.Data.CompetitionTypes = competitionTypeBusiness.FilterByGame(id).Data;
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("availablecomptypes")]
        public HttpResponseMessage AvailableCompetitionTypes(int id)
        {
            Result<List<GameCompetitionType>> res = competitionTypeBusiness.FilterByGameAvailable(id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }


        // Create a new Game
        // POST /api/game/addgame
        [HttpPost]
        [ActionName("addgame")]
        public HttpResponseMessage AddGame(Game game)
        {
            Result<Game> res = gameBusiness.AddGame(game);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // Update an existing Game
        // POST /api/game/updategame
        [HttpPut]
        [ActionName("updategame")]
        public HttpResponseMessage UpdateGame(Game game)
        {
            Result<Game> res = gameBusiness.UpdateGame(game);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpDelete]
        [ActionName("deletegame")]
        public HttpResponseMessage Delete(int id)
        {
            bool res = gameBusiness.DeleteGame(id);

            HttpResponseMessage response = res ?
                Request.CreateResponse(HttpStatusCode.OK) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Error deleting game");

            return response;
        }

        // api/game/matches
        [HttpGet]
        [ActionName("matches")]
        public HttpResponseMessage GetMatches(string id, int page, int count)
        {
            int gameID = Int32.Parse(id);

            Result<PagedResult<Match>> res =
                matchBusiness.FilterByStatusAndGame(page, count, MatchStatus.Confirmed, gameID);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/game/players
        [HttpGet]
        [ActionName("players")]
        public HttpResponseMessage GetPlayers(int id, int page, int count)
        {
            Result<PagedResult<Player>> res =
                competitorBusiness.GetPlayersForGame(page, count, id);

            if (res.Sucess)
            {
                competitorBusiness.LoadUsers(res.Data.Items);
                competitorBusiness.LoadCategories(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);
            
            return response;
        }

        // api/game/teams
        [HttpGet]
        [ActionName("teams")]
        public HttpResponseMessage GetTeams(int id, int page, int count)
        {
            Result<PagedResult<Team>> res =
                competitorBusiness.GetTeamsForGame(page, count, id);
            
            if (res.Sucess)
            {
                competitorBusiness.LoadCategories(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/game/individualcategories
        [HttpGet]
        [ActionName("individualcategories")]
        public HttpResponseMessage GetIndividualCategories()
        {
            Result<List<GameCategory>> res =
                gameCategoryBusiness.GetGameCategoriesByCompetitorType(CompetitorType.Individual);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/game/individualgames
        [HttpGet]
        [ActionName("individualgames")]
        public HttpResponseMessage GetIndividualGames(int id)
        {
            Result<List<Game>> res =
                gameBusiness.FilterByCategoryAndCompetitionType(id, CompetitorType.Individual);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/game/teamcategories
        [HttpGet]
        [ActionName("teamcategories")]
        public HttpResponseMessage GetTeamCategories()
        {
            Result<List<GameCategory>> res =
                gameCategoryBusiness.GetGameCategoriesByCompetitorType(CompetitorType.Team);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/game/teamgames
        [HttpGet]
        [ActionName("teamgames")]
        public HttpResponseMessage GetTeamGames(int id)
        {
            Result<List<Game>> res =
                gameBusiness.FilterByCategoryAndCompetitionType(id, CompetitorType.Team);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }


        [HttpPost]
        [ActionName("uploadgamepicture")]
        public async Task<HttpResponseMessage> UploadGamePicture()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = GetGamePicturesRootFolder();
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
                int gameId = Int32.Parse(provider.FormData["ID"]);
                string retUrl = "";
                if (provider.FileData.Count > 0)
                {
                    string localName = provider.FileData[0].LocalFileName;
                    FileInfo fileInfo = new FileInfo(localName);
                    string filePath = String.Format("{0}{1}_{2}_temp.{3}",
                                            root,
                                            Constants.Images.GamePicturePrefix,
                                            gameId,
                                            Constants.Images.GamePictureExtension);

                    ImageUtil.ScaleImage(fileInfo.FullName,
                                        filePath,
                                        Constants.Images.GameImageFormat,
                                        Constants.Images.GameImageMaxSize,
                                        Constants.Images.GameImageMaxSize);

                    fileInfo.Delete();

                    fileInfo = new FileInfo(filePath);
                    retUrl = String.Format("{0}{1}", Constants.Images.GamePictureRoot, fileInfo.Name);
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
        [ActionName("cropgamepicture")]
        public HttpResponseMessage CropGamePicture(CropingArgs coords)
        {
            //User currentUser = GetUserByEmail(User.Identity.Name);
            string root = GetGamePicturesRootFolder();
            string filePath = String.Format("{0}{1}_{2}_temp.{3}",
                                                        root,
                                                        Constants.Images.GamePicturePrefix,
                                                        coords.ID,
                                                        Constants.Images.GamePictureExtension);

            string destFilePath = String.Format("{0}{1}_{2}.{3}",
                                                                    root,
                                                                    Constants.Images.GamePicturePrefix,
                                                                    coords.ID,
                                                                    Constants.Images.GamePictureExtension);
            try
            {
                ImageUtil.CropImage(filePath,
                                    destFilePath,
                                    Constants.Images.GameImageFormat,
                                    coords);

                // delete temporary file
                FileInfo fileInfo = new FileInfo(filePath);
                fileInfo.Delete();

                fileInfo = new FileInfo(destFilePath);
                string retUrl = String.Format("{0}{1}", Constants.Images.GamePictureRoot, fileInfo.Name);

                // if all ok update game with image
                Game game = Uow.Games.GetById(coords.ID);
                game.PictureUrl = retUrl;
                Uow.Games.Update(game, game.GameID);
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
}