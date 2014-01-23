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
        private IMatchBusiness matchBusiness;

        public GameCategoryController(IPlaygroundUow uow, 
            IGameCategoryBusiness gcBusiness,
            IGameBusiness gBusiness,
            IMatchBusiness mBusiness)
        {
            this.Uow = uow;
            this.gameCategoryBusiness = gcBusiness;
            this.gameBusiness = gBusiness;
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
            Result<GameCategory> res =
                gameCategoryBusiness.DeleteGameCategory(id);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

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
        public PagedResult<Player> GetPlayers(string id, int page, int count)
        {
            int gameCategoryId = Int32.Parse(id);
            int totalItems = Uow.Competitors
                                        .GetAll(c => ((Player)c).User)
                                        .OfType<Player>()
                                        .Where(c => c.Games.Any(g => g.Game.GameCategoryID == gameCategoryId))
                                        .Count();

            List<Player> players = Uow.Competitors
                                        .GetAll(c => ((Player)c).User, c => c.Games)
                                        .OfType<Player>()
                                        .Where(c => c.Games.Any(g => g.Game.GameCategoryID == gameCategoryId))
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
            int gameCategoryId = Int32.Parse(id);
            int totalItems = Uow.Competitors
                                        .GetAll()
                                        .OfType<Team>()
                                        .Where(c => c.Games.Any(g => g.Game.GameCategoryID == gameCategoryId))
                                        .Count();

            List<Team> teams = Uow.Competitors
                                        .GetAll(c => c.Games)
                                        .OfType<Team>()
                                        .Where(c => c.Games.Any(g => g.Game.GameCategoryID == gameCategoryId))
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