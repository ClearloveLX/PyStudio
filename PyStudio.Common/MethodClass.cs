using System;
using System.Collections.Generic;
using System.Text;

namespace PyStudio.Common
{
    public class DataClass
    {
        /// <summary>
        /// 获取生成序列
        /// </summary>
        /// <param name="NumCode">传入的值</param>
        /// <param name="DefaultCode">缺省值</param>
        /// <returns></returns>
        public string GetSerialNumber(string NumCode, string DefaultCode = "10000")
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(NumCode))
            {
                result = DefaultCode;
            }
            else
            {
                result = NumCode.ToString();
            }
            return result;
        }

        /// <summary>
        /// 格式化SerialNumber
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public string FormatCode(string Code)
        {
            string a = string.Empty;
            string b = string.Empty;
            for (int i = 0; i < Code.Length; i++)
            {
                try
                {
                    b += Convert.ToInt32(Code.Substring(i, 1));
                }
                catch
                {
                    a += Code.Substring(i, 1);
                }
            }
            string result = a + (Convert.ToInt32(b) + 1).ToString().PadLeft(b.Length, '0');
            return result;
        }
    }
}
