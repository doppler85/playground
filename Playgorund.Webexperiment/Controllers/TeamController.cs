using Playgorund.Webexperiment.Filters;
using Playground.Data;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Playgorund.Webexperiment.Controllers
{
    [InitializeSimpleMembership]
    public class TeamController : CompetitorController
    {
    
        //
        // GET: /AddTeam/

        [Authorize]
        public ActionResult AddTeam()
        {
            Team team = new Team();
            var roles = (SimpleRoleProvider)Roles.Provider;
            if (roles.IsUserInRole(WebSecurity.CurrentUserName, Util.Constants.RoleNames.Admin))
            {
                RedirectToAction("AddTeamAdmin");
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

                team.CreatorID = currentUser.UserID;
            }

            List<SelectListItem> games = new List<SelectListItem>();
            List<Game> teamGames = db.Games.Where(g => g.CompetitionTypes.Any(ct => ct.CompetitionType.CompetitorType == CompetitorType.Team)).ToList();
            foreach (Game g in teamGames)
            {
                games.Add(new SelectListItem()
                {
                    Text = g.Title,
                    Value = g.GameID.ToString()
                });
            }
            ViewBag.Games = games;

            return View("~/Views/Team/AddTeam.cshtml", team);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTeam(Team team)
        {
            team.Status = CompetitorStatus.Active;
            User creator = db.Users.First(u => u.ExternalUserID == WebSecurity.CurrentUserId);
            team.Creator = creator;
            team.CreationDate = DateTime.Now;
            db.Competitors.Add(team);
            db.SaveChanges();

            return RedirectToAction("UserProfile", "User");
        }

        //
        // GET: /AddTeamAdmin/

        public ActionResult AddTeamAdmin()
        {
            return View();
        }

        //
        // GET: /Team/Details

        [Authorize]
        public ActionResult Details(long id)
        {
            Team team = db.Competitors
                .Include("Players.Player.User")
                .Include("Creator")
                .Include("Game")
                .OfType<Team>()
                .FirstOrDefault(t => t.CompetitorID == id);

            return View(team);
        }

        //
        // GET: /Team/AddPlayer

        [Authorize]
        public ActionResult AddPlayer(long teamID)
        {
            Team team = db.Competitors
                .OfType<Team>()
                .FirstOrDefault(t => t.CompetitorID == teamID);

            var playerIds = team.Players.Select(p => p.PlayerID).ToList();

            var query = (
                    from  gamePlayers in db.Competitors.OfType<Player>()
                    where
                        gamePlayers.GameID == team.GameID &&
                        !playerIds.Contains(gamePlayers.CompetitorID)
                    select gamePlayers
            );

            List<Player> availablePlayers = query.ToList();

            List<SelectListItem> availablePlayersList = new List<SelectListItem>();
            foreach (Player p in availablePlayers)
            {
                availablePlayersList.Add(new SelectListItem()
                {
                    Text = String.Format("{0} ({1} {2})", p.Name, p.User.FirstName, p.User.LastName),
                    Value = p.CompetitorID.ToString()
                });
            }

            ViewBag.AvailablePlayers = availablePlayersList;
            TeamPlayer teamPlayer = new TeamPlayer() 
            {
                Team = team
            };

            return View(teamPlayer);
        }

        //
        // POST: /Team/AddPlayer

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPlayer(TeamPlayer player)
        {
            db.TeamPlayers.Add(player);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = player.TeamID });
        }

        //
        // GET: /Team/RemovePlayer

        [Authorize]
        public ActionResult RemovePlayer(long teamID, long playerID)
        {
            TeamPlayer player = db.TeamPlayers
                .Include("Team")
                .Include("Player.User")
                .FirstOrDefault(tp => tp.TeamID == teamID && tp.PlayerID == playerID);
            return View(player);
        }

        //
        // POST: /Team/RemovePlayer
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemovePlayer(TeamPlayer player)
        {
            DbEntityEntry dbEntityEntry = db.Entry(player);
            dbEntityEntry.State = System.Data.EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = player.TeamID });
        }
    }
}
