using log4net;
using Playground.Business.Contracts;
using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business
{
    public class MatchBusiness : PlaygroundBusinessBase, IMatchBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MatchBusiness(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        public Result<PagedResult<Match>> FilterByUser(int page, int count, int userID)
        {
            Result<PagedResult<Match>> retVal = null;
            try
            {
                throw new NotImplementedException();
                
                // retVal = ResultHandler<PagedResult<Match>>.Sucess(result);
                //Uow.Competitors.GetAll(p => ((Player)p).Teams)
                //    .OfType<Player>()
                //    .Where(p => p.UserID == userID)

                IQueryable<Player> myPlayers = Uow.Competitors
                    .GetAll(p => ((Player)p).Teams)
                    .OfType<Player>()
                    .Where(p => p.UserID == userID);
                
                // IQueryable<Team> myTeams = myPlayers.SelectMany(p => p.Teams)

                Uow.Matches.GetAll()
                    .SelectMany(s => s.Scores)
                    .Select(s => s.Match)
                    .OrderBy(m => m.Date);
            }
            catch (Exception ex)
            {
                log.Error("Error getting list of matches", ex);
                retVal = ResultHandler<PagedResult<Match>>.Erorr("Error getting list of matches");
            }

            return retVal;
        }

        public Result<PagedResult<Match>> FilterByStatus(int page, int count, MatchStatus status)
        {
            Result<PagedResult<Match>> retVal = null;
            try
            {
                int totalItems = Uow.Matches
                            .GetAll()
                            .Where(m => m.Status == status)
                            .Count();

                page = GetPage(totalItems, page, count);

                List<Match> matches = Uow.Matches
                                            .GetAll(m => m.Winner, m => m.Game, m => m.Scores)
                                            .Where(m => m.Status == status)
                                            .OrderByDescending(s => s.Date)
                                            .Skip((page - 1) * count)
                                            .Take(count)
                                            .ToList();

                foreach (CompetitorScore competitorScore in matches.SelectMany(m => m.Scores)) 
                {
                    competitorScore.Competitor = Uow.Competitors.GetById(competitorScore.CompetitorID);
                }

                PagedResult<Match> result = new PagedResult<Match>()
                {
                    CurrentPage = page,
                    TotalPages = (totalItems + count - 1) / count,
                    TotalItems = totalItems,
                    Items = matches
                };

                retVal = ResultHandler<PagedResult<Match>>.Sucess(result);
            }
            catch (Exception ex)
            {
                log.Error("Error getting list of matches", ex);
                retVal = ResultHandler<PagedResult<Match>>.Erorr("Error getting list of matches");
            }

            return retVal;
        }
    }
}
