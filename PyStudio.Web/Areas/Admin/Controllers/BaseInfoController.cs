using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PyStudio.Model.Models;
using PyStudio.Model.ClientModel;
using PyStudio.Common;
using PyStudio.Web.Extends;
using Microsoft.Extensions.Options;

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseInfoController : Controller
    {
        private readonly PyStudioDBContext _context;
        private readonly PySelfSetting _pySelfSetting;
        private readonly DataClass _dataClass = new DataClass();

        public BaseInfoController(PyStudioDBContext context, IOptions<PySelfSetting> pySelfSetting)
        {
            _context = context;
            _pySelfSetting = pySelfSetting.Value;
        }

        #region 地区/区域

        /// <summary>
        /// 地区列表
        /// </summary>
        /// <returns></returns>
        public IActionResult AreaInfoList()
        {
            var areaInfo = _context.InfoArea.OrderBy(b => b.AreaId).ToList();
            return View(areaInfo);
        }

        /// <summary>
        /// 地区新增GET
        /// </summary>
        /// <returns></returns>
        public IActionResult AreaInfoCreate()
        {
            return View();
        }

        /// <summary>
        /// 地区新增POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AreaInfoCreate([Bind("AreaCode")]AreaInfo _areaInfo)
        {
            if (ModelState.IsValid)
            {
                #region 验证
                if (_context.InfoArea.Any(b => b.AreaName.ToUpper() == _areaInfo.AreaName.Trim().ToUpper()))//验证名称是否重复
                {
                    this.MsgBox("已存在相同的名称！");
                }
                else if (_context.InfoArea.Any(b => b.AreaCode.ToUpper() == _areaInfo.AreaCode.Trim().ToUpper()))//验证编号是否重复
                {
                    this.MsgBox("已存在相同的编号！");
                }
                #endregion

                InfoArea infoArea = new InfoArea
                {
                    AreaId = Convert.ToInt32(_dataClass.FormatCode(_dataClass.GetSerialNumber(_context.InfoArea.Max(m => m.AreaId).ToString()))),
                    AreaCode = _areaInfo.AreaCode.Trim()
                };


            }
            return View();
        }

        #endregion
    }
}