using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PyStudio.Model.Models;
using PyStudio.Model.ClientModel;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MemberController : Controller
    {
        private readonly PyStudioDBContext _context;
        private readonly PySelfSetting _selfSetting;

        public MemberController(PyStudioDBContext context, IOptions<PySelfSetting> selfSetting)
        {
            _context = context;
            _selfSetting = selfSetting.Value;
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
