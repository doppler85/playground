using Playgorund.Webexperiment.Filters;
using Playground.Data;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Playgorund.Webexperiment.Controllers
{
    [InitializeSimpleMembership]
    public class DashboardController : Controller
    {
        private PlaygroundDbContext db = new PlaygroundDbContext();

        //
        // GET: /Dashboard/
        [Authorize(Roles="Admin")]
        public ActionResult Index()
        {
            ViewBag.AllUsers = db.Users.ToList();
            ViewBag.AllPlayers = db.Competitors.OfType<Player>().ToList();
            ViewBag.AllTeams = db.Competitors.OfType<Team>().ToList();
            ViewBag.AllMatches = db.Matches.ToList();
 
            return View();
        }

    }
}
