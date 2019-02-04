using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PM.Data.Repository
{
    public interface IRepository<TEntity> where TEntity: class
    {
        IEnumerable<TEntity> GetAll();
        Task<TEntity> GetById(object id);
        Task Create(TEntity entity);
        Task Update(object id, TEntity entity);
        Task Delete(object id);
    }
}
