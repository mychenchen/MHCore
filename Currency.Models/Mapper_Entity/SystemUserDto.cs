using Currency.Models.DB_Entity;
using System;

namespace Currency.Models.Mapper_Entity
{
    /// <summary>
    /// 系统用户信息    Dto
    /// </summary>
    [AutoMappers(typeof(SystemUserEntity))]
    public class SystemUserDto : BaseDto
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 登陆账号
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        /// 加盐
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 最近一次登陆时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

    }
}
