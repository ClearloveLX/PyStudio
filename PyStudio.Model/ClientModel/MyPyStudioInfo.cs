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

    public class PyUserInfo
    {

    }
}
