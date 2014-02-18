using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Hubs;
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
        private IUserBusiness userBusiness;

        public MatchController(
            IMatchBusiness mBusiness,
            IUserBusiness uBusiness)
        {
            this.matchBusiness = mBusiness;
            this.userBusiness = uBusiness;
        }

        [HttpGet]
        [ActionName("matches")]
        public HttpResponseMessage GetMatches(int page, int count)
        {
            Result<PagedResult<Match>> res =
                matchBusiness.FilterByStatus(page, count, MatchStatus.Confirmed);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }
    }
}