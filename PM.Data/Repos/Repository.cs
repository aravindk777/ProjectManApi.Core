using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PM.Data.Repos
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext _context)
        {
            Context = _context;
        }

        public int Count()
        {
            return Context.Set<T>().Count();
        }

        public T Create(T entity)
        {
            try
            {
                Context.Set<T>().Add(entity);
                Context.SaveChanges();
                Context.Entry<T>(entity).Reload();
                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(T entity)
        {
            try
            {
                Context.Set<T>().Remove(entity);
                return Context.SaveChanges() != 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>().AsEnumerable();
        }

        public virtual T GetById(object identifier)
        {
            return Context.Set<T>().Find(identifier);
        }

        public IEnumerable<T> Search(Expression<Func<T, bool>> query)
        {
            return Context.Set<T>().Where(query);
        }

        public bool Update(T entity)
        {
            try
            {
                //context.Entry(entity).State = EntityState.Modified;
                Context.Set<T>().Update(entity);
                return Context.SaveChanges() != 0;
            }
            catch (Exception)
            {                
                throw;
            }
        }
    }
}
