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
    }
}
