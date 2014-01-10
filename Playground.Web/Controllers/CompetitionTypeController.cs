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
    public class CompetitionTypeController : ApiBaseController
    {
        private ICompetitionTypeBusiness competitionTypeBusiness;

        public CompetitionTypeController(IPlaygroundUow uow, ICompetitionTypeBusiness ctBusiness)
        {
            this.Uow = uow;
            this.competitionTypeBusiness = ctBusiness;
        }

        // GET /api/competitiontype/5
        [HttpGet]
        [ActionName("getcompetitiontype")]
        public CompetitionType GetById(int id)
        {
            CompetitionType retVal = Uow.CompetitionTypes.GetById(id);

            return retVal;
        }

        // GET /api/competitiontype
        [HttpGet]
        [ActionName("getcompetitiontypes")]
        public PagedResult<CompetitionType> GetAll(int page, int count)
        {
            List<CompetitionType> competitionTypes = Uow.CompetitionTypes
                .GetAll()
                .OrderBy(ct => ct.CompetitorType)
                .ThenBy(ct => ct.Name)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            int totalItems = Uow.CompetitionTypes
                .GetAll()
                .Count();

            PagedResult<CompetitionType> retVal = new PagedResult<CompetitionType>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = competitionTypes
            };

            return retVal;
        }

        // POST /api/competitiontype/addcompetitiontype 
        [HttpPost]
        [ActionName("addcompetitiontype")]
        public HttpResponseMessage AddCompetitionType(CompetitionType competitionType)
        {
            ResponseObject<CompetitionType> res = 
                competitionTypeBusiness.CreateCompetitionType(competitionType);

            HttpResponseMessage response;
            if (res.Sucess)
            {
                response = Request.CreateResponse(HttpStatusCode.Created, res.Data);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);
            }

            return response;
        }

        // Update an existing Game
        // POST /api/game/updategame
        [HttpPut]
        [ActionName("updatecompetitiontype")]
        public HttpResponseMessage UpdateCompetitionType(CompetitionType competitionType)
        {
            ResponseObject<CompetitionType> res =
                competitionTypeBusiness.CreateCompetitionType(competitionType);

            HttpResponseMessage response;
            if (res.Sucess)
            {
                response = Request.CreateResponse(HttpStatusCode.Created, res.Data);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, res.Message);
            }

            // var response = Request.CreateResponse(HttpStatusCode.OK, competitionType);

            return response;
        }

        [HttpDelete]
        [ActionName("deletecompetitiontype")]
        public HttpResponseMessage Delete(int id)
        {
            Uow.CompetitionTypes.Delete(id);
            Uow.Commit();
            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }
    }
}
