using Currency.Models.DB_Entity;
using Currency.Repository.DB_EF;
using Currency.Service.IService;
using System;

namespace Currency.Service
{
    /// <summary>
    /// 系统用户信息
    /// </summary>
    //[UseDI(ServiceLifetime.Scoped, typeof(ISystemUserDAL))]
    public class SystemUserDAL : EFBaseRepository<SystemUserEntity>, ISystemUserDAL
    {
        public SystemUserDAL(MyDbContext myDbContext, MyReadDbContext myDbContextRead) : base(myDbContext, myDbContextRead)
        {

        }

        /// <summary>
        /// 事务处理
        /// </summary>
        public void DemoTScope()
        {
            using (var transaction = myDbContext.Database.BeginTransaction())
            {
                try
                {
                    // context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/dotnet" });
                    myDbContext.SaveChanges();

                    // context.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/visualstudio" });
                    myDbContext.SaveChanges();

                    //var blogs = context.Blogs.OrderBy(b => b.Url).ToList();

                    // Commit transaction if all commands succeed, transaction will auto-rollback
                    // when disposed if either commands fails
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                }
            }
        }

    }
}
