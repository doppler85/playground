using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data.Contracts
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeExpressions);
        T GetById(object id);
        T GetById(Expression<Func<T, bool>> keyFunction, params Expression<Func<T, object>>[] includeExpressions);
        void Add(T entity);
        void Update(T entity, params object[] keyValues);
        void Delete(T entity);
        void Delete(object id);
        void Delete(Expression<Func<T, bool>> keyFunction);
    }
}
