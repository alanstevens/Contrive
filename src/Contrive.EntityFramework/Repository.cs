using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Contrive.Core;
using Contrive.Core.Extensions;

namespace Contrive.EntityFramework
{
  public class Repository<T>
    : DisposableBase, IRepository<T> where T : class
  {
    public Repository(DbContext context)
    {
      _context = context;
      _dbSet = _context.Set<T>();
    }

    readonly DbContext _context;
    readonly DbSet<T> _dbSet;

    public IQueryable<T> GetQuery()
    {
      return _dbSet;
    }

    public IEnumerable<T> GetAll()
    {
      return _dbSet.AsEnumerable();
    }

    public IEnumerable<T> Where(Func<T, bool> where)
    {
      return _dbSet.Where(where);
    }

    public T Single(Func<T, bool> where)
    {
      return _dbSet.Single(where);
    }

    public T First(Func<T, bool> where)
    {
      return _dbSet.First(where);
    }

    public T FirstOrDefault(Func<T, bool> where)
    {
      return _dbSet.FirstOrDefault(where);
    }

    public virtual T GetByID(object id)
    {
      return _dbSet.Find(id);
    }

    public virtual void Delete(object id)
    {
      T entity = GetByID(id);
      Delete(entity);
    }

    public void Delete(T entity)
    {
      AttachIfNeeded(entity);
      _dbSet.Remove(entity);
    }

    public void Insert(T entity)
    {
      _dbSet.Add(entity);
    }

    public void Update(T entity)
    {
      _dbSet.Attach(entity);
      _context.Entry(entity).State = EntityState.Modified;
    }

    public void SaveChanges()
    {
      _context.SaveChanges();
    }

    public virtual IEnumerable<T> GetWithSql(string sql, params object[] parameters)
    {
      return _dbSet.SqlQuery(sql, parameters).ToList();
    }

    protected override void OnDisposing(bool disposing)
    {
      if (_context.IsNotNull())
        _context.Dispose();
    }

    void AttachIfNeeded(T entity)
    {
      if (_context.Entry(entity).State == EntityState.Detached)
        _dbSet.Attach(entity);
    }
  }
}