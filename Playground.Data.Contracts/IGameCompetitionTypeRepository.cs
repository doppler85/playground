using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Contracts
{
    public interface IGameCompetitionTypeRepository : IRepository<GameCompetitionType>
    {
        IQueryable<GameCompetitionType> GetByGameId(int id);
        GameCompetitionType GetByIds(int gameId, int comeptitionTypeId);
        void Delete(int gameId, int comeptitionTypeId);
    }
}
