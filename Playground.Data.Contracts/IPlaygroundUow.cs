using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Contracts
{
    public interface IPlaygroundUow
    {
        void Commit();

        // Repositories
        IRepository<GameCategory> GameCategories { get; }
        IRepository<Game> Games { get; }
        
        IGameCompetitionTypeRepository GameCompetitionTypes { get; }
        ICompetitionTypeRepository CompetitionTypes { get; }
    }
}
