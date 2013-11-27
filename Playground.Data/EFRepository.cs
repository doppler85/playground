using Playground.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
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

        public virtual IQueryable<T> GetAll(string include)
        {
            return DbSet.Include(include);
        }

        public virtual T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public virtual T GetById(long id)
        {
            return DbSet.Find(id);
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

        private object GetPrimaryKey(DbEntityEntry entry)
        {
            var myObject = entry.Entity;
            var property =
                myObject.GetType()
                     .GetProperties().FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)));
            return property.GetValue(myObject, null);
        }

        public virtual void Update(T entity) 
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            var key = GetPrimaryKey(dbEntityEntry);

            if (dbEntityEntry.State == EntityState.Detached)
            {
                var attachetEntity = DbSet.Find(key);
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

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) return; 
            Delete(entity);
        }

        public virtual void Delete(long id)
        {
            var entity = GetById(id);
            if (entity == null) return;
            Delete(entity);
        }
    }
}
