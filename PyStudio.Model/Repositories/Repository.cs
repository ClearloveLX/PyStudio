using Microsoft.EntityFrameworkCore;
using PyStudio.Model.Models;
using PyStudio.Model.Models.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PyStudio.Model.Repositories
{
    /// <summary>
    /// 仓储接口实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly PyStudioDBContext _context;
        private DbSet<TEntity> _Entities;

        public Repository(PyStudioDBContext context)
        {
            _context = context;
            _Entities = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null
                                            , Func<IQueryable<TEntity>
                                            , IOrderedQueryable<TEntity>> orderBy = null
                                            , string includeProperties = "")
        {
            IQueryable<TEntity> query = _Entities;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null
                                            , Func<IQueryable<TEntity>
                                            , IOrderedQueryable<TEntity>> orderBy = null
                                            , string includeProperties = "")
        {
            IQueryable<TEntity> query = _Entities;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public TEntity GetById(object id)
        {
            return _Entities.Find(id);
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _Entities.FindAsync(id);
        }

        public int Insert(TEntity entity)
        {
            _Entities.Add(entity);
            return _context.SaveChanges();
        }

        public async Task<int> InsertAsync(TEntity entity)
        {
            await _Entities.AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public int Update(TEntity entity)
        {
            _Entities.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return _context.SaveChanges();
        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            _Entities.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public int Delete(object id)
        {
            TEntity entityToDelete = _Entities.Find(id);
            if (_context.Entry(entityToDelete).State == EntityState.Deleted)
            {
                _Entities.Attach(entityToDelete);
            }
            _Entities.Remove(entityToDelete);
            return _context.SaveChanges();
        }

        public async Task<int> DeleteAsync(object id)
        {
            TEntity entityToDelete = await _Entities.FindAsync(id);
            if (_context.Entry(entityToDelete).State == EntityState.Deleted)
            {
                _Entities.Attach(entityToDelete);
            }
            _Entities.Remove(entityToDelete);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> SaveLogInfoAsync(int userId, string info, int operation, string ips) {
            var result = false;
            _context.Add(new SysLogger
            {
                LoggerUser = userId,
                LoggerDescription = info,
                LoggerOperation = operation,
                LoggerCreateTime = DateTime.Now,
                LoggerIps = ips
            });

            var save = await _context.SaveChangesAsync();

            if (save > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
