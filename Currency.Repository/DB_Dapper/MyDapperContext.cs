using Currency.Repository.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Currency.Repository.DB_Dapper
{
    /// <summary>
    /// Dapper上下文
    /// </summary>
    public class MyDapperContext : IDapperContext
    {
        protected readonly DapperOptions _opt;
        public MyDapperContext(IOptions<DapperOptions> options)
        {
            _opt = options.Value;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        public DbConnection Create()
        {
            DbConnection conn = null;
            switch (_opt.DbModels)
            {
                case DbModel.SqlServer:
                    conn = new SqlConnection(_opt.ConnectionString);
                    break;
                case DbModel.MySql:
                    //conn = new MySqlConnection(_opt.ConnectionString);
                    //Dapper.SimpleCRUD
                    throw new Exception("未实现Mysql相关");
                default:
                    throw new Exception("未知数据库类型");
            }


            return conn;
        }
    }
}
