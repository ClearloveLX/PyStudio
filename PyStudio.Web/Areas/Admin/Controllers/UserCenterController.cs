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
using PyStudio.Model.Models.Sys;

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

        /// <summary>
        /// 修改信息GET
        /// </summary>
        /// <returns></returns>
        public IActionResult ModifyUserInfo()
        {
            return View(_MyUserInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ModifyUserInfo([Bind("UserId,UserName,UserNickName,UserEmail,UserTel,UserIntroduce,UserBirthday,UserAddress,UserBlog")]PyUserInfo pyUserInfo)
        {
            var data = new PyStudioPromptData();
            if (ModelState.IsValid)
            {
                var userInfo = _context.InfoUser.Where(b => b.UserId.Equals(pyUserInfo.UserId)).FirstOrDefault();
                if (userInfo != null)
                {
                    userInfo.UserNickName = pyUserInfo.UserNickName;
                    userInfo.UserEmail = pyUserInfo.UserEmail;
                    userInfo.UserTel = pyUserInfo.UserTel;
                    userInfo.UserIntroduce = pyUserInfo.UserIntroduce;
                    userInfo.UserBirthday = pyUserInfo.UserBirthday;
                    userInfo.UserAddress = pyUserInfo.UserAddress;
                    userInfo.UserBlog = pyUserInfo.UserBlog;

                    _MyUserInfo.UserNickName = pyUserInfo.UserNickName;
                    _MyUserInfo.UserEmail = pyUserInfo.UserEmail;
                    _MyUserInfo.UserTel = pyUserInfo.UserTel;
                    _MyUserInfo.UserIntroduce = pyUserInfo.UserIntroduce;
                    _MyUserInfo.UserBirthday = pyUserInfo.UserBirthday;
                    _MyUserInfo.UserAddress = pyUserInfo.UserAddress;
                    _MyUserInfo.UserBlog = pyUserInfo.UserBlog;

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        HttpContext.Session.Set<PyUserInfo>(HttpContext.Session.SessionKey(), _MyUserInfo);
                        _MyUserInfo = HttpContext.Session.Get<PyUserInfo>(HttpContext.Session.SessionKey());

                        data.IsOK = 1;
                        data.Msg = "修改成功";
                        _context.SysLogger.Add(new SysLogger
                        {
                            LoggerUserId = _MyUserInfo.UserId,
                            LoggerDescription = $"用户{_MyUserInfo.UserName}{EmLogStatus.修改}个人信息",
                            LoggerOperation = (int)EmLogStatus.修改,
                            LoggerCreateTime = DateTime.Now,
                            LoggerIps = this.GetUserIp()
                        });
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        data.IsOK = 0;
                        data.Msg = "修改失败！请稍后再试...";
                        return Json(data);
                    }
                }
                else
                {
                    data.IsOK = 0;
                    data.Msg = "修改失败！请稍后再试...";
                    return Json(data);
                }
            }
            else
            {
                data.IsOK = 0;
                data.Msg = "修改失败！请稍后再试...";
                return Json(data);
            }
            return Json(data);
        }
    }
}