using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data
{
    public class CompetitionTypeRepository : EFRepository<CompetitionType>, ICompetitionTypeRepository
    {
        public CompetitionTypeRepository(DbContext context) : base(context) { }

        public IQueryable<CompetitionType> GetAvailableByGameId(int id)
        {
            return DbSet.Where(ct => !ct.Games.Any(g => g.GameID == id)).Distinct();
        }
    }
}
