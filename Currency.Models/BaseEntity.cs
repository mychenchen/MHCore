using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations;

namespace Currency.Models
{
    /// <summary>
    /// 通用字段
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否删除 0否 1是
        /// </summary>
        public int IsDelete { get; set; } = 0;
    }
}
