using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PyStudio.Web.Areas.Admin.Models
{
    public class CallbackViewModel
    {
        public DateTime? CachedTime { get; set; }
        public string Message { get; set; }
    }
}
