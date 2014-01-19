using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
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
    public class MatchController : ApiBaseController
    {
        private IMatchBusiness matchBusiness;

        public MatchController(IPlaygroundUow uow, IMatchBusiness mBusiness)
        {
            this.Uow = uow;
            this.matchBusiness = mBusiness;
        }

        [HttpGet]
        [ActionName("matches")]
        public HttpResponseMessage GetMatches(int page, int count)
        {
            Result<PagedResult<Match>> res =
                matchBusiness.FilterByStatus(page, count, MatchStatus.Confirmed);

            HttpResponseMessage response = res.Sucess ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }
    }
}