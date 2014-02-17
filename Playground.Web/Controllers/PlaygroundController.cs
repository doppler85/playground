using Microsoft.AspNet.Identity;
using Playground.Business.Contracts;
using Playground.Model;
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

        public PlaygroundController(IPlaygroundBusiness pBusiness,
            IUserBusiness uBusiness)
        {
            this.playgroundBusiness = pBusiness;
            this.userBusiness = uBusiness;
        }

        [HttpGet]
        [ActionName("getplaygrounds")]
        public HttpResponseMessage GetPlaygrounds(int page, int count)
        {
            Result<PagedResult<Playground.Model.Playground>> res =
                playgroundBusiness.GetPlaygrounds(page, count, false);

            HttpResponseMessage response = res.Sucess ?
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

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }
    }
}