using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PyStudio.Model.Models;
using PyStudio.Model.ClientModel;
using Microsoft.Extensions.Options;
using PyStudio.Web.Extends;
using Microsoft.EntityFrameworkCore;
using static PyStudio.Common.Helper.EnumHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly PyStudioDBContext _context;
        private readonly PySelfSetting _selfSetting;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;

        public AccountController(PyStudioDBContext context, IOptions<PySelfSetting> selfSetting, IHostingEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _context = context;
            _selfSetting = selfSetting.Value;
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
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
                    return RedirectToAction(nameof(HomeController.Index), "Home", new { Area = "Admin" });
                }
                else
                {
                    Redirect(returnUrl);
                }
                return View();
            }
            this.MsgBox(returnUrl ?? "/Admin/Home/Index", "returnUrl");
            return View();
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public IActionResult LoginOut()
        {
            HttpContext.Session.Remove(HttpContext.Session.SessionKey());
            return RedirectToAction(nameof(AccountController.Login), "Account", new { Area = "Admin" });
        }

        #region 忘记密码

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<PyStudioPromptData> ForgetPassword(string email)
        {
            var data = new PyStudioPromptData();
            if (string.IsNullOrWhiteSpace(email))
            {
                data.IsOK = 2;
                data.Msg = "请输入邮箱！";
                return data;
            }

            email = email.Trim().ToLower();
            if (email.Length >= 50 || email.Length <= 3)
            {
                data.IsOK = 2;
                data.Msg = "邮箱长度不符！";
                return data;
            }
            else if (!email.Contains("@"))
            {
                data.IsOK = 2;
                data.Msg = "邮箱格式不正确！";
                return data;
            }


            var userInfo = await _context.InfoUser.SingleOrDefaultAsync(m => m.UserEmail.ToLower() == email);

            if (userInfo == null)
            {
                data.IsOK = 0;
                data.Msg = "找不到绑定该邮箱的账号！";
                return data;
            }
            else if (userInfo.UserStatus == (int)EmUserStatus.禁用)
            {
                data.IsOK = 0;
                data.Msg = "该邮箱已被封禁，您可以通过发送邮件至：gk1213656215@outlook.com 联系管理员！";
                return data;
            }

            var timeOut = 10;
            var now = DateTime.Now.AddMinutes(timeOut);
            var expires = now.ToString("yyyy-MM-dd hh:mm");
            var token = $"{expires}-{email}-{Request.Host.Host}"._Md5();
            var appUrl = $"http://{Request.Host.Host}:{Request.Host.Port}";
            var comfirmUrl = $"{appUrl}/Admin/Account/ConfirmPassword?expire={expires}&token={token}&={email}&t=0.{now.ToString("ssfff")}";

            //读取模版
            var tpl = await ExtentionsClass._GetHtmlTpl(EmEmailTpl.MsgBox, $"{_hostingEnvironment.WebRootPath}/{_selfSetting.EmailTplPath}");
            if (string.IsNullOrWhiteSpace(tpl))
            {
                data.IsOK = 0;
                data.Msg = "发送绑定邮箱失败，请稍后重试。";
                return data;
            }
            tpl = tpl.Replace("{name}", $"尊敬的用户{userInfo.UserNickName}")
                     .Replace("{content}", $"您正在使用 <a href = '{appUrl}'>PyStudio</a> 邮箱重置密码功能，请点击以下链接确认绑定邮箱 <a href = '{comfirmUrl}'>{ comfirmUrl}</a>；注意该地址有效时间{timeOut}分钟。");
            var isOk = ExtentionsClass._SendEmail(new Dictionary<string, string> { { $"尊敬的用户{userInfo.UserNickName}", email } }, "PyStudio - 重置密码", tpl);
            data.IsOK = isOk ? 1 : 0;
            data.Msg = isOk ? "已给您邮箱发送了绑定确认邮件，请收件后点击确认绑定链接地址。" : "发送绑定邮件失败，请稍后重试！";

            return data;
        }

        public IActionResult ConfirmPassword(string expire, string token, string email, string t)
        {
            if (string.IsNullOrWhiteSpace(expire) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email) || !email.Contains("@") || string.IsNullOrWhiteSpace(t))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "无效的请求！", Area = "Admin" });
            }
            else if (t.Length != 7)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "无效的请求！", Area = "Admin" });
            }
            email = email.Trim().ToLower();
            if (!DateTime.TryParse(expire, out var expires))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "无效的请求！", Area = "Admin" });
            }
            else if (expires.AddMinutes(10) > DateTime.Now)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "请求已过期，请重新操作！", Area = "Admin" });
            }
            var compareToken = $"{expire}-{email}-{Request.Host.Host}"._Md5();
            if (!token.Equals(compareToken))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "验证失败，无效的请求！", Area = "Admin" });
            }
            var userInfo = _context.InfoUser.SingleOrDefault(b => b.UserEmail.ToLower() == email);
            if (userInfo == null)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "不存在该绑定邮箱的账号！", Area = "Admin" });
            }
            else if (userInfo.UserStatus == (int)EmUserStatus.禁用)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "该账号已被禁用，可以通过发送邮件至：gk1213656215@outlook.com联系客服！", Area = "Admin" });
            }
            var key = $"checkConfirmPwd{email}";
            if (!_cache.TryGetValue<PyUserInfo>(key, out var result))
            {
                _cache.Set<PyUserInfo>(key, new PyUserInfo { UserId = userInfo.UserId, UserEmail = email }, TimeSpan.FromMinutes(10));
            }
            return View(new PyRegisterUser { UserName = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPassword([Bind("UserName", "UserPwd", "ComfirmPwd")]PyRegisterUser registerUser)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(registerUser.UserPwd))
                {
                    this.MsgBox("密码不能为空！");
                    return View(registerUser);
                }
                else if (string.IsNullOrWhiteSpace(registerUser.ComfirmPwd))
                {
                    this.MsgBox("确认密码不能为空！");
                    return View(registerUser);
                }
                else if (registerUser.UserPwd != registerUser.ComfirmPwd)
                {
                    this.MsgBox("密码和确认密码不相同！");
                    return View(registerUser);
                }
                var key = $"checkConfirmPwd{registerUser.UserName}";
                if (!_cache.TryGetValue<PyUserInfo>(key, out var checkUser))
                {
                    return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "请求已过期，重新操作！" });
                }

                var user = _context.InfoUser.Where(b => b.UserId == checkUser.UserId && b.UserEmail == checkUser.UserEmail).SingleOrDefault();
                if (user == null)
                {
                    _cache.Remove(key);
                    return RedirectToAction(nameof(HomeController.Error), "Home", new { msg = "重置密码失败，请稍后重试！" });
                }

                if (user.UserPwd == registerUser.UserPwd.Trim()._Md5())
                {
                    this.MsgBox("新密码与旧密码不能相同！请确认。");
                    return View(registerUser);
                }

                user.UserPwd = registerUser.UserPwd.Trim()._Md5();
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _cache.Remove(key);
                    this.MsgBox("重置密码成功！");
                }
                else
                {
                    this.MsgBox("重置密码失败！");
                }
            }
            return View(registerUser);
        }

        #endregion
    }
}
