using System;
using System.Collections.Generic;

namespace PyStudio.Model.Models.Account
{
    public partial class InfoUser
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 真实姓名/昵称
        /// </summary>
        public string UserNickName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string UserPwd { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string UserEmail { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string UserTel { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int UserSex { get; set; }
        /// <summary>
        /// 个人介绍
        /// </summary>
        public string UserIntroduce { get; set; }
        /// <summary>
        /// 头像位置
        /// </summary>
        public string UserHeadPhoto { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? UserBirthday { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string UserAddress { get; set; }
        /// <summary>
        /// 博客地址
        /// </summary>
        public string UserBlog { get; set; }
        /// <summary>
        /// 启用状态
        /// </summary>
        public int UserStatus { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime UserCreateTime { get; set; }
        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime? UserLoginTime { get; set; }
        /// <summary>
        /// 上次登录IP
        /// </summary>
        public string UserIps { get; set; }
    }
}
