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

        public IGameCompetitionTypeRepository GameCompetitionTypes
        {
            get { return GetRepo<IGameCompetitionTypeRepository>(); }
        }

        public ICompetitionTypeRepository CompetitionTypes
        {
            get { return GetRepo<ICompetitionTypeRepository>(); }
        }

        public IUserRepository Users
        {
            get { return GetRepo<IUserRepository>(); }
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
