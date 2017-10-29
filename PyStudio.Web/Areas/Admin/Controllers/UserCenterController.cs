using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PyStudio.Web.Extends;
using PyStudio.Model.Models;
using PyStudio.Model.ClientModel;
using Microsoft.Extensions.Options;
using System.IO;
using static PyStudio.Common.Helper.EnumHelper;

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserCenterController : BaseController
    {
        private readonly PyStudioDBContext _context;
        private readonly PySelfSetting _pySelfSetting;

        public UserCenterController(PyStudioDBContext context, IOptions<PySelfSetting> pySelfSetting)
        {
            _context = context;
            _pySelfSetting = pySelfSetting.Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <returns></returns>
        public IActionResult UpHeadPhoto()
        {
            return View(_MyUserInfo);
        }
    }
}