using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Currency.Models.DB_Entity
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    [Table("User")]
    public class SystemUserEntity: BaseEntity
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string NickName { get; set; }

        /// <summary>
        /// 登陆账号
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LoginName { get; set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LoginPwd { get; set; }

        /// <summary>
        /// 加盐
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Salt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 最近一次登陆时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

    }
}
