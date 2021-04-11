using Currency.Models.DB_Entity;
using Currency.Repository;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Currency.Service.SugarDemo
{
    public interface ISugarSystemUserDAL : IBaseRepository<SystemUserEntity>
    {
        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="pageSize">条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="whereLambda">条件表达式</param>
        /// <param name="orderByLambda">orderby表达式</param>
        /// <param name="isAsc">true 正序 false 倒叙</param>
        /// <returns></returns>
        Task<ServicePageResult<SystemUserEntity>> SelectPage(int pageSize, int pageIndex, Expression<Func<SystemUserEntity, bool>> whereLambda, Expression<Func<SystemUserEntity, object>> orderByLambda, bool isAsc = true);

    }
}
