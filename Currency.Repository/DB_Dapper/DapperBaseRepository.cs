using Dapper;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Currency.Repository.DB_Dapper
{
    public class DapperBaseRepository<T>
    {
        protected readonly DbConnection _conn;
        public DapperBaseRepository(MyDapperContext myDapperContext)
        {
            _conn = myDapperContext.Create();
        }


        public void Insert(T entity)
        {
            _conn.InsertAsync<Key, T>(entity);
        }

        public void Update(T entity)
        {
            _conn.UpdateAsync(entity);
        }

        //Task<int> Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> entity);

        public void Delete(T entity)
        {
            _conn.DeleteAsync(entity);
        }

        //Task<int> Delete(Expression<Func<T, bool>> whereLambda);

        public Task<T> GetEntity(string id)
        {
            return _conn.GetAsync<T>(id);
            //return _conn.QueryFirstAsync<T>($"select * from {typeof(T).Name} where id = @ID", new { ID = id });
        }

        public List<T> Select()
        {
            return _conn.Query<T>($"select * from {typeof(T).Name}").ToList();
        }

    }
}
