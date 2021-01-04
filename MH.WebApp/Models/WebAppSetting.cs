using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MH.WebApp.Models
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
