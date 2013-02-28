using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth.EntityFramework
{
    public class Repository<T> : DisposableBase, IRepository<T> where T : class
    {
        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        readonly DbContext _context;
        readonly DbSet<T> _dbSet;

        public virtual IQueryable<T> GetQuery()
        {
            return _dbSet;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.AsEnumerable();
        }

        public virtual IEnumerable<T> Where(Func<T, bool> where)
        {
            return _dbSet.Where(where);
        }

        public virtual T Single(Func<T, bool> where)
        {
            return _dbSet.Single(where);
        }

        public virtual T First(Func<T, bool> where)
        {
            return _dbSet.First(where);
        }

        public virtual T FirstOrDefault(Func<T, bool> where)
        {
            return _dbSet.FirstOrDefault(where);
        }

        public virtual T GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Delete(object id)
        {
            var entity = GetByID(id);
            Delete(entity);
        }

        public virtual void Delete(T entity)
        {
            AttachIfNeeded(entity);
            _dbSet.Remove(entity);
        }

        public virtual void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        public virtual IEnumerable<T> GetWithSql(string sql, params object[] parameters)
        {
            return _dbSet.SqlQuery(sql, parameters).ToList();
        }

        protected override void OnDisposing(bool disposing)
        {
            if (_context.IsNotNull()) _context.Dispose();
        }

        void AttachIfNeeded(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached) _dbSet.Attach(entity);
        }
    }
}