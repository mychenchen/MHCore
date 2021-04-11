namespace Currency.Models.Comm_Entity
{
    /// <summary>
    /// 配置项实体
    /// </summary>
    public class WebAppSetting
    {
        /// <summary>
        /// 是否为测试环境
        /// </summary>
        public bool IsDebug { get; set; }

        /// <summary>
        /// Redis - db数据库
        /// </summary>
        public int RedisDbNum { get; set; }

        /// <summary>
        /// Redis链接
        /// </summary>
        public string RedisConnection { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string JwtSecret { get; set; }
        /// <summary>
        /// 发行人
        /// </summary>
        public string JwtIss { get; set; }
        /// <summary>
        /// 订阅人
        /// </summary>
        public string JwtAud { get; set; }
    }
}
