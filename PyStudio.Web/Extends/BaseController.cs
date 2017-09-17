using Microsoft.AspNetCore.Mvc;
using PyStudio.Model.ClientModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PyStudio.Web.Extends
{
    public class BaseController : Controller
    {
        public PyUserInfo _MyUserInfo;

        /// <summary>
        /// 判断是否登陆
        /// </summary>
        /// <param name="context">未登陆返回登陆页面</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _MyUserInfo = context.HttpContext.Session.Get<PyUserInfo>(context.HttpContext.Session.SessionKey());
            if (_MyUserInfo == null)
            {
                //未登录跳转地址
            }
            ViewData["MyUserInfo"] = _MyUserInfo;
            base.OnActionExecuting(context);
        }
    }
}
