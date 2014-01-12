using Playground.Business.Contracts;
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
    public class MatchController : ApiBaseController
    {
        public MatchController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        [HttpGet]
        [ActionName("matches")]
        public PagedResult<Match> GetMatches(int page, int count)
        {
            int totalItems = Uow.Matches
                                        .GetAll()
                                        .Where(m => m.Status == MatchStatus.Confirmed)
                                        .Count();

            List<Match> matches = Uow.Matches
                                        .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                        .Where(m => m.Status == MatchStatus.Confirmed)
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