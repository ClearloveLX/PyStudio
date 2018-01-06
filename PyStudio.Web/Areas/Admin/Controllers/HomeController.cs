using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PyStudio.Web.Models;
using System.Diagnostics;
using PyStudio.Web.Extends;

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error(string msg = null)
        {
            this.MsgBox(msg ?? "访问出问题了，开发人员正从火星赶回来修复，请耐心等待！");
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }
    }
}