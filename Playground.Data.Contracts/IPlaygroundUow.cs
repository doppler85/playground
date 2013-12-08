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
        IRepository<CompetitionType> CompetitionTypes { get; }
        IRepository<Game> Games { get; }
        IRepository<Competitor> Competitors { get; }
        IRepository<Match> Matches { get; }
        IRepository<CompetitorScore> Scores { get; }
        
        IGameCompetitionTypeRepository GameCompetitionTypes { get; }
        IUserRepository Users { get; }
    }
}
