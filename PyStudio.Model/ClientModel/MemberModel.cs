using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PyStudio.Model.ClientModel
{
    /// <summary>
    /// 注册实体
    /// </summary>
    public class PyRegisterUser
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "账号长度范围6-30字符！")]
        [Display(Prompt = "邮箱/手机号/6-30字符")]
        [RegularExpression(@"[^\s]{6,30}", ErrorMessage = "账号长度范围6-30字符。")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "密码长度范围6-20字符！")]
        [DataType(DataType.Password)]
        [Display(Prompt = "密码长度范围6-20字符！")]
        [RegularExpression(@"[^\s]{6,20}", ErrorMessage = "密码长度范围6-20字符。")]
        public string UserPwd { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [Compare("UserPwd", ErrorMessage = "密码与确认密码不相同！")]
        [DataType(DataType.Password)]
        [Display(Prompt = "必须与密码相同")]
        public string ComfirmPwd { get; set; }
    }
}
