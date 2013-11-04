using Playground.Data;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Playgorund.Webexperiment.Controllers
{
    public class CompetitorController : Controller
    {
        protected PlaygroundDbContext db = new PlaygroundDbContext();
        
        //
        // GET: /Competitor/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LatestCompetitors(int competitorsCount)
        {
            List<Competitor> latestCompetitors = db.Competitors
                .OrderByDescending(c => c.CreationDate)
                .Take(competitorsCount)
                .ToList();
            
            
            return PartialView("~/Views/Competitor/_ListCompetitorsPartial.cshtml", latestCompetitors);
        }

    }
}
