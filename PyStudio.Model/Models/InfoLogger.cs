using System;
using System.Collections.Generic;

namespace PyStudio.Model.Models.Sys
{
    public partial class SysLogger
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public int LoggerId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? LoggerUserId { get; set; }
        /// <summary>
        /// 日志说明
        /// </summary>
        public string LoggerDescription { get; set; }
        /// <summary>
        /// 操作介绍
        /// </summary>
        public int? LoggerOperation { get; set; }
        /// <summary>
        /// 录入日期
        /// </summary>
        public DateTime? LoggerCreateTime { get; set; }
        /// <summary>
        /// 操作IP
        /// </summary>
        public string LoggerIps { get; set; }
    }
}
