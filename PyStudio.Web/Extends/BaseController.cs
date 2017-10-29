using Microsoft.AspNetCore.Mvc;
using PyStudio.Model.ClientModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Web;

namespace PyStudio.Web.Extends
{
    public class BaseController : Controller
    {
        public PyUserInfo _MyUserInfo;

        /// <summary>
        /// 判断是否登陆
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _MyUserInfo = context.HttpContext.Session.Get<PyUserInfo>(context.HttpContext.Session.SessionKey());
            if (_MyUserInfo == null)
            {
                context.Result = Redirect("/Admin/Account/Login?ReturnUrl=" + HttpUtility.UrlEncode(context.HttpContext.Request.Path));
            }
            ViewData["MyUserInfo"] = _MyUserInfo;
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 获取到QueryString
        /// </summary>
        /// <param name="key">参数的Key</param>
        /// <returns>参数的值</returns>
        public string GetQueryString(string key)
        {
            string _queryString = Request.Query[key].ToString();
            string result = string.Empty;

            try
            {
                if (!string.IsNullOrWhiteSpace(_queryString))
                {
                    result = _queryString;
                }
                else
                {
                    result = "";
                }
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }
    }
}
