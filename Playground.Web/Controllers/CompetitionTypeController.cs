using Playground.Data.Contracts;
using Playground.Model;
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

        // GET /api/competitiontype
        [HttpGet]
        public List<CompetitionType> GetAll()
        {
            return Uow.CompetitionTypes.GetAll().OrderBy(ct => ct.CompetitorType).ThenBy(ct => ct.Name).ToList();
        }

        // Create a new Game
        // POST /api/competitiontype
        [HttpPost]
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

    }
}
