using System;
using System.Collections.Generic;
using System.Linq;

namespace Contrive.Common
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IQueryable<T> GetQuery();

        IEnumerable<T> GetAll();

        IEnumerable<T> Where(Func<T, bool> where);

        T Single(Func<T, bool> where);

        T First(Func<T, bool> where);

        T FirstOrDefault(Func<T, bool> where);

        T GetByID(object id);

        void Delete(object id);

        void Delete(T entity);

        void Insert(T entity);

        void Update(T entity);

        void SaveChanges();

        IEnumerable<T> GetWithSql(string sql, params object[] parameters);
    }
}