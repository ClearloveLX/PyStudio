using System;
using System.Collections.Generic;

namespace PyStudio.Model.Models
{
    public partial class InfoArea
    {
        public int AreaId { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string AreaPathId { get; set; }
        public string AreaPid { get; set; }
        public int? AreaLevel { get; set; }
        public string AreaCoord { get; set; }
        public string AreaZipCode { get; set; }
        public string AreaNote { get; set; }
    }
}
