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
using Microsoft.AspNetCore.Hosting;
using PyStudio.Model.Repositories;

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserCenterController : BaseController
    {
        private readonly PyStudioDBContext _context;
        private readonly PySelfSetting _pySelfSetting;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UserCenterController(PyStudioDBContext context, IOptions<PySelfSetting> pySelfSetting, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _pySelfSetting = pySelfSetting.Value;
            _hostingEnvironment = hostingEnvironment;
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
                            LoggerUser = _MyUserInfo.UserId,
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


        #region 邮箱相关
        public IActionResult SettingEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SettingEmail(string email)
        {
            var data = new PyStudioPromptData();
            if (string.IsNullOrWhiteSpace(email))
            {
                data.IsOK = 2;
                data.Msg = "请输入邮箱！";
                return Json(data);
            }
            email = email.Trim();
            if (email.Length >= 50 || email.Length <= 3)
            {
                data.IsOK = 2;
                data.Msg = "邮箱长度不符！";
                return Json(data);
            }
            else if (!email.Contains("@"))
            {
                data.IsOK = 2;
                data.Msg = "邮箱格式不正确！";
                return Json(data);
            }
            var timeOut = 30;
            var now = DateTime.Now.AddMinutes(timeOut);
            var expires = now.ToString("yyyy-MM-dd hh:mm:ss");
            var token = $"{expires}-{email}-{Request.Host.Host}-{_MyUserInfo.UserId}"._Md5();
            var appUrl = $"http://{Request.Host.Host}:{Request.Host.Port}";
            var comfirmUrl = $"{appUrl}/Admin/UserCenter/ConfirmSettingEmail?expire={expires}&token={token}&email={email}&t=0.9527{_MyUserInfo.UserId}";

            //读取邮箱模版
            var tpl = await ExtentionsClass._GetHtmlTpl(EmEmailTpl.SettingEmail, $"{_hostingEnvironment.WebRootPath}/{_pySelfSetting.EmailTplPath}");
            if (string.IsNullOrWhiteSpace(tpl))
            {
                data.IsOK = 0;
                data.Msg = "发送绑定邮件失败，请稍后再试！";
                return Json(data);
            }
            string name = _MyUserInfo.UserNickName;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = _MyUserInfo.UserName;
            }
            tpl = tpl.Replace("{name}", name).Replace("{content}", $"您正在使用<a href='{appUrl}'>PyStudio交易网</a>邮箱绑定功能<a href='{comfirmUrl}'>{comfirmUrl}</a>；注意该地址有效时间{timeOut}分钟。");
            //发送
            var isOk = ExtentionsClass._SendEmail(new Dictionary<string, string> {
                { name,email}
            }, "PyStudio交易网", tpl);

            data.IsOK = isOk ? 1 : 0;
            data.Msg = isOk ? "已给您邮箱发送了绑定确认邮件，请收件后点击确认绑定链接地址。" : "发送绑定邮件失败，请稍后重试！";

            return Json(data);
        }

        public async Task<IActionResult> ConfirmSettingEmail(string expire, string token, string email, string t)
        {
            if (string.IsNullOrWhiteSpace(expire) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email) || !email.Contains("@") || string.IsNullOrWhiteSpace(t))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "无效的请求。", Area = "Admin" });
            }
            if (!DateTime.TryParse(expire, out var expires))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "无效的请求。", Area = "Admin" });
            }
            else if (expires.AddMinutes(30) > DateTime.Now)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "请求已过期，请重新操作！。", Area = "Admin" });
            }
            t = t.Replace("0.9527", "");
            var compareToken = $"{expire}-{email}-{Request.Host.Host}-{t}"._Md5();
            if (!token.Equals(compareToken))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "验证失败，请确认操作是否正确！", Area = "Admin" });
            }
            var userId = t.ToInt();
            if (userId != _MyUserInfo.UserId)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "验证失败，请登录后重新绑定！", Area = "Admin" });
            }
            var userInfo = _context.InfoUser.Where(b => b.UserId == userId && b.UserStatus == (int)EmUserStatus.启用).SingleOrDefault();
            if (userInfo == null)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "绑定失败，用户信息无效或被禁用！", Area = "Admin" });
            }

            userInfo.UserEmail = email;
            _MyUserInfo.UserEmail = email;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                HttpContext.Session.Set<PyUserInfo>(HttpContext.Session.SessionKey(), _MyUserInfo);
                _context.SysLogger.Add(new SysLogger
                {
                    LoggerUser = _MyUserInfo.UserId,
                    LoggerDescription = $"修改邮箱为{email}",
                    LoggerOperation = (int)EmLogStatus.修改,
                    LoggerCreateTime = DateTime.Now,
                    LoggerIps = _MyUserInfo.UserIps
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserCenterController.ModifyUserInfo), "UserCenter", new { Area = "Admin" });
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "绑定失败，请联系管理员！", Area = "Admin" });
            }
        }
        #endregion
    }
}