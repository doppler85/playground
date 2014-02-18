using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Playground.Web.Controllers
{
    public class HomeController : ApiBaseController
    {
        private IMatchBusiness matchBusiness;
        private ICompetitorBusiness competitorBusiness;

        public HomeController(IMatchBusiness mBusiness, 
            ICompetitorBusiness cBusiness)
        {
            this.matchBusiness = mBusiness;
            this.competitorBusiness = cBusiness;
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

        [HttpGet]
        [ActionName("competitors")]
        public HttpResponseMessage GetCompetitors(int page, int count)
        {
            Result<PagedResult<Competitor>> res = competitorBusiness.GetCompetitors(page, count);
            if (res.Success)
            {
                competitorBusiness.LoadCategories(res.Data.Items);
            }

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.OK, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }
    }
}
