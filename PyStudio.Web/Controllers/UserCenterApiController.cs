using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PyStudio.Model.Models;
using PyStudio.Web.Extends;
using PyStudio.Model.ClientModel;
using System.IO;
using Microsoft.Extensions.Options;
using static PyStudio.Common.Helper.EnumHelper;

namespace PyStudio.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UserCenterApiController : BaseController
    {
        private readonly PyStudioDBContext _context;
        private readonly PySelfSetting _pySelfSetting;

        public UserCenterApiController(PyStudioDBContext context, IOptions<PySelfSetting> pySelfSetting)
        {
            _context = context;
            _pySelfSetting = pySelfSetting.Value;
        }

        [HttpPost]
        public async Task<JsonResult> UpHeadPhoto([Bind("UserId")]PyUserInfo pyUserInfo)
        {
            var data = new PyStudioPromptData();
            var file = Request.Form.Files.Where(b => b.Name == "myHeadPhoto" && b.ContentType.Contains("image")).SingleOrDefault();
            if (file == null)
            {
                data.Msg = "请选择要上传的图片！";
                data.IsOK = 2;
                return Json(data);
            }
            var maxSize = 1024 * 1024 * 4;
            if (file.Length > maxSize)
            {
                data.Msg = "头像大小不能大于4M！";
                data.IsOK = 2;
                return Json(data);
            }
            var fileExtend = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            var fileNewName = $"{DateTime.Now.ToString("yyyyMMddhhmmssfff")}{fileExtend}";
            var path = Path.Combine(_pySelfSetting.UpHeadPhotoPath, fileNewName);
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await file.CopyToAsync(stream);
            }

            var viewPath = $"{_pySelfSetting.ViewHeadPhotoPath}/{fileNewName}";
            var user = _context.InfoUser.Where(b => b.UserId == _MyUserInfo.UserId).SingleOrDefault();

            if (user == null)
            {
                data.Msg = "上传失败，请稍后再试！";
                data.IsOK = 0;
                return Json(data);
            }
            user.UserHeadPhoto = viewPath;
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _MyUserInfo.UserHeadPhoto = viewPath;
                HttpContext.Session.Set<PyUserInfo>(HttpContext.Session.SessionKey(), _MyUserInfo);
                data.Msg = "上传成功！";
                data.IsOK = 1;
                _context.InfoLogger.Add(new InfoLogger
                {
                    LoggerUserId = _MyUserInfo.UserId,
                    LoggerDescription = $"用户{_MyUserInfo.UserName}{EmLogStatus.修改}头像",
                    LoggerOperation = (int)EmLogStatus.修改,
                    LoggerCreateTime = DateTime.Now,
                    LoggerIps = this.GetUserIp()
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                data.Msg = "上传失败，请稍后再试！";
                data.IsOK = 0;
                return Json(data);
            }

            return Json(data);
        }
    }
}