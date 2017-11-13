using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PyStudio.Model.Models;
using PyStudio.Model.ClientModel;
using PyStudio.Web.Extends;
using static PyStudio.Common.Helper.EnumHelper;
using Microsoft.EntityFrameworkCore;
using PyStudio.Model.Models.Account;
using PyStudio.Model.Models.Sys;

namespace PyStudio.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountApiController : Controller
    {
        private readonly PyStudioDBContext _context;

        public AccountApiController(PyStudioDBContext context)
        {
            _context = context;
        }

        #region 账号相关操作 API

        /// <summary>
        /// 注册API
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Register([Bind("UserName,UserPwd,ComfirmPwd")] PyRegisterUser registerUser)
        {
            var data = new PyStudioPromptData();
            if (ModelState.IsValid)
            {
                #region 验证
                if (_context.InfoUser.Any(b => b.UserName.ToUpper().Equals(registerUser.UserName.Trim().ToUpper())))
                {
                    data.IsOK = 2;
                    data.Msg = "已存在相同的账号！";
                    return Json(data);
                }
                #endregion

                InfoUser infoUser = new InfoUser();
                infoUser.UserName = registerUser.UserName.Trim();
                infoUser.UserNickName = registerUser.UserName;
                infoUser.UserPwd = registerUser.UserPwd.Trim()._Md5();
                infoUser.UserSex = 3;
                infoUser.UserHeadPhoto = "/images/default.png";
                infoUser.UserStatus = (int)EmUserStatus.启用;
                infoUser.UserCreateTime = DateTime.Now;
                infoUser.UserIps = this.GetUserIp();

                _context.Add(infoUser);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    data.IsOK = 1;
                    data.Msg = "注册成功";
                    var _pyUserInfo = new PyUserInfo
                    {
                        UserId = infoUser.UserId,
                        UserName = infoUser.UserName,
                        UserNickName = infoUser.UserNickName,
                        UserEmail = infoUser.UserEmail,
                        UserTel = infoUser.UserTel,
                        UserSex = infoUser.UserSex,
                        UserIntroduce = infoUser.UserIntroduce,
                        UserHeadPhoto = infoUser.UserHeadPhoto,
                        UserBirthday = infoUser.UserBirthday,
                        UserAddress = infoUser.UserAddress,
                        UserBlog = infoUser.UserBlog,
                        UserStatus = infoUser.UserStatus,
                        UserCreateTime = infoUser.UserCreateTime,
                        UserLoginTime = infoUser.UserLoginTime,
                        UserIps = infoUser.UserIps
                    };
                    HttpContext.Session.Set<PyUserInfo>(HttpContext.Session.SessionKey(), _pyUserInfo);

                    #region 操作日志记录
                    _context.SysLogger.Add(new SysLogger
                    {
                        LoggerUserId = _pyUserInfo.UserId,
                        LoggerDescription = $"用户{_pyUserInfo.UserName} {EmLogStatus.注册} ",
                        LoggerOperation = (int)EmLogStatus.注册,
                        LoggerCreateTime = DateTime.Now,
                        LoggerIps = _pyUserInfo.UserIps
                    });

                    _context.SysLogger.Add(new SysLogger
                    {
                        LoggerUserId = _pyUserInfo.UserId,
                        LoggerDescription = $"用户{_pyUserInfo.UserName} {EmLogStatus.登录} ",
                        LoggerOperation = (int)EmLogStatus.登录,
                        LoggerCreateTime = DateTime.Now,
                        LoggerIps = _pyUserInfo.UserIps
                    });

                    await _context.SaveChangesAsync();
                    #endregion
                }
                else
                {
                    data.IsOK = 0;
                    data.Msg = "注册失败！请联系客服";
                }
            }
            else
            {
                data.IsOK = 0;
                data.Msg = "注册失败！请联系客服";
            }
            return Json(data);
        }


        /// <summary>
        /// 登录API
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login([Bind("UserName,UserPwd,ReturnUrl")] PyLoginUser loginUser)
        {
            var data = new PyStudioPromptData();

            if (ModelState.IsValid)
            {
                var md5Pwd = loginUser.UserPwd.Trim()._Md5();
                var userInfo = await _context.InfoUser.SingleOrDefaultAsync(b => b.UserName.Equals(loginUser.UserName, StringComparison.CurrentCultureIgnoreCase) && b.UserPwd.Equals(md5Pwd));

                if (userInfo == null)
                {
                    data.IsOK = 2;
                    data.Msg = "用户名或密码错误！";
                    return Json(data);
                }
                else if (userInfo.UserStatus == (int)EmUserStatus.禁用)
                {
                    data.IsOK = 2;
                    data.Msg = "该账号已被禁用，或许你可以重新注册一个账号或者联系管理员！";
                    return Json(data);
                }

                userInfo.UserIps = this.GetUserIp();
                userInfo.UserLoginTime = DateTime.Now;

                var _pyUserInfo = new PyUserInfo
                {
                    UserId = userInfo.UserId,
                    UserName = userInfo.UserName,
                    UserNickName = userInfo.UserNickName,
                    UserEmail = userInfo.UserEmail,
                    UserTel = userInfo.UserTel,
                    UserSex = userInfo.UserSex,
                    UserIntroduce = userInfo.UserIntroduce,
                    UserHeadPhoto = userInfo.UserHeadPhoto,
                    UserBirthday = userInfo.UserBirthday,
                    UserAddress = userInfo.UserAddress,
                    UserBlog = userInfo.UserBlog,
                    UserStatus = userInfo.UserStatus,
                    UserCreateTime = userInfo.UserCreateTime,
                    UserLoginTime = userInfo.UserLoginTime,
                    UserIps = userInfo.UserIps
                };
                HttpContext.Session.Set<PyUserInfo>(HttpContext.Session.SessionKey(), _pyUserInfo);

                _context.SysLogger.Add(new SysLogger
                {
                    LoggerUserId = _pyUserInfo.UserId,
                    LoggerDescription = $"用户{_pyUserInfo.UserName}{EmLogStatus.登录} ",
                    LoggerOperation = (int)EmLogStatus.登录,
                    LoggerCreateTime = DateTime.Now,
                    LoggerIps = _pyUserInfo.UserIps
                });

                await _context.SaveChangesAsync();

                data.IsOK = 1;
                data.Msg = "登录成功";
            }
            else
            {
                data.IsOK = 0;
                data.Msg = "登录失败！";
            }
            return Json(data);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetLoginInfo()
        {
            var data = new PyStudioPromptData();
            var userInfo = HttpContext.Session.Get<PyUserInfo>(HttpContext.Session.SessionKey());

            if (userInfo != null)
            {
                data.Data = userInfo;
                data.IsOK = 1;
            }
            if (data.IsOK.Equals(1))
            {
                data.Msg = "已登录";
            }
            else
            {
                data.Msg = "未登录";
            }
            return Json(data);
        }



        #endregion
    }
}