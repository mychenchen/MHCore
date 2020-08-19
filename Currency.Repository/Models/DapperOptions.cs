namespace Currency.Repository.Models
{
    public class DapperOptions
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        public DbModel DbModels { get; set; }

    }

}
