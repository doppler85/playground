using Playground.Business.Contracts;
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

        public PlaygroundController(IPlaygroundBusiness pBusiness)
        {
            this.playgroundBusiness = pBusiness;
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
    }
}