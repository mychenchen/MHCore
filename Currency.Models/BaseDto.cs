using System;
using System.ComponentModel.DataAnnotations;

namespace Currency.Models
{
    /// <summary>
    /// 通用字段
    /// </summary>
    public class BaseDto
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否删除 0否 1是
        /// </summary>
        public int IsDelete { get; set; }

        /// <summary>
        /// 是否删除 0否 1是
        /// </summary>
        public string IsDeleteStr
        {
            get
            {
                var str = "";
                if (this.IsDelete == 1)
                {
                    str = "已删除";
                }
                else
                {
                    str = "未删除";
                }
                return str;
            }
        }
    }
}
