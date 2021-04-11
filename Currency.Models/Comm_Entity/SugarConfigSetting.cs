using System.Collections.Generic;

namespace Currency.Models.Comm_Entity
{
    /// <summary>
    /// sugar配置
    /// </summary>
    public class SugarConfigSetting
    {
        public string Conn { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 从库属性
        /// </summary>
        public List<SlaveConnection> SlaveConnectionList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SlaveConnection
    {
        public string Conn { get; set; }

        /// <summary>
        /// 权重越大概率越大
        /// </summary>
        public int HitRate { get; set; }
    }
}
