using System;
using System.Collections.Generic;
using System.Text;

namespace PyStudio.Model.Models.SmallApp
{
    public partial class InfoMessageBoard
    {
        public int MessageBoardId { get; set; }
        /// <summary>
        /// 发送用户
        /// </summary>
        public string MessageBoardUser { get; set; }
        /// <summary>
        /// 发送者Ip
        /// </summary>
        public string MessageBoardIp { get; set; }
        /// <summary>
        /// 录入日期
        /// </summary>
        public DateTime MessageBoardCreateTime { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string MessageBoardContent { get; set; }
    }
}
