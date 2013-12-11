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
        IRepository<GameCompetitionType> GameCompetitionTypes { get; }
        IRepository<Game> Games { get; }
        IRepository<User> Users { get; }
        IRepository<Competitor> Competitors { get; }
        IRepository<GameCompetitor> GameCompetitors { get; }
        IRepository<Match> Matches { get; }
        IRepository<CompetitorScore> Scores { get; }
        IRepository<TeamPlayer> TeamPlayers { get; }
        
        // Custom repositories
        // IGameCompetitionTypeRepository GameCompetitionTypes { get; }
    }
}
