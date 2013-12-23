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
        public GameController(IPlaygroundUow uow)
        {
            this.Uow = uow;
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
            // retVal.CompetitionTypes = Uow.GameCompetitionTypes.GetByGameId(retVal.GameID).ToList();
            retVal.CompetitionTypes = GetByGameId(retVal.GameID);
            retVal.GamePictureUrl = String.Format("{0}{1}_{2}.{3}?nocache={3}",
                Constants.Images.GamePictureRoot,
                Constants.Images.GamePicturePrefix,
                retVal.GameID,
                Constants.Images.GamePictureExtension,
                DateTime.Now.Ticks);

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
        // POST /api/Game
        [HttpPost]
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

        // Create a new Game
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
            foreach (GameCompetitionType ct in game.CompetitionTypes)
            {
                Uow.GameCompetitionTypes.Add(ct);
            }

            Uow.Games.Update(game, game.GameID);

            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.OK, game);

            return response;
        }

        [HttpDelete]
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
