using Currency.Models.DB_Entity;
using Currency.Repository.DB_SqlSugar;
using SqlSugar;

namespace Currency.Service.SugarDemo
{
    /// <summary>
    /// 系统用户信息
    /// </summary>
    //[UseDI(ServiceLifetime.Scoped, typeof(ISystemUserDAL))]
    public class SugarSystemUserDAL : SugarBaseRepository<SystemUserEntity>, ISugarSystemUserDAL
    {
        public SugarSystemUserDAL()
            : base()
        {

        }

    }
}
