using Currency.Common;
using Currency.Common.DIRegister;
using Currency.Models.Comm_Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Currency.Repository.DB_SqlSugar
{
    /// <summary>
    /// Sugar通用实现接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SugarBaseRepository<T> where T : class, new()
    {
        protected static SqlSugarClient db;

        /// <summary>
        /// 1.如果存在事务所有操作都走主库，不存在事务 修改、写入、删除走主库，查询操作走从库
        /// 2.HitRate 越大走这个从库的概率越大
        /// </summary>
        public SugarBaseRepository()
        {
            var _config = DI.GetService<IOptions<SugarConfigSetting>>().Value;
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _config.Conn,//连接符字串
                DbType = GetDbType(_config.DbType),
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                //从库
                SlaveConnectionConfigs = _config.SlaveConnectionList.Select(a => new SlaveConnectionConfig
                {
                    HitRate = a.HitRate,
                    ConnectionString = a.Conn,
                }).ToList()
            });

            //添加Sql打印事件，开发中可以删掉这个代码
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
#if DEBUG
                Console.WriteLine(sql);
#endif
            };
        }

        private DbType GetDbType(string t)
        {
            if (t == "MySql")
                return DbType.MySql;
            else if (t == "SqlServer")
                return DbType.SqlServer;
            else if (t == "Sqlite")
                return DbType.Sqlite;
            else if (t == "Oracle")
                return DbType.Oracle;
            else if (t == "PostgreSQL")
                return DbType.PostgreSQL;
            else if (t == "Dm")
                return DbType.Dm;
            else if (t == "Kdbndp")
                return DbType.Kdbndp;
            else
                return DbType.SqlServer;
        }

        #region 必须实现的接口

        /// <summary>
        /// 添加,返回添加结果类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> Insert(T entity)
        {
            return await db.Insertable(entity).ExecuteReturnEntityAsync();
        }

        public void Update(T entity)
        {
            db.Updateable(entity).ExecuteCommand();
        }

        public void Delete(T entity)
        {
            db.Deleteable(entity).ExecuteCommand();
        }

        public async Task<bool> IsExist(Expression<Func<T, bool>> whereLambda)
        {
            return await db.Queryable<T>().Where(whereLambda).AnyAsync();
        }

        public async Task<T> GetEntity(Expression<Func<T, bool>> whereLambda)
        {
            return await db.Queryable<T>().FirstAsync(whereLambda);
        }

        public async Task<List<T>> Select()
        {
            return await db.Queryable<T>().ToListAsync();
        }

        public async Task<List<T>> Select(Expression<Func<T, bool>> whereLambda)
        {
            return await db.Queryable<T>().Where(whereLambda).ToListAsync();
        }

        [Obsolete("已弃用,请用SelectPage方法", true)]
        public async Task<ServicePageResult<T>> Select<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc)
        {
            throw new Exception("已弃用,请用SelectPage方法");
        }

        #endregion

        #region 异步基础方法

        /// <summary>
        /// 批量添加,返回结果数量
        /// </summary>
        /// <param name="add">数据集合</param>
        /// <returns></returns>
        public async Task<int> Insert(List<T> add)
        {
            return await db.Insertable(add).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新指定的字段
        /// </summary>
        /// <param name="updateLambda"></param>
        /// <returns></returns>
        public async Task<int> Update(Expression<Func<T, object>> updateLambda)
        {
            return await db.Updateable<T>().UpdateColumns(updateLambda).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据表达式更新,更新指定的字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public async Task<int> Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> updateLambda)
        {
            return await db.Updateable<T>().Where(whereLambda).UpdateColumns(updateLambda).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据表达式删除数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public async Task<int> Delete(Expression<Func<T, bool>> whereLambda)
        {
            return await db.Deleteable<T>().Where(whereLambda).ExecuteCommandAsync();
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="pageSize">条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="whereLambda">条件表达式</param>
        /// <param name="orderByLambda">orderby表达式</param>
        /// <param name="isAsc">true 正序 false 倒叙</param>
        /// <returns></returns>
        public async Task<ServicePageResult<T>> SelectPage(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderByLambda, bool isAsc = true)
        {
            ServicePageResult<T> res = new ServicePageResult<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            RefAsync<int> total = 0;
            if (isAsc)
                res.List = await db.Queryable<T>().OrderBy(orderByLambda, OrderByType.Asc)
                    .ToPageListAsync(pageIndex, pageSize, total);
            else
                res.List = await db.Queryable<T>().OrderBy(orderByLambda, OrderByType.Desc)
                    .ToPageListAsync(pageIndex, pageSize, total);

            res.TotalSize = total.Value;

            return res;
        }
        #endregion

    }
}
