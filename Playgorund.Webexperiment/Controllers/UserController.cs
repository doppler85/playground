using Playgorund.Webexperiment.Filters;
using Playground.Data;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Playgorund.Webexperiment.Util;

namespace Playgorund.Webexperiment.Controllers
{
    [InitializeSimpleMembership]
    public class UserController : Controller
    {
        private PlaygroundDbContext db = new PlaygroundDbContext();

        //
        // GET: /User/

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        //
        // GET: /UserProfile/

        [Authorize]
        public ActionResult UserProfile()
        {
            //db.Configuration.LazyLoadingEnabled = true;
            // System.Web.Security.u
            User user = db.Users.FirstOrDefault(u => u.ExternalUserID == WebSecurity.CurrentUserId);
            if (user == null)
            {
                user = new User()
                {
                    ExternalUserID = WebSecurity.CurrentUserId,
                    Gender = Gender.Male
                };
            }

            List<Player> players = db.Competitors.OfType<Player>().Where(p => p.UserID == user.UserID).ToList();
            List<Team> teams = new List<Team>();
            foreach (Player player in players)
            {
                foreach (TeamPlayer tp in player.Teams)
                {
                    teams.Add(db.Competitors.OfType<Team>().First(t => t.CompetitorID == tp.TeamID));
                }
            }
            var userTeams = db.Competitors.OfType<Team>().Where(t => t.CreatorID == user.UserID);
            foreach (Team team in userTeams) 
            {
                if (teams.SingleOrDefault(t => t.CompetitorID == team.CompetitorID) == null)
                {
                    teams.Add(team);
                }
            }

            List<long> ids = new List<long>();
            foreach (Player p in players)
            {
                ids.Add(p.CompetitorID);
            }
            foreach (Team t in teams)
            {
                ids.Add(t.CompetitorID);
            }
            
            List<Match> matches = db.Matches.Include("Scores.Competitor")
                .Where(m => m.Scores
                                .Any(s => ids.Contains(s.CompetitorID)))
                .OrderByDescending(s => s.Date)
                .Take(5).ToList();

            
            ViewBag.AllPlayerProfiles = players;
            ViewBag.AllTeams = teams;
            ViewBag.AllMatches = matches;
            ViewBag.Genders = user.Gender.ToSelectList();

            return View("~/Views/User/Profile.cshtml", user);
        }

        //
        // POST: /UpdateProfile/

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(User userModel)
        {
            if (userModel.UserID != 0)
            {
                DbEntityEntry dbEntityEntry = db.Entry(userModel);
                if (dbEntityEntry.State == EntityState.Detached)
                {
                    db.Users.Attach(userModel);
                }
                dbEntityEntry.State = EntityState.Modified;
            }
            else
            {
                db.Users.Add(userModel);
            }
            db.SaveChanges();
            return RedirectToAction("UserProfile");
        }
    }
}
