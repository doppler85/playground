using Playground.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        public EFRepository(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }

        protected DbContext DbContext { get; set; }
        protected DbSet<T> DbSet { get; set; }

        public virtual IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public virtual IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeExpressions)
        {
            return includeExpressions.Aggregate<Expression<Func<T, object>>, IQueryable<T>>
               (DbSet, (current, expression) => current.Include(expression));
        }

        public virtual T GetById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual T GetById(Expression<Func<T, bool>> keyFunction, 
            params Expression<Func<T, object>>[] includeExpressions)
        {
            if (includeExpressions.Any())
            {
                var set = includeExpressions.Aggregate<Expression<Func<T, object>>, IQueryable<T>>
                                (DbSet, (current, expression) => current.Include(expression));

                return set.SingleOrDefault(keyFunction);
            }
            return DbSet.SingleOrDefault(keyFunction);
        }

        public virtual void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }

        public virtual void Update(T entity, params object[] keyValues)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);

            if (dbEntityEntry.State == EntityState.Detached)
            {
                var attachetEntity = DbSet.Find(keyValues);
                if (attachetEntity != null)
                {
                    var attachedEntry = DbContext.Entry(attachetEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    DbSet.Attach(entity);
                    dbEntityEntry.State = EntityState.Modified;
                }
            }
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public virtual void Delete(object id)
        {
            var entity = GetById(id);
            if (entity == null) return;
            Delete(entity);
        }


        public virtual void Delete(Expression<Func<T, bool>> keyFunction)
        {
            var entity = GetById(keyFunction);
            if (entity == null) return;
            Delete(entity);
        }
    }
}
