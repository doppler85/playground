using Playground.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Helpers
{
    public class RepositoryFactories
    {
        private readonly IDictionary<Type, Func<DbContext, object>> _repositoryFactories;

        private IDictionary<Type, Func<DbContext, object>> GetPlaygoundFactories()
        {
            return new Dictionary<Type, Func<DbContext, object>> 
            { 
                // add facty methods for creating non standard repositories
                {typeof(IGameCompetitionTypeRepository), dbContext =>  new GameCompetitionTypeRepository(dbContext)},
                {typeof(ICompetitionTypeRepository), dbContext =>  new CompetitionTypeRepository(dbContext)},
                {typeof(IUserRepository), dbContext =>  new UserRepository(dbContext)}
                // {typeof(IAttendanceRepository), dbContext => new AttendanceRepository(dbContext)},
                // { typeof(Ig
            };
        }

        public RepositoryFactories()
        {
            _repositoryFactories = GetPlaygoundFactories();
        }

        public RepositoryFactories(IDictionary<Type, Func<DbContext, object>> factories)
        {
            _repositoryFactories = factories;
        }

        public Func<DbContext, object> GetRepositoryFactory<T>()
        {

            Func<DbContext, object> factory;
            _repositoryFactories.TryGetValue(typeof(T), out factory);
            return factory;
        }

        public Func<DbContext, object> GetRepositoryFactoryForEntityType<T>() where T : class
        {
            return GetRepositoryFactory<T>() ?? DefaultEntityRepositoryFactory<T>();
        }

        protected virtual Func<DbContext, object> DefaultEntityRepositoryFactory<T>() where T : class
        {
            return dbContext => new EFRepository<T>(dbContext);
        }
    }
}
