using Playground.Business.Contracts;
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

        private List<GameCompetitionType> GetByGameId(int gameID)
        {
            return Uow.GameCompetitionTypes
                                        .GetAll(ct => ct.CompetitionType)
                                        .Where(ct => ct.GameID == gameID)
                                        .ToList();
        }

        [HttpGet]
        [ActionName("details")]
        public Game GameDetails(int id)
        {
            Game retVal = Uow.Games.GetById(id);
            retVal.Category = Uow.GameCategories.GetById(retVal.GameCategoryID);
            if (!String.IsNullOrEmpty(retVal.PictureUrl))
            {
                retVal.PictureUrl += String.Format("?nocache={0}", DateTime.Now.Ticks);
            }
            else if (!String.IsNullOrEmpty(retVal.Category.PictureUrl))
            {
                retVal.PictureUrl = String.Format("{0}?nocache={1}", retVal.Category.PictureUrl, DateTime.Now.Ticks);
            }
            return retVal;
        }

        [HttpGet]
        [ActionName("getgamestats")]
        public GameStats GetStats(int id)
        {
            Game game = GameDetails(id);
            GameStats retVal = new GameStats(game);
            retVal.Category = Uow.GameCategories.GetById(retVal.GameCategoryID);
            retVal.TotalCompetitors = Uow.Competitors
                .GetAll()
                .Where(c => c.Games.Any(g => g.GameID == retVal.GameID))
                .Count();
            retVal.TotalMatches = Uow.Matches
                .GetAll()
                .Where(m => m.Scores.Any(s => s.Match.GameID == retVal.GameID))
                .Count();

            return retVal;
        }

        [HttpGet]
        [ActionName("getupdategame")]
        public Game GetUpdateGame(int id)
        {
            Game retVal = GameDetails(id);
            retVal.CompetitionTypes = competitionTypeBusiness.GetGameCompetitionTypes(id).Data;

            return retVal;
        }

        [HttpGet]
        [ActionName("availablecomptypes")]
        public List<GameCompetitionType> AvailableCompetitionTypes(int id)
        {
            List<GameCompetitionType> retVal = new List<GameCompetitionType>();
            IQueryable<CompetitionType> availableCompetitionTypes = Uow.CompetitionTypes
                .GetAll()
                .Where(ct => !ct.Games.Any(g => g.GameID == id))
                .Distinct();
            foreach (CompetitionType ct in availableCompetitionTypes)
            {
                retVal.Add(new GameCompetitionType()
                {
                    CompetitionType = ct,
                    CompetitionTypeID = ct.CompetitionTypeID,
                    GameID = id
                });
            }
            
            return retVal;
        }


        // Create a new Game
        // POST /api/game/addgame
        [HttpPost]
        [ActionName("addgame")]
        public HttpResponseMessage AddGame(Game game)
        {
            Uow.Games.Add(game);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.Created, game);

            // Compose location header that tells how to get this game 
            // e.g. ~/api/game/5

            response.Headers.Location =
                new Uri(Url.Link(RouteConfig.ControllerAndId, new { id = game.GameID }));

            return response;
        }

        // Update an existing Game
        // POST /api/game/updategame
        [HttpPut]
        [ActionName("updategame")]
        public HttpResponseMessage UpdateGame(Game game)
        {
            // clear prvious competition types
            List<GameCompetitionType> currentCompetitionTypes = GetByGameId(game.GameID); 
            foreach (GameCompetitionType ct in currentCompetitionTypes)
            {
                Uow.GameCompetitionTypes.Delete(ct);
            }
            foreach (GameCompetitionType ct in game.CompetitionTypes.Where(ct => ct.Selected))
            {
                ct.Game = null;
                ct.CompetitionType = null;
                Uow.GameCompetitionTypes.Add(ct);
            }
            Uow.Games.Update(game, game.GameID);
            Uow.Commit();
            var response = Request.CreateResponse(HttpStatusCode.OK, game);

            return response;
        }

        [HttpDelete]
        [ActionName("deletegame")]
        public HttpResponseMessage Delete(int id)
        {
            Uow.Games.Delete(id);
            Uow.Commit();
            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private string GetGamePicturesRootFolder()
        {
            return HttpContext.Current.Server.MapPath(String.Format("~{0}", Constants.Images.GamePictureRoot));
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

        [HttpGet]
        [ActionName("players")]
        public HttpResponseMessage GetPlayers(string id, int page, int count)
        {
            int gameID = Int32.Parse(id);

            Result<PagedResult<Player>> res =
                competitorBusiness.GetPlayersForGame(page, count, gameID);

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

        [HttpGet]
        [ActionName("teams")]
        public HttpResponseMessage GetTeams(string id, int page, int count)
        {
            int gameID = Int32.Parse(id);

            Result<PagedResult<Team>> res =
                competitorBusiness.GetTeamsForGame(page, count, gameID);
            
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
        public HttpResponseMessage GetIndividualGames(string id)
        {
            int cateogryID = Int32.Parse(id);

            Result<List<Game>> res =
                gameBusiness.FilterByCategoryAndCompetitionType(cateogryID, CompetitorType.Individual);

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
        public HttpResponseMessage GetTeamGames(string id)
        {
            int cateogryID = Int32.Parse(id);

            Result<List<Game>> res =
                gameBusiness.FilterByCategoryAndCompetitionType(cateogryID, CompetitorType.Team);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }
    }
}