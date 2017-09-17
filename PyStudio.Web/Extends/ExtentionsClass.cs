using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyStudio.Web.Extends
{
    public static class ExtentionsClass
    {
        #region 格式化字符串
        /// <summary>
        /// 获取到当前日期的间隔
        /// </summary>
        /// <param name="date">初始日期</param>
        /// <param name="yearNum">每年天数</param>
        /// <param name="monthNum">每月天数</param>
        /// <returns></returns>
        public static string FormatDateToNow(this DateTime date, int yearNum = 365, int monthNum = 31)
        {
            var subTime = DateTime.Now.Subtract(date);
            var dayNum = subTime.Days;

            var year = dayNum / yearNum;
            var month = dayNum % yearNum / monthNum;
            var day = dayNum % yearNum % monthNum;

            var str = year > 0 ? $"{year}年" : "";
            str += month > 0 ? $"{month}月" : "";
            str += day > 0 ? $"{day}天" : "1天";

            return str;
        }

        /// <summary>
        /// 保密电话号
        /// </summary>
        /// <param name="val">初始电话号</param>
        /// <param name="startLen">开头保留长度</param>
        /// <param name="endLen">结尾保留长度</param>
        /// <returns></returns>
        public static string FormatPhone(this string val, int startLen = 3, int endLen = 3)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                return "";
            }

            var len = val.Trim().Length;
            var start = string.Empty;
            var end = string.Empty;

            if (len > startLen)
            {
                start = val.Substring(0, startLen);
            }
            else
            {
                start = val;
            }

            if (len - endLen > startLen)
            {
                end = val.Substring(len - endLen, endLen);
            }

            return string.Format("{0}***{1}", start, end);
        }

        /// <summary>
        /// 保密邮箱地址
        /// </summary>
        /// <param name="val">初始邮箱</param>
        /// <param name="startLen">开头保留长度</param>
        /// <returns>startLen位后至最后一个@前保密</returns>
        public static string FormatEmail(this string val, int startLen = 3)
        {
            var array = val.Split('@');
            int endLen = array[array.Length - 1].Length + 1;
            if (string.IsNullOrWhiteSpace(val))
            {
                return "";
            }

            var len = val.Trim().Length;
            var start = string.Empty;
            var end = string.Empty;

            if (len > startLen)
            {
                start = val.Substring(0, startLen);
            }
            else
            {
                start = val;
            }

            if (len - endLen > startLen)
            {
                end = val.Substring(len - endLen, endLen);
            }

            return string.Format("{0}***{1}", start, end);
        }

        /// <summary>
        /// 省略startLen位后字符
        /// </summary>
        /// <param name="val">初始文字</param>
        /// <param name="startLen">开头保留位数</param>
        /// <param name="op">省略字符</param>
        /// <returns></returns>
        public static string FormatSubStr(this string val, int startLen = 20, string op = "...")
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                return "";
            }
            val = val.Trim();
            if (val.Length < startLen)
            {
                return val;
            }
            else
            {
                val = val.Substring(0, startLen) + op;
            }
            return val;
        }
        #endregion

        #region 扩展

        public static string GetRequestString(this string str) {
            return null;
        }

        #endregion

        #region Controller扩展
        /// <summary>
        /// 提示消息
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="msg">提示内容</param>
        /// <param name="key">ViewData命名</param>
        public static void MsgBox(this Controller controller, string msg, string key = "MsgBox")
        {
            controller.ViewData[key] = msg;
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static string GetUserIp(this Controller controller)
        {
            return controller.HttpContext.Connection.RemoteIpAddress.ToString();
        }
        #endregion

        #region Session 扩展

        /// <summary>
        /// SessionKey生成
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string SessionKey(this ISession session)
        {
            return "MySession";
        }

        /// <summary>
        /// 设置Session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key">Key</param>
        /// <param name="val">内容</param>
        /// <returns></returns>
        public static bool Set<T>(this ISession session, string key, T val)
        {
            if (string.IsNullOrWhiteSpace(key) || val == null)
            {
                return false;
            }

            var strVal = JsonConvert.SerializeObject(val);
            var content = Encoding.UTF8.GetBytes(strVal);
            session.Set(key, content);
            return true;
        }

        /// <summary>
        /// 获取Session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this ISession session, string key)
        {
            var t = default(T);
            if (string.IsNullOrWhiteSpace(key))
            {
                return t;
            }

            if (session.TryGetValue(key, out byte[] val))
            {
                var strVal = Encoding.UTF8.GetString(val);
                t = JsonConvert.DeserializeObject<T>(strVal);
            }
            return t;
        }

        #endregion
    }
}
