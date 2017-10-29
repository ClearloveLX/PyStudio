using System;
using System.Collections.Generic;

namespace PyStudio.Model.Models
{
    public partial class InfoLogger
    {
        public int LoggerId { get; set; }
        public int? LoggerUserId { get; set; }
        public string LoggerDescription { get; set; }
        public int? LoggerOperation { get; set; }
        public DateTime? LoggerCreateTime { get; set; }
        public string LoggerIps { get; set; }
    }
}
