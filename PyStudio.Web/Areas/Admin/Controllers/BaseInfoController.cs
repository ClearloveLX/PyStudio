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
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseInfoController : BaseController
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
            var _listItem = _context.InfoArea.OrderBy(b => b.AreaCode).ToList();
            return View(_listItem);
        }

        /// <summary>
        /// 地区新增GET
        /// </summary>
        /// <returns></returns>
        public IActionResult AreaInfoCreate(string tag, int level, string id)
        {

            AreaInfo _areaInfo = new AreaInfo();

            if (!string.IsNullOrWhiteSpace(tag))
            {
                //add=新增第一级，low新增下级
                if (tag.Equals("add"))
                {
                    //查询上级信息
                    var result = (from t1 in _context.InfoArea
                                  where t1.AreaId == id.ToInt()
                                  join t2 in _context.InfoArea
                                  on t1.AreaPid equals t2.AreaId.ToString()
                                  select new { UpName = t2.AreaName, UpPathId = t2.AreaPathId, ThisPid = t1.AreaPid }).FirstOrDefault();
                    if (result != null) //查询出结果赋值
                    {
                        _areaInfo.UpAreaName = result.UpName;
                        _areaInfo.UpAreaPathId = result.UpPathId;
                        _areaInfo.AreaPid = result.ThisPid;
                        _areaInfo.AreaCode = _dataClass.FormatCode(
                                    _dataClass.GetSerialNumber(
                                    _context.InfoArea
                                    .Where(n => n.AreaLevel == level && n.AreaPid == result.ThisPid)
                                    .Max(m => m.AreaCode
                                    ), DefaultCode: "00"));
                    }
                    else if (level.Equals(0))//未查询出结果单Level=0
                    {
                        _areaInfo.AreaCode = _dataClass.FormatCode(
                                    _dataClass.GetSerialNumber(
                                    _context.InfoArea
                                    .Where(n => n.AreaLevel == level)
                                    .Max(m => m.AreaCode
                                    ), DefaultCode: "00"));
                    }
                }
                else if (tag.Equals("low"))//下级
                {
                    var list = _context.InfoArea.Where(m => m.AreaId.Equals(id.ToInt())).ToList();
                    string upcode = string.Empty;

                    if (list.Count > 0)
                    {
                        _areaInfo.UpAreaName = list[0].AreaName;
                        _areaInfo.UpAreaPathId = list[0].AreaPathId;
                        upcode = list[0].AreaCode;
                    }

                    _areaInfo.AreaCode = _dataClass.FormatCode(
                                    _dataClass.GetSerialNumber(
                                    _context.InfoArea
                                    .Where(n => n.AreaLevel == level && n.AreaPid == id)
                                    .Max(m => m.AreaCode
                                    ), DefaultCode: upcode + "00"));
                    _areaInfo.AreaPid = id;
                }
                _areaInfo.AreaLevel = level;
            }

            return View(_areaInfo);
        }

        /// <summary>
        /// 地区新增POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AreaInfoCreate([Bind("AreaCode,AreaName,AreaPid,AreaLevel,AreaCoord,AreaZipCode,AreaNote,UpAreaName,UpAreaPathId")]AreaInfo _areaInfo)
        {
            if (ModelState.IsValid)
            {
                #region 验证是否存在重复
                if (_context.InfoArea.Any(b => b.AreaName.ToUpper() == _areaInfo.AreaName.Trim().ToUpper() && b.AreaLevel.Equals(_areaInfo.AreaLevel)))//验证名称是否重复
                {
                    this.MsgBox("已存在相同的名称！");
                    return View(_areaInfo);
                }
                else if (_context.InfoArea.Any(b => b.AreaCode.ToUpper() == _areaInfo.AreaCode.Trim().ToUpper()))//验证编号是否重复
                {
                    this.MsgBox("已存在相同的编号！");
                    return View(_areaInfo);
                }
                #endregion
                var a1 = _context.InfoArea.Select(s => s.AreaId.ToString()).Max() ?? "";
                var b2 = _dataClass.GetSerialNumber(a1.ToString());
                var c3 = _dataClass.FormatCode(b2).ToInt();
                _areaInfo.AreaId = c3;
                if (_areaInfo.AreaLevel > 0)
                {
                    _areaInfo.AreaPathId = _areaInfo.UpAreaPathId + "" + _areaInfo.AreaId + "|";
                }
                else if (_areaInfo.AreaLevel == 0)
                {
                    _areaInfo.AreaPathId = "|" + _areaInfo.AreaId + "|";
                }

                InfoArea infoArea = new InfoArea
                {
                    AreaId = _areaInfo.AreaId,
                    AreaCode = _areaInfo.AreaCode.Trim(),
                    AreaName = _areaInfo.AreaName.Trim(),
                    AreaPathId = _areaInfo.AreaPathId,
                    AreaPid = _areaInfo.AreaPid,
                    AreaLevel = _areaInfo.AreaLevel,
                    AreaCoord = _areaInfo.AreaCoord,
                    AreaZipCode = _areaInfo.AreaZipCode,
                    AreaNote = _areaInfo.AreaNote
                };
                _context.Add(infoArea);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    this.MsgBox("提交成功！");
                    Response.Redirect($"AreaInfoCreate?tag=add&level={_areaInfo.AreaLevel}&id={_areaInfo.AreaId}");
                }
                else
                {
                    this.MsgBox("提交失败！请稍后再试...");
                    return View(_areaInfo);
                }
            }
            return View(_areaInfo);




        }

        /// <summary>
        /// 地区修改GET
        /// </summary>
        /// <param name="id">地区ID</param>
        /// <returns></returns>
        public IActionResult AreaInfoModify(int id)
        {
            AreaInfo _areaInfo = new AreaInfo();
            var result = (from t1 in _context.InfoArea
                          where t1.AreaId.Equals(id)
                          join t2 in _context.InfoArea
                          on t1.AreaPid equals t2.AreaId.ToString() into temp
                          from t2 in temp.DefaultIfEmpty()
                          select new
                          {
                              UpName = t2.AreaName,
                              UpPathId = t2.AreaPathId,
                              ThisId = t1.AreaId,
                              ThisCode = t1.AreaCode,
                              ThisName = t1.AreaName,
                              ThisPathId = t1.AreaPathId,
                              ThisCoord = t1.AreaCoord,
                              ThisZipCode = t1.AreaZipCode,
                              ThisNote = t1.AreaNote
                          }).FirstOrDefault();

            if (result != null)
            {
                _areaInfo.UpAreaName = result.UpName;
                _areaInfo.UpAreaPathId = result.UpPathId;
                _areaInfo.AreaId = result.ThisId;
                _areaInfo.AreaCode = result.ThisCode;
                _areaInfo.AreaName = result.ThisName;
                _areaInfo.AreaPathId = result.ThisPathId;
                _areaInfo.AreaCoord = result.ThisCoord;
                _areaInfo.AreaZipCode = result.ThisZipCode;
                _areaInfo.AreaNote = result.ThisNote;
            }
            return View(_areaInfo);
        }

        /// <summary>
        /// 地区修改POST
        /// </summary>
        /// <param name="_areaInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AreaInfoModify([Bind("AreaId,AreaPathId,AreaName,AreaCoord,AreaZipCode,AreaNote,UpAreaName,UpAreaPathId")]AreaInfo _areaInfo)
        {
            var data = new PyStudioPromptData();
            if (ModelState.IsValid)
            {
                var infoArea = _context.InfoArea.Where(b => b.AreaId.Equals(_areaInfo.AreaId) && b.AreaPathId.Equals(_areaInfo.AreaPathId)).FirstOrDefault();
                if (infoArea != null)
                {
                    infoArea.AreaName = _areaInfo.AreaName;
                    infoArea.AreaCoord = _areaInfo.AreaCoord;
                    infoArea.AreaZipCode = _areaInfo.AreaZipCode;
                    infoArea.AreaNote = _areaInfo.AreaNote;
                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        data.IsOK = 1;
                        data.Msg = "修改成功";
                    }
                    else
                    {
                        data.IsOK = 0;
                        data.Msg = "修改失败！请稍后再试...";
                    }
                }
                else
                {
                    data.IsOK = 0;
                    data.Msg = "修改失败！请稍后再试...";
                }
            }
            return Json(data);
        }

        /// <summary>
        /// 地区删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AreaInfoDelete(int id)
        {
            var pathId = (from t1 in _context.InfoArea where t1.AreaId.Equals(id) select new { ThisPathId = t1.AreaPathId }).FirstOrDefault();
            var infoArea = _context.InfoArea.Where(t => t.AreaPathId.StartsWith(pathId.ThisPathId)).ToList();

            var data = new PyStudioPromptData();
            if (infoArea != null)
            {
                foreach (var item in infoArea)
                {
                    _context.InfoArea.Remove(item);
                }
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    data.IsOK = 1;
                    data.Msg = $"删除成功,共删除{result}条数据！";
                }
                else
                {
                    data.IsOK = 0;
                    data.Msg = "删除失败！";
                }
            }
            else
            {
                data.IsOK = 2;
                data.Msg = "未找到数据！";
            }
            return Json(data);
        }

        public IActionResult AreaInfoDetails(int id)
        {
            AreaInfo _areaInfo = new AreaInfo();
            var result = (from t1 in _context.InfoArea
                          where t1.AreaId.Equals(id)
                          join t2 in _context.InfoArea
                          on t1.AreaPid equals t2.AreaId.ToString() into temp
                          from t2 in temp.DefaultIfEmpty()
                          select new
                          {
                              UpName = t2.AreaName,
                              UpPathId = t2.AreaPathId,
                              ThisId = t1.AreaId,
                              ThisCode = t1.AreaCode,
                              ThisName = t1.AreaName,
                              ThisPathId = t1.AreaPathId,
                              ThisCoord = t1.AreaCoord,
                              ThisZipCode = t1.AreaZipCode,
                              ThisNote = t1.AreaNote
                          }).FirstOrDefault();

            if (result != null)
            {
                _areaInfo.UpAreaName = result.UpName;
                _areaInfo.UpAreaPathId = result.UpPathId;
                _areaInfo.AreaId = result.ThisId;
                _areaInfo.AreaCode = result.ThisCode;
                _areaInfo.AreaName = result.ThisName;
                _areaInfo.AreaPathId = result.ThisPathId;
                _areaInfo.AreaCoord = result.ThisCoord;
                _areaInfo.AreaZipCode = result.ThisZipCode;
                _areaInfo.AreaNote = result.ThisNote;
            }
            return View(_areaInfo);
        }
        #endregion
    }
}