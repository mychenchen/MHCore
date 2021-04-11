﻿using SqlSugar;
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
    [SugarTable("User")]
    public class SystemUserEntity : BaseEntity
    {
        /// <summary>
        /// 标签ID
        /// </summary>
        public Guid LabelId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid RoleId { get; set; }

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
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 自我介绍
        /// </summary>
        public string MyIntroduce { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(200)]
        public string HeadImg { get; set; }
    }
}
