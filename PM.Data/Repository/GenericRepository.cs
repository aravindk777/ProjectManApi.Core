using Microsoft.EntityFrameworkCore;
using PM.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Data.Repository
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly PMDbContext _dbContext;

        public GenericRepository(PMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>()
                .AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(object id)
        {
            var entity = await GetById(id);
            _dbContext.Set<TEntity>()
                .Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>().AsEnumerable();
        }

        public async Task<TEntity> GetById(object id)
        {
            return await _dbContext.Set<TEntity>()
                .FindAsync(id);
        }

        public async Task Update(object id, TEntity entity)
        {
            _dbContext.Set<TEntity>()
                .Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
