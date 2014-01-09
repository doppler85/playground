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
        public CompetitionTypeController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        // GET /api/competitiontype/5
        [HttpGet]
        [ActionName("getcompetitiontype")]
        public CompetitionType GetAll(int id)
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

        // Create a new Game
        // POST /api/competitiontype
        [HttpPost]
        [ActionName("addcompetitiontype")]
        public HttpResponseMessage AddCompetitionType(CompetitionType competitionType)
        {
            Uow.CompetitionTypes.Add(competitionType);
            Uow.Commit();

            var response = Request.CreateResponse(HttpStatusCode.Created, competitionType);

            // Compose location header that tells how to get this game 
            // e.g. ~/api/game/5

            response.Headers.Location =
                new Uri(Url.Link(RouteConfig.ControllerAndId, new { id = competitionType.CompetitionTypeID }));

            return response;
        }

        // Update an existing Game
        // POST /api/game/updategame
        [HttpPut]
        [ActionName("updatecompetitiontype")]
        public HttpResponseMessage UpdateCompetitionType(CompetitionType competitionType)
        {
            Uow.CompetitionTypes.Update(competitionType, competitionType.CompetitionTypeID);
            Uow.Commit();
            var response = Request.CreateResponse(HttpStatusCode.OK, competitionType);

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
