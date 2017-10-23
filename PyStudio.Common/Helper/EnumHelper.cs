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
        public enum EmoLogStatus
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
    }
}
