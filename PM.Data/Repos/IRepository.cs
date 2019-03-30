using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PM.Data.Repos
{
    public interface IRepository<T> where T : class
    {
        T Create(T entity);
        IEnumerable<T> GetAll();
        T GetById(object identifier);
        bool Update(T entity);
        bool Delete(T entity);
        IEnumerable<T> Search(Expression<Func<T, bool>> query);
        int Count();
    }
}
