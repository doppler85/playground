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
    public class HomeController : ApiBaseController
    {
        public HomeController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        [HttpGet]
        [ActionName("matches")]
        public List<Match> GetMatches(int count)
        {
            List<Match> matches = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Status == MatchStatus.Confirmed)
                                        .OrderByDescending(s => s.Date)
                                        .Take(count)
                                        .ToList();

            List<long> competitorIds = matches
                .SelectMany(m => m.Scores)
                .ToList()
                .Select(s => s.CompetitorID)
                .ToList();

            List<Competitor> competitors = Uow.Competitors
                .GetAll()
                .Where(c => competitorIds.Contains(c.CompetitorID))
                .ToList();

            return matches;
        }

        [HttpGet]
        [ActionName("competitors")]
        public List<Competitor> GetCompetitors(int count)
        {
            List<Competitor> competitors = Uow.Competitors
                                                .GetAll(c => c.Games)
                                                .OrderByDescending(c => c.CreationDate)
                                                .Take(count)
                                                .ToList();

            List<int> gameIds = competitors
                                                .SelectMany(c => c.Games)
                                                .ToList()
                                                .Select(g => g.GameID)
                                                .ToList();

            List<Game> categories = Uow.Games
                                                .GetAll(g => g.Category)
                                                .Where(g => gameIds.Contains(g.GameID))
                                                .ToList();

            return competitors;
        }
    }
}
