using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Currency.Repository.DB_Dapper
{
    public interface IDapperContext
    {
        DbConnection Create();
    }
}
