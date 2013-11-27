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
    public class MatchController : Controller
    {
        private PlaygroundDbContext db = new PlaygroundDbContext();

        //
        // GET: /Match/

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult LatestMatches(int matchesCount)
        {
            List<Match> latestMatches = db.Matches
                .Include("Scores.Competitor")
                .OrderByDescending(m => m.Date)
                .Take(matchesCount)
                .ToList();


            return PartialView("~/Views/Match/_ListMatchPartial.cshtml", latestMatches);
        }

        [Authorize]
        public ActionResult Create(int? gameID, int? competitionTypeID)
        {
            Match match = new Match();
            var roles = (SimpleRoleProvider)Roles.Provider;
            if (roles.IsUserInRole(WebSecurity.CurrentUserName, Util.Constants.RoleNames.Admin))
            {
                RedirectToAction("CreateAdmin");
            }


            var userGames = (
                from player in db.Competitors.OfType<Player>()
                where player.User.ExternalUserID == WebSecurity.CurrentUserId
                select new
                    {
                        GameID = player.GameID,
                        Title = player.Game.Title
                    }
                ).ToList();

            var userTeamGames = (
                from teamPlayer in db.TeamPlayers
                where teamPlayer.Player.User.ExternalUserID == WebSecurity.CurrentUserId
                select new
                    {
                        GameID = teamPlayer.Player.GameID,
                        Title = teamPlayer.Player.Game.Title
                    }
                ).ToList();

            userGames.AddRange(userTeamGames);
            userGames = userGames.Distinct().ToList();

            List<SelectListItem> games = new List<SelectListItem>();
            games.Add(new SelectListItem()
            {
                Text = "Select game",
                Value = ""
            });

            foreach (var g in userGames)
            {
                games.Add(new SelectListItem()
                {
                    Text = g.Title,
                    Value = g.GameID.ToString()
                });
            }

            Game selectedGame = null;
            if (gameID.HasValue)
            {
                selectedGame = db.Games.First(g => g.GameID == gameID.Value);
                List<SelectListItem> competitionTypes = new List<SelectListItem>();
                competitionTypes.Add(new SelectListItem()
                {
                    Text = "Select competition type",
                    Value = ""
                });

                foreach (var ct in selectedGame.CompetitionTypes)
                {
                    competitionTypes.Add(new SelectListItem()
                    {
                        Text = ct.CompetitionType.Name,
                        Value = ct.CompetitionTypeID.ToString()
                    });
                }
                ViewBag.CompetitionTypes = competitionTypes;
            }

            CompetitionType selectedCompetitionType = null;
            if (competitionTypeID.HasValue)
            {
                selectedCompetitionType = selectedGame.CompetitionTypes.First(ct => ct.CompetitionType.CompetitionTypeID == competitionTypeID.Value).CompetitionType;
            }

            if (selectedGame != null && selectedCompetitionType != null)
            {
                match.Scores = new List<CompetitorScore>();
                List<SelectListItem> myCompetitors = new List<SelectListItem>();
                List<SelectListItem> oponentCompetitors = new List<SelectListItem>();

                if (selectedCompetitionType.CompetitorType == CompetitorType.Individual)
                {
                    var myPlayers = (
                        from player in db.Competitors.OfType<Player>()
                        where player.User.ExternalUserID == WebSecurity.CurrentUserId &&
                              player.GameID == selectedGame.GameID
                        select new
                        {
                            CompetitorID = player.CompetitorID,
                            Name = player.Name + "(" + player.User.FirstName + " " + player.User.LastName + ")"
                        }
                        ).ToList();

                    var oponentPlayers = (
                        from player in db.Competitors.OfType<Player>()
                        where player.User.ExternalUserID != WebSecurity.CurrentUserId &&
                              player.GameID == selectedGame.GameID
                        select new
                        {
                            CompetitorID = player.CompetitorID,
                            Name = player.Name + "(" + player.User.FirstName + " " + player.User.LastName + ")"
                        }
                        ).ToList();

                    foreach (var competitor in myPlayers)
                    {
                        myCompetitors.Add(new SelectListItem()
                        {
                            Text = competitor.Name,
                            Value = competitor.CompetitorID.ToString()
                        });
                    }

                    foreach (var competitor in oponentPlayers)
                    {
                        oponentCompetitors.Add(new SelectListItem()
                        {
                            Text = competitor.Name,
                            Value = competitor.CompetitorID.ToString()
                        });
                    }
                }
                else
                {
                    var myTeams = (
                        from teamPlayer in db.TeamPlayers
                        where teamPlayer.Player.User.ExternalUserID == WebSecurity.CurrentUserId && 
                              teamPlayer.Player.Game.GameID == selectedGame.GameID
                        select new
                        {
                            CompetitorID = teamPlayer.TeamID,
                            Name = teamPlayer.Team.Name
                        }
                        ).ToList();

                    List<long> teamIds = myTeams.Select(t => t.CompetitorID).ToList();

                    var oponentTeams = (
                        from teamPlayer in db.TeamPlayers
                        where teamPlayer.Player.Game.GameID == selectedGame.GameID &&
                              !teamIds.Contains(teamPlayer.TeamID)
                        select new
                        {
                            CompetitorID = teamPlayer.TeamID,
                            Name = teamPlayer.Team.Name
                        }
                        ).Distinct().ToList();

                    foreach (var competitor in myTeams)
                    {
                        myCompetitors.Add(new SelectListItem()
                        {
                            Text = competitor.Name,
                            Value = competitor.CompetitorID.ToString()
                        });
                    }

                    foreach (var competitor in oponentTeams)
                    {
                        oponentCompetitors.Add(new SelectListItem()
                        {
                            Text = competitor.Name,
                            Value = competitor.CompetitorID.ToString()
                        });
                    }
                }

                ViewBag.MyCompetitors = myCompetitors;
                ViewBag.OponentCompetitors = oponentCompetitors;

            }

            ViewBag.Games = games;
            if (!gameID.HasValue)
            {
                return View("~/Views/Match/Create_Step1.cshtml", match);
            }
            else if (gameID.HasValue && !competitionTypeID.HasValue) 
            {
                return View("~/Views/Match/Create_Step2.cshtml", match);
            }
            else
            {
                match.GameID = selectedGame.GameID;
                match.CompetitionTypeID = selectedCompetitionType.CompetitionTypeID;
                match.Date = DateTime.Now;
                match.Scores = new List<CompetitorScore>();
                for(int i = 0; i < selectedCompetitionType.CompetitorsCount; i++) {
                    match.Scores.Add(new CompetitorScore());
                }
                return View("~/Views/Match/Create_Step3.cshtml", match);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Match match)
        {
            match.Status = MatchStatus.Submited;
            match.WinnerID = match.Scores.OrderByDescending(s => s.Score).First().CompetitorID;
            db.Matches.Add(match);
            db.SaveChanges();

            return RedirectToAction("UserProfile", "User");

        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateAdmin()
        {
            throw new NotImplementedException("Method is not implemented, yet");
        }
    }
}
