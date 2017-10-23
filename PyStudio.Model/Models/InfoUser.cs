using System;
using System.Collections.Generic;

namespace PyStudio.Model.Models
{
    public partial class InfoUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserNickName { get; set; }
        public string UserPwd { get; set; }
        public string UserEmail { get; set; }
        public string UserTel { get; set; }
        public int UserSex { get; set; }
        public string UserIntroduce { get; set; }
        public string UserHeadPhoto { get; set; }
        public DateTime? UserBirthday { get; set; }
        public string UserAddress { get; set; }
        public string UserBlog { get; set; }
        public int UserStatus { get; set; }
        public DateTime UserCreateTime { get; set; }
        public DateTime? UserLoginTime { get; set; }
        public string UserIps { get; set; }
    }
}
