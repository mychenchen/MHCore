using Currency.Models.DB_Entity;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Currency.Repository.DB_EF
{
    /// <summary>
    /// 从库(读库)
    /// </summary>
    public class MyReadDbContext : DbContext
    {
        /// <summary>
        /// 从库(读库)
        /// </summary>
        /// <param name="options"></param>
        public MyReadDbContext(DbContextOptions<MyReadDbContext> options)
            : base(options)
        {

        }

        #region 映射到数据库

        /// <summary>
        /// 系统用户表
        /// </summary>
        public DbSet<SystemUserEntity> User { get; set; }

        #endregion
    }
}
