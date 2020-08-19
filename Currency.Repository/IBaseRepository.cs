using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Currency.Repository
{
    public interface IBaseRepository<T> where T : class, new()
    {
        Task<T> Insert(T entity);

        void Update(T entity);

        //Task<int> Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> entity);

        void Delete(T entity);

        //Task<int> Delete(Expression<Func<T, bool>> whereLambda);

        Task<bool> IsExist(Expression<Func<T, bool>> whereLambda);

        Task<T> GetEntity(Expression<Func<T, bool>> whereLambda);

        Task<List<T>> Select();

        Task<List<T>> Select(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="pageSize">条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="whereLambda">条件</param>
        /// <param name="orderByLambda">排序</param>
        /// <param name="isAsc">是否正序</param>
        /// <returns></returns>
        Task<ServicePageResult<T>> Select<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc);

    }
}
