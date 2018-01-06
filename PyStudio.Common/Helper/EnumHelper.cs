using System;
using System.Collections.Generic;
using System.Text;

namespace PyStudio.Common.Helper
{
    /// <summary>
    /// 枚举支援类
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 日志类别
        /// </summary>
        public enum EmLogStatus
        {
            注册 = 0,
            登录 = 1,
            增加 = 2,
            修改 = 3,
            删除 = 4
        }
        /// <summary>
        /// 用户账号状态
        /// </summary>
        public enum EmUserStatus
        {
            禁用 = 0, 启用 = 1
        }

        /// <summary>
        /// 邮件模板
        /// </summary>
        public enum EmEmailTpl
        {
            /// <summary>
            /// 消息通知
            /// </summary>
            MsgBox = 1,

            /// <summary>
            /// 绑定邮箱
            /// </summary>
            SettingEmail = 2,

            /// <summary>
            /// 绑定手机
            /// </summary>
            SettingTel = 3
        }
    }
}
