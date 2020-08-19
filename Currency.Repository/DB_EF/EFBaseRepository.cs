using Currency.Repository.DB_EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Currency.Repository
{
    public class EFBaseRepository<T> where T : class, new()
    {
        protected readonly MyDbContext myDbContext;
        protected readonly MyReadDbContext myDbContextRead;

        public EFBaseRepository(MyDbContext myDbContext, MyReadDbContext myDbContextRead)
        {
            this.myDbContext = myDbContext;
            this.myDbContextRead = myDbContextRead;
        }

        public async Task<T> Insert(T entity)
        {
            var model = await myDbContextRead.Set<T>().AddAsync(entity);
            await myDbContextRead.SaveChangesAsync();
            return model.Entity;
        }

        public void Update(T entity)
        {
            myDbContextRead.Set<T>().Update(entity);
            myDbContextRead.SaveChanges();
        }

        //public async Task<int> Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> entity)
        //{
        //    return await myDbContext.Set<T>().Where(whereLambda).UpdateAsync(entity);
        //}

        public void Delete(T entity)
        {
            myDbContextRead.Set<T>().Remove(entity);
            myDbContextRead.SaveChanges();
        }
        //public async Task<int> Delete(Expression<Func<T, bool>> whereLambda)
        //{
        //    return await myDbContext.Set<T>().Where(whereLambda).DeleteAsync();
        //}

        public async Task<bool> IsExist(Expression<Func<T, bool>> whereLambda)
        {
            return await myDbContextRead.Set<T>().AnyAsync(whereLambda);
        }

        public async Task<T> GetEntity(Expression<Func<T, bool>> whereLambda)
        {
            return await myDbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(whereLambda);
        }

        public async Task<List<T>> Select()
        {
            return await myDbContext.Set<T>().ToListAsync();
        }

        public async Task<List<T>> Select(Expression<Func<T, bool>> whereLambda)
        {
            return await myDbContext.Set<T>().Where(whereLambda).ToListAsync();
        }

        public async Task<ServicePageResult<T>> Select<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc)
        {
            ServicePageResult<T> res = new ServicePageResult<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            res.TotalSize = await myDbContext.Set<T>().Where(whereLambda).CountAsync();

            if (isAsc)
            {
                var entities = await myDbContext.Set<T>().Where(whereLambda)
                                      .OrderBy<T, S>(orderByLambda)
                                      .Skip(pageSize * (pageIndex - 1))
                                      .Take(pageSize).ToListAsync();
                res.List = entities;
                return res;
            }
            else
            {
                var entities = await myDbContext.Set<T>().Where(whereLambda)
                                      .OrderByDescending<T, S>(orderByLambda)
                                      .Skip(pageSize * (pageIndex - 1))
                                      .Take(pageSize).ToListAsync();

                return res;
            }
        }

    }
}
