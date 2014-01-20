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
    public class CompetitorBusiness : PlaygroundBusinessBase, ICompetitorBusiness
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CompetitorBusiness(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        public List<long> GetCompetitorIdsForUser(long userID)
        {
            List<long> retVal = null;
            try
            {
                List<long> playerIds = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.UserID == userID)
                    .Select(p => p.CompetitorID)
                    .ToList();

                List<long> teamIds = Uow.Competitors
                    .GetAll()
                    .OfType<Player>()
                    .Where(p => p.UserID == userID)
                    .SelectMany(p => p.Teams)
                    .Select(t => t.Team.CompetitorID)
                    .ToList();

                retVal = playerIds.Concat(teamIds).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Error retrieving competitor ids", ex);
            }
            return retVal;
        }
    }
}
