using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Playground.Data.Contracts;
using Playground.Data.Helpers;
using Playground.Model;

namespace Playground.Data
{
    public class PlaygroundUow : IPlaygroundUow, IDisposable
    {
        private PlaygroundDbContext DbContext { get; set; }

        protected IRepositoryProvider RepositoryProvider { get; set; }

        public IRepository<GameCategory> GameCategories
        {
            get { return GetStandardRepo<GameCategory>(); }
        }

        public IRepository<Game> Games
        {
            get { return GetStandardRepo<Game>(); }
        }

        public IRepository<GameCompetitionType> GameCompetitionTypes
        {
            get { return GetStandardRepo<GameCompetitionType>(); }
        }

        public IRepository<CompetitionType> CompetitionTypes
        {
            get { return GetStandardRepo<CompetitionType>(); }
        }

        public IRepository<User> Users
        {
            get { return GetStandardRepo<User>(); }
        }

        public IRepository<Competitor> Competitors
        {
            get { return GetStandardRepo<Competitor>(); }
        }

        public IRepository<GameCompetitor> GameCompetitors
        {
            get { return GetStandardRepo<GameCompetitor>(); }
        }

        public IRepository<CompetitorScore> CompetitorScores 
        {
            get { return GetStandardRepo<CompetitorScore>(); } 
        }

        public IRepository<Match> Matches
        {
            get { return GetStandardRepo<Match>(); }
        }

        public IRepository<CompetitorScore> Scores
        {
            get { return GetStandardRepo<CompetitorScore>(); }
        }

        public IRepository<TeamPlayer> TeamPlayers
        {
            get { return GetStandardRepo<TeamPlayer>(); }
        }

        public IRepository<AutomaticMatchConfirmation> AutomaticMatchConfirmations 
        {
            get { return GetStandardRepo<AutomaticMatchConfirmation>(); }
        }

        public PlaygroundUow(IRepositoryProvider repositoryProvider)
        {
            CreateDbContext();
            repositoryProvider.DbContext = DbContext;
            RepositoryProvider = repositoryProvider;
        }

        public void Commit()
        {
            DbContext.SaveChanges();
        }

        private void CreateDbContext()
        {
            DbContext = new PlaygroundDbContext();

            // Do NOT enable proxied entities, else serialization fails
            DbContext.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = false;

            // Because Web API will perform validation, we don't need/want EF to do so
            DbContext.Configuration.ValidateOnSaveEnabled = false;

            // disable auto detect changes in EF
            DbContext.Configuration.AutoDetectChangesEnabled = false;
        }

        private IRepository<T> GetStandardRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepositoryForEntityType<T>();
        }
        private T GetRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepository<T>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }
    }
}
