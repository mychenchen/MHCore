using Currency.Models.DB_Entity;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Currency.Repository.DB_EF
{
    /// <summary>
    /// 主库(写库)
    /// </summary>
    public class MyDbContext : DbContext
    {
        /// <summary>
        /// 主库(写库)
        /// </summary>
        public MyDbContext(DbContextOptions<MyDbContext> options)
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
