using System;
using System.Collections.Generic;
using System.Text;

namespace PyStudio.Model.ClientModel
{
    /// <summary>
    /// 地区/区域
    /// </summary>
    public class AreaInfo
    {
        /// <summary>
        /// 地区ID
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 地区编号
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 上级地区名称
        /// </summary>
        public string UpAreaName { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 上级路径
        /// </summary>
        public string UpAreaPathId { get; set; }
        /// <summary>
        /// 地区路径ID
        /// Level=0:|当前ID|
        /// Level=1：|上级PathId|本级ID|
        /// ...
        /// </summary>
        public string AreaPathId { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public string AreaPid { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int AreaLevel { get; set; }
        /// <summary>
        /// 地图坐标
        /// </summary>
        public string AreaCoord { get; set; }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string AreaZipCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string AreaNote { get; set; }
    }
}
