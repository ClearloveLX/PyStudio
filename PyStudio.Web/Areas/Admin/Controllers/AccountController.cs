using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PyStudio.Model.Models;
using PyStudio.Model.ClientModel;
using Microsoft.Extensions.Options;
using PyStudio.Web.Extends;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly PyStudioDBContext _context;
        private readonly PySelfSetting _selfSetting;

        public AccountController(PyStudioDBContext context, IOptions<PySelfSetting> selfSetting)
        {
            _context = context;
            _selfSetting = selfSetting.Value;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public IActionResult Login(string returnUrl = null)
        {
            var _userInfo = HttpContext.Session.Get<PyUserInfo>(HttpContext.Session.SessionKey());
            if (_userInfo != null)
            {
                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    return Redirect("/Admin/Home/Index");
                }
                else
                {
                    Redirect(returnUrl);
                }
                return View();
            }
            this.MsgBox(returnUrl?? "/Admin/Home/Index", "returnUrl");
            return View();
        }

        public IActionResult UpHeadPhoto()
        {
            return View();
        }
    }
}
