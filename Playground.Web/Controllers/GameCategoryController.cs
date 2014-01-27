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
    public class GameCategoryController : ApiBaseController
    {
        private IGameCategoryBusiness gameCategoryBusiness;
        private IGameBusiness gameBusiness;
        private ICompetitorBusiness competitorBusiness;
        private IMatchBusiness matchBusiness;

        public GameCategoryController(IPlaygroundUow uow, 
            IGameCategoryBusiness gcBusiness,
            IGameBusiness gBusiness,
            ICompetitorBusiness cBusiness,
            IMatchBusiness mBusiness)
        {
            this.Uow = uow;
            this.gameCategoryBusiness = gcBusiness;
            this.gameBusiness = gBusiness;
            this.competitorBusiness = cBusiness;
            this.matchBusiness = mBusiness;
        }

        private string GetGameCategpryPicturesRootFolder()
        {
            return HttpContext.Current.Server.MapPath(String.Format("~{0}", Constants.Images.GameCategoryPictureRoot));
        }

        // api/gamecategory/allcategories
        [HttpGet]
        [ActionName("allcategories")]
        public HttpResponseMessage GetCategories(int page, int count)
        {
            Result<PagedResult<GameCategory>> res =
                gameCategoryBusiness.GetGameCategories(page, count);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // api/gamecategory/getcategory/5
        [HttpGet]
        [ActionName("getcategory")]
        public HttpResponseMessage GetCategory(int id)
        {
            Result<GameCategory> res =
                gameCategoryBusiness.GetById(id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // GET: api/gamecategory/getcategorystats/5
        [HttpGet]
        [ActionName("getcategorystats")]
        public GameCategoryStats GetStats(int id)
        {
            Result<GameCategory> res =
                gameCategoryBusiness.GetById(id);

            GameCategoryStats retVal = new GameCategoryStats(res.Data);

            retVal.TotalGames = gameCategoryBusiness.TotalGamesCount(id);
            retVal.TotalCompetitors = gameCategoryBusiness.TotalCompetitorsCount(id);
            retVal.TotalMatches = gameCategoryBusiness.TotalMatchesCount(id);

            return retVal;
        }

        // POST /api/gamecategory/addgamecategory
        [HttpPost]
        [ActionName("addgamecategory")]
        public HttpResponseMessage AddGameCategory(GameCategory gameCategory)
        {
            Result<GameCategory> res =
                gameCategoryBusiness.AddGameCategory(gameCategory);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // POST /api/gamecategory/updategamecategory
        [HttpPut]
        [ActionName("updategamecategory")]
        public HttpResponseMessage UpdateGameCategory(GameCategory gameCategory)
        {
            Result<GameCategory> res =
                gameCategoryBusiness.UpdateGameCategory(gameCategory);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }


        // DELETE /api/gamecategory
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            bool res =
                gameCategoryBusiness.DeleteGameCategory(id);

            HttpResponseMessage response = res ?
                Request.CreateResponse(HttpStatusCode.OK) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Error deleting game category");

            return response;
        }


        // GET /api/gamecategory/matches
        [HttpGet]
        [ActionName("matches")]
        public HttpResponseMessage GetMatches(int id, int page, int count)
        {
            Result<PagedResult<Match>> res =
                matchBusiness.FilterByStatusAndGameCategory(page, count, MatchStatus.Confirmed, id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // GET /api/gamecategory/games
        [HttpGet]
        [ActionName("games")]
        public HttpResponseMessage GetGames(int id, int page, int count)
        {
            Result<PagedResult<Game>> res =
                gameBusiness.FilterByCategory(page, count, id);

            if (res.Sucess)
            {
                gameBusiness.LoadImages(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("players")]
        public HttpResponseMessage GetPlayers(int id, int page, int count)
        {
            Result<PagedResult<Player>> res =
                competitorBusiness.FilterPlayersByGameCategory(page, count, id);

            if (res.Sucess)
            {
                competitorBusiness.LoadUsers(res.Data.Items);
                competitorBusiness.LoadCategories(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;            
        }

        [HttpGet]
        [ActionName("teams")]
        public HttpResponseMessage GetTeams(int id, int page, int count)
        {
            Result<PagedResult<Team>> res =
                competitorBusiness.FilterTeamsByGameCategory(page, count, id);

            if (res.Sucess)
            {
                competitorBusiness.LoadCategories(res.Data.Items);
            }

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpPost]
        [ActionName("uploadgamecategorypicture")]
        public async Task<HttpResponseMessage> UploadGamePicture()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = GetGameCategpryPicturesRootFolder();
            var provider = new UniqueMultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names for uploaded files.
                // User currentUser = GetUserByEmail(User.Identity.Name);

                if (String.IsNullOrEmpty(provider.FormData["ID"]))
                {
                    throw new Exception("No ID for the game category specified");
                }
                int gameCategoryId = Int32.Parse(provider.FormData["ID"]);
                string retUrl = "";
                if (provider.FileData.Count > 0)
                {
                    string localName = provider.FileData[0].LocalFileName;
                    FileInfo fileInfo = new FileInfo(localName);
                    string filePath = String.Format("{0}{1}_{2}_temp.{3}",
                                            root,
                                            Constants.Images.GameCategoryPicturePrefix,
                                            gameCategoryId,
                                            Constants.Images.GameCategoryPictureExtension);

                    ImageUtil.ScaleImage(fileInfo.FullName,
                                        filePath,
                                        Constants.Images.GameCategoryImageFormat,
                                        Constants.Images.GameCategoryImageMaxSize,
                                        Constants.Images.GameCategoryImageMaxSize);

                    fileInfo.Delete();

                    fileInfo = new FileInfo(filePath);
                    retUrl = String.Format("{0}{1}", Constants.Images.GameCategoryPictureRoot, fileInfo.Name);
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
        [ActionName("cropgamecategorypicture")]
        public HttpResponseMessage CropGamePicture(CropingArgs coords)
        {
            //User currentUser = GetUserByEmail(User.Identity.Name);
            string root = GetGameCategpryPicturesRootFolder();
            string filePath = String.Format("{0}{1}_{2}_temp.{3}",
                                                        root,
                                                        Constants.Images.GameCategoryPicturePrefix,
                                                        coords.ID,
                                                        Constants.Images.GameCategoryPictureExtension);

            string destFilePath = String.Format("{0}{1}_{2}.{3}",
                                                                    root,
                                                                    Constants.Images.GameCategoryPicturePrefix,
                                                                    coords.ID,
                                                                    Constants.Images.GameCategoryPictureExtension);
            try
            {
                ImageUtil.CropImage(filePath,
                                    destFilePath,
                                    Constants.Images.GameCategoryImageFormat,
                                    coords);

                // delete temporary file
                FileInfo fileInfo = new FileInfo(filePath);
                fileInfo.Delete();

                fileInfo = new FileInfo(destFilePath);
                string retUrl = String.Format("{0}{1}", Constants.Images.GameCategoryPictureRoot, fileInfo.Name);

                // if all ok update category image 
                GameCategory category = Uow.GameCategories.GetById(coords.ID);
                category.PictureUrl = retUrl;
                Uow.GameCategories.Update(category, category.GameCategoryID);
                Uow.Commit();

                return new HttpResponseMessage()
                {
                    Content = new StringContent(String.Format("{0}?nocache={1}", retUrl, DateTime.Now.Ticks))
                };
            }
            catch (System.Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}