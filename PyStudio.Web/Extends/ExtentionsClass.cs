using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static PyStudio.Common.Helper.EnumHelper;

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

        /// <summary>
        /// 给字符串加前缀字符
        /// </summary>
        /// <param name="Num">要加的数量</param>
        /// <param name="first">字符前缀</param>
        /// <param name="supplement">填充字符</param>
        /// <returns>如果为0不返回为空，如果为1以上开始运算</returns>
        public static string CharacterPrefix(this int Num, string first = "├", string supplement = "─")
        {
            string result = string.Empty;
            var ment = string.Empty;
            if (Num > 0)
            {
                if (Num == 1)
                {
                    result = first + supplement + " ";
                }
                else
                {
                    for (int i = 0; i < Num; i++)
                    {
                        ment += supplement;
                    }
                    result = first + ment + " ";
                }
            }
            return result;
        }
        #endregion

        #region 扩展

        /// <summary>
        /// 转换为Int类型
        /// </summary>
        /// <param name="val">要转换的值</param>
        /// <returns>失败返回0</returns>
        public static int ToInt(this string val)
        {
            int number;
            bool isSuccessful = int.TryParse(val, out number);
            return number;
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

        #region Md5加密
        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="input">传入字符</param>
        /// <param name="key">默认字符</param>
        /// <returns></returns>
        public static string _Md5(this string input, string key = "LX.Pystudio")
        {
            var hash = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                hash = GetMd5Hash(md5Hash, input + key);
            }
            return hash;
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString().ToUpper();
        }
        #endregion

        #region 邮箱相关

        /// <summary>
        /// 读取HTML模版
        /// </summary>
        /// <param name="tpl"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static async Task<string> _GetHtmlTpl(EmEmailTpl tpl, string folderPath)
        {
            var content = string.Empty;
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return content;
            }
            var path = $"{folderPath}/{tpl}.html";
            try
            {
                using (var stream = File.OpenRead(path))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        content = await reader.ReadToEndAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return content;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="dicToEmail">收件人and内容</param>
        /// <param name="title">邮件标题</param>
        /// <param name="content">内容</param>
        /// <param name="name">发件人</param>
        /// <param name="fromEmail">来自邮箱</param>
        /// <returns></returns>
        public static bool _SendEmail(
            Dictionary<string, string> dicToEmail,
            string title, string content,
            string name = "PyStudio交易网", string fromEmail = "gk1213656215@qq.com")
        {
            var isOk = false;
            try
            {
                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content)) { return isOk; }

                //设置基本信息
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(name, fromEmail));
                foreach (var item in dicToEmail.Keys)
                {
                    message.To.Add(new MailboxAddress(item, dicToEmail[item]));
                }
                message.Subject = title;
                message.Body = new TextPart("html")
                {
                    Text = content
                };

                //链接发送
                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    //采用qq邮箱服务器发送邮件
                    client.Connect("smtp.qq.com", 587, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    //qq邮箱，密码(安全设置短信获取后的密码)
                    client.Authenticate("gk1213656215@qq.com", "yppnyvzoiyjdhfif");

                    client.Send(message);
                    client.Disconnect(true);
                }
                isOk = true;
            }
            catch (Exception)
            {

            }
            return isOk;
        }

        #endregion
    }
}
