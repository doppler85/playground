using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Playground.Web.Controllers
{
    public class CompetitorController : ApiBaseController
    {
        public CompetitorController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }


        [HttpGet]
        [ActionName("matches")]
        public PagedResult<Match> GetMatches(string id, int page, int count)
        {
            int competitorId = Int32.Parse(id);
            User currentUser = GetUserByEmail(User.Identity.Name);
            int totalItems = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Status == MatchStatus.Confirmed && 
                                                    m.Scores.Any(s => s.CompetitorID == competitorId))
                                        .Count();

            List<Match> matches = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Status == MatchStatus.Confirmed && 
                                                    m.Scores.Any(s => s.CompetitorID == competitorId))
                                        .OrderByDescending(s => s.Date)
                                        .Skip((page - 1) * count)
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

            PagedResult<Match> retVal = new PagedResult<Match>()
            {
                CurrentPage = page,
                TotalPages = (totalItems + count - 1) / count,
                TotalItems = totalItems,
                Items = matches
            };

            return retVal;
        }
    }
}