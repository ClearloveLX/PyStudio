using System;
using System.Collections.Generic;
using System.Text;

namespace PyStudio.Model.ClientModel
{
    /// <summary>
    /// 自定义配置
    /// </summary>
    public class PySelfSetting
    {
        /// <summary>
        /// 头像图片保存地址 
        /// </summary>
        public string UpHeadPhotoPath { get; set; }

        /// <summary>
        /// 头像图片访问地址 
        /// </summary>
        public string ViewHeadPhotoPath { get; set; }

        /// <summary>
        /// 内容图片保存地址 
        /// </summary>
        public string UpContentPhotoPath { get; set; }

        /// <summary>
        /// 查看内容图片保存地址 
        /// </summary>
        public string ViewContentPhotoPath { get; set; }

        /// <summary>
        /// 邮件模板文件夹路径 
        /// </summary>
        public string EmailTplPath { get; set; }

        /// <summary>
        /// 数据库链接
        /// </summary>
        public string DbLink { get; set; }
    }

    /// <summary>
    /// 用户信息存储
    /// </summary>
    public class PyUserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string UserNickName { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string UserEmail { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string UserTel { get; set; }
        /// <summary>
        /// 性别
        /// @Note 0女 1男 2默认/未知/保密
        /// </summary>
        public int UserSex { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string UserIntroduce { get; set; }
        /// <summary>
        /// 头像地址
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
        /// 登录时间
        /// </summary>
        public DateTime? UserLoginTime { get; set; }
        /// <summary>
        /// 登录Ip
        /// </summary>
        public string UserIps { get; set; }
    }

    /// <summary>
    /// 登录信息
    /// </summary>
    public class PyStudioPromptData
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int IsOK { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }
}
