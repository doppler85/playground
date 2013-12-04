using Playground.Model;
using System.Linq;

namespace Playground.Data.Contracts
{
    public interface ICompetitionTypeRepository : IRepository<CompetitionType>
    {
        IQueryable<CompetitionType> GetAvailableByGameId(int id);
    }
}
