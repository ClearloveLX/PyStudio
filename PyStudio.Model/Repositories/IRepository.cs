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
    /// 仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="filter">筛选器</param>
        /// <param name="orderBy">排序方式</param>
        /// <param name="includeProperties">include</param>
        /// <returns></returns>
        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null
                                    , Func<IQueryable<TEntity>
                                    , IOrderedQueryable<TEntity>> orderBy = null
                                    , string includeProperties = "");

        /// <summary>
        /// 获取列表(异步)
        /// </summary>
        /// <param name="filter">筛选器</param>
        /// <param name="orderBy">排序方式</param>
        /// <param name="includeProperties">include</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null
                                    , Func<IQueryable<TEntity>
                                    , IOrderedQueryable<TEntity>> orderBy = null
                                    , string includeProperties = "");

        /// <summary>
        /// 根据主键获取数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        TEntity GetById(object id);

        /// <summary>
        /// 根据主键获取数据(异步)
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsync(object id);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        int Insert(TEntity entity);

        /// <summary>
        /// 新增(异步)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> InsertAsync(TEntity entity);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        int Update(TEntity entity);

        /// <summary>
        /// 修改(异步)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity entity);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Delete(object id);

        /// <summary>
        /// 删除(异步)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(object id);

        Task<bool> SaveLogInfoAsync(int userId, string info, int operation, string ips);

        ///// <summary>
        ///// 批量删除
        ///// </summary>
        ///// <param name="entity">要删除的实体</param>
        ///// <returns></returns>
        //int Delete(TEntity entity);

        ///// <summary>
        ///// 批量删除(异步)
        ///// </summary>
        ///// <param name="entity">要删除的实体</param>
        ///// <returns></returns>
        //Task<int> DeleteAsync(TEntity entity);
    }
}
