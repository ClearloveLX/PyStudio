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

namespace PyStudio.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MemberApiController : Controller
    {
        private readonly PyStudioDBContext _context;

        public MemberApiController(PyStudioDBContext context)
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

        #endregion
    }
}