using System;
using System.Collections.Generic;

namespace PyStudio.Model.Models.BaseInfo
{
    /// <summary>
    /// 地区信息
    /// </summary>
    public partial class InfoArea
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
        /// 地区名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string AreaPathId { get; set; }
        /// <summary>
        /// 上级ID
        /// </summary>
        public string AreaPid { get; set; }
        /// <summary>
        /// 当前等级
        /// </summary>
        public int AreaLevel { get; set; }
        /// <summary>
        /// 坐标位置
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

    /// <summary>
    /// Excel测试表(需要数据库与实体字段百分百对应)
    /// </summary>
    public partial class InfoEi
    {
        public string Eicol1 { get; set; }
        public string Eicol2 { get; set; }
        public string Eicol3 { get; set; }
        public string Eicol4 { get; set; }
    }
}
