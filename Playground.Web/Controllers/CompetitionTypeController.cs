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
    public class CompetitionTypeController : ApiController
    {
        private ICompetitionTypeBusiness competitionTypeBusiness;

        public CompetitionTypeController(ICompetitionTypeBusiness ctBusiness)
        {
            this.competitionTypeBusiness = ctBusiness;
        }

        // GET /api/competitiontype/5
        [HttpGet]
        [ActionName("getcompetitiontype")]
        public HttpResponseMessage GetById(int id)
        {
            Result<CompetitionType> res =
                competitionTypeBusiness.GetById(id);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // GET /api/gealltcompetitiontypes
        [HttpGet]
        [ActionName("gealltcompetitiontypes")]
        public HttpResponseMessage GetAll()
        {
            Result<List<CompetitionType>> res =
                competitionTypeBusiness.GetAllCompetitionTypes();

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // GET /api/competitiontypes?page=1&count=5
        [HttpGet]
        [ActionName("getcompetitiontypes")]
        public HttpResponseMessage GetAll(int page, int count)
        {
            Result<PagedResult<CompetitionType>> res =
                competitionTypeBusiness.GetCompetitionTypes(page, count);

            HttpResponseMessage response = res.Success ?
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // POST /api/competitiontype/addcompetitiontype 
        [HttpPost]
        [ActionName("addcompetitiontype")]
        public HttpResponseMessage AddCompetitionType(CompetitionType competitionType)
        {
            Result<CompetitionType> res = 
                competitionTypeBusiness.AddCompetitionType(competitionType);

            HttpResponseMessage response = res.Success ? 
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // PUT /api/competitiontype/updatecompetitiontype
        [HttpPut]
        [ActionName("updatecompetitiontype")]
        public HttpResponseMessage UpdateCompetitionType(CompetitionType competitionType)
        {
            Result<CompetitionType> res =
                competitionTypeBusiness.UpdateCompetitionType(competitionType);

            HttpResponseMessage response = res.Success ? 
                Request.CreateResponse(HttpStatusCode.Created, res.Data) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);

            return response;
        }

        // DELETE /api/competitiontype/deletecompetitiontype
        [HttpDelete]
        [ActionName("deletecompetitiontype")]
        public HttpResponseMessage Delete(int id)
        {
            bool res = competitionTypeBusiness.DeleteCompetitionType(id);

            HttpResponseMessage response = res ?
                Request.CreateResponse(HttpStatusCode.OK) :
                Request.CreateResponse(HttpStatusCode.InternalServerError, "Error deleting competition type");

            return response;
        }
    }
}