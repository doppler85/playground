using Playgorund.Webexperiment.Filters;
using Playground.Data;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Playgorund.Webexperiment.Controllers
{
    
    [InitializeSimpleMembership]
    public class PlayerController : CompetitorController
    {
        //
        // GET: /AddPlayer/

        [Authorize]
        public ActionResult AddPlayer()
        {
            Player player = new Player();
            var roles = (SimpleRoleProvider)Roles.Provider;
            if (roles.IsUserInRole(WebSecurity.CurrentUserName, Util.Constants.RoleNames.Admin))
            {
                RedirectToAction("AddPlayerAdmin");
            }
            else
            {
                User currentUser = db.Users.FirstOrDefault(u => u.ExternalUserID == WebSecurity.CurrentUserId);
                if (currentUser == null)
                {
                    currentUser = new User()
                    {
                        ExternalUserID = WebSecurity.CurrentUserId
                    };
                    db.Users.Add(currentUser);
                    db.SaveChanges();
                }

                player.UserID = currentUser.UserID;
            }
            List<SelectListItem> games = new List<SelectListItem>();
            List<Game> individualGames = db.Games.Where(g => g.CompetitionTypes.Any(ct => ct.CompetitorType == CompetitorType.Individual)).ToList();
            foreach (Game g in individualGames)
            {
                games.Add(new SelectListItem()
                {
                    Text = g.Title,
                    Value = g.GameID.ToString()
                });
            }
            ViewBag.Games = games;

            return View("~/Views/Player/AddPlayer.cshtml", player);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPlayer(Player player)
        {
            player.Status = CompetitorStatus.Active;
            player.CreationDate = DateTime.Now;
            db.Competitors.Add(player);
            db.SaveChanges();

            return RedirectToAction("UserProfile", "User");
        }

        //
        // GET: /AddPlayerAdmin/
        [Authorize(Roles="Admin")]
        public ActionResult AddPlayerAdmin()
        {
            return View();
        }

    }
}
