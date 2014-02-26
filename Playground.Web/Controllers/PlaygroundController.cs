using Microsoft.AspNet.Identity;
using Playground.Business.Contracts;
using Playground.Model;
using Playground.Model.Util;
using Playground.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Playground.Web.Controllers
{
    public class PlaygroundController : ApiBaseController
    {
        private IPlaygroundBusiness playgroundBusiness;
        private IUserBusiness userBusiness;
        private IGameBusiness gameBusiness;

        public PlaygroundController(IPlaygroundBusiness pBusiness,
            IUserBusiness uBusiness,
            IGameBusiness gBusiness)
        {
            this.playgroundBusiness = pBusiness;
            this.userBusiness = uBusiness;
            this.gameBusiness = gBusiness;
        }

        [HttpGet]
        [ActionName("getplaygrounds")]
        public HttpResponseMessage GetPlaygrounds(int page, int count)
        {
            Result<PagedResult<Playground.Model.Playground>> res =
                playgroundBusiness.GetPlaygrounds(page, count, false);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpPost]
        [ActionName("addplayground")]
        public HttpResponseMessage AddPlayground(Playground.Model.Playground playground)
        {
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            playground.OwnerID = currentUser.UserID;

            Result<Playground.Model.Playground> res = playgroundBusiness.AddPlayground(playground);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpDelete]
        [ActionName("removeplayground")]
        public HttpResponseMessage RemovePlayground(int id)
        {
            bool success = playgroundBusiness.RemovePlayground(id);

            HttpResponseMessage response = success ?
                Request.CreateResponse(HttpStatusCode.Created, success) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Error deleting playgorund");

            return response;
        }

        [HttpGet]
        [ActionName("availablegames")]
        public HttpResponseMessage GetAvailableGames([FromUri]SearchPlaygroundArgs args)
        {
            if (args.Search == null)
            {
                args.Search = String.Empty;
            }

            Result<PagedResult<Game>> res = gameBusiness.SearchAvailableByPlayground(args.Page, args.Count, args.PlaygroundID, args.Search);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpPost]
        [ActionName("addgame")]
        public HttpResponseMessage AddGameToPlayground(PlaygroundGame playgroundGame)
        {
            bool success = playgroundBusiness.AddGameToPlayGround(playgroundGame);

            HttpResponseMessage response = success ?
                Request.CreateResponse(HttpStatusCode.Created, success) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Eror adding game to playground");

            return response;
        }

        [HttpDelete]
        [ActionName("removegame")]
        public HttpResponseMessage RemoveeGameFromPlayground([FromUri]PlaygroundGame playgroundGame)
        {
            bool success = playgroundBusiness.RemoveGameFromPlayground(playgroundGame);

            HttpResponseMessage response = success ?
                Request.CreateResponse(HttpStatusCode.Created, success) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Eror removing game from playground");

            return response;
        }

        [HttpGet]
        [ActionName("availableusers")]
        public HttpResponseMessage GetAvailableUsers([FromUri]SearchPlaygroundArgs args)
        {
            if (args.Search == null)
            {
                args.Search = String.Empty;
            }

            Result<PagedResult<User>> res = userBusiness.SearchAvailableByPlayground(args.Page, args.Count, args.PlaygroundID, args.Search);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpPost]
        [ActionName("adduser")]
        public HttpResponseMessage AddUserToPlayground(PlaygroundUser playgroundUser)
        {
            bool success = playgroundBusiness.AddUserToPlaygroound(playgroundUser);

            HttpResponseMessage response = success ?
                Request.CreateResponse(HttpStatusCode.Created, success) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Eror adding user to playground");

            return response;
        }

        [HttpDelete]
        [ActionName("removeuser")]
        public HttpResponseMessage RemoveeGameFromPlayground([FromUri]PlaygroundUser playgroundUser)
        {
            bool success = playgroundBusiness.RemoveUserFromPlayground(playgroundUser);

            HttpResponseMessage response = success ?
                Request.CreateResponse(HttpStatusCode.Created, success) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Eror removing user from playground");

            return response;
        }

        [HttpGet]
        [ActionName("getupdateplayground")]
        public HttpResponseMessage GetUpdatePlayground(int playgroundID)
        {
            Result<Playground.Model.Playground> res = playgroundBusiness.GetById(playgroundID);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpPut]
        [ActionName("updateplayground")]
        public HttpResponseMessage UpdatePlayground(Playground.Model.Playground playground)
        {
            Result<Playground.Model.Playground> res = playgroundBusiness.UpdatePlayground(playground);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("getplaygroundsinarea")]
        public HttpResponseMessage GetPlaygroundsInArea([FromUri]SearchAreaArgs args)
        {
            Result<List<Playground.Model.Playground>> res = playgroundBusiness.SearchInArea(
                new Location()
                {
                   Latitude = args.StartLocationLatitude,
                   Longitude = args.StartLocationLongitude
                }, 
                new Location()
                {
                    Latitude = args.EndLocationLatitude,
                    Longitude = args.EndLocationLongitude
                },
                args.Count);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        [HttpGet]
        [ActionName("searchplaygrounds")]
        public HttpResponseMessage SearchPlaygrounds([FromUri]SearchAreaArgs args)
        {
            User currentUser = userBusiness.GetUserByExternalId(User.Identity.GetUserId()).Data;
            int userId = currentUser == null ? 0 : currentUser.UserID;

            Result<PagedResult<Playground.Model.ViewModel.Playground>> res = playgroundBusiness.SearchPlaygrounds(args, userId);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }
    }
}