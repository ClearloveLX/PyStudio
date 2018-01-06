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
using PyStudio.Model.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;
using PyStudio.Model.Models.BaseInfo;
using PyStudio.Model.Models.Sys;
using static PyStudio.Common.Helper.EnumHelper;

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseInfoController : BaseController
    {
        private readonly PyStudioDBContext _context;
        private readonly PySelfSetting _pySelfSetting;
        private readonly IMemoryCache _cache;
        private readonly DataClass _dataClass = new DataClass();
        private readonly IRepository<InfoArea> _repository;
        private readonly IRepository<InfoEi> _repositoryEi;
        private readonly IHostingEnvironment _hostingEnvironment;
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public BaseInfoController(PyStudioDBContext context, IOptions<PySelfSetting> pySelfSetting, IMemoryCache cache, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _pySelfSetting = pySelfSetting.Value;
            _cache = cache;
            _repository = new Repository<InfoArea>(_context);
            _repositoryEi = new Repository<InfoEi>(_context);
            _hostingEnvironment = hostingEnvironment;
        }

        #region 地区/区域

        /// <summary>
        /// 地区列表
        /// </summary>
        /// <returns></returns>
        public IActionResult AreaInfoList(string condition)
        {
            string key = "_GetAreaInfo";
            List<InfoArea> objModel = new List<InfoArea>();
            if (!_cache.TryGetValue(key, out objModel))//缓存三十秒，防止频繁刷新
            {
                objModel = _repository.GetList(orderBy: s => s.OrderBy(c => c.AreaCode)).ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromSeconds(30));
                _cache.Set(key, objModel, cacheEntryOptions);
            }

            if (!string.IsNullOrWhiteSpace(condition))
            {
                objModel = objModel.Where(m => m.AreaName.Contains(condition)).ToList();
            }

            return View(objModel);
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
                    _context.SysLogger.Add(new SysLogger
                    {
                        LoggerCreateTime = DateTime.Now,
                        LoggerDescription = $"{EmLogStatus.增加}地区{infoArea.AreaName}信息",
                        LoggerIps = _MyUserInfo.UserIps,
                        LoggerOperation = (int)EmLogStatus.增加,
                        LoggerUser = _MyUserInfo.UserId
                    });
                    await _context.SaveChangesAsync();
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
                        _context.SysLogger.Add(new SysLogger
                        {
                            LoggerCreateTime = DateTime.Now,
                            LoggerDescription = $"{EmLogStatus.修改}地区{infoArea.AreaName}信息",
                            LoggerIps = _MyUserInfo.UserIps,
                            LoggerOperation = (int)EmLogStatus.修改,
                            LoggerUser = _MyUserInfo.UserId
                        });
                        await _context.SaveChangesAsync();
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
                    string resultName = string.Empty;
                    for (int i = 0; i < infoArea.Count; i++)
                    {
                        resultName += infoArea[i].AreaName + ",";
                    }
                    _context.SysLogger.Add(new SysLogger
                    {
                        LoggerCreateTime = DateTime.Now,
                        LoggerDescription = $"{EmLogStatus.删除}地区{resultName.TrimEnd(',')}",
                        LoggerIps = _MyUserInfo.UserIps,
                        LoggerOperation = (int)EmLogStatus.删除,
                        LoggerUser = _MyUserInfo.UserId
                    });
                    await _context.SaveChangesAsync();
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

        /// <summary>
        /// 地区查看
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        #region 导入导出

        public IActionResult ImportAndExportIndex()
        {
            return View(_repositoryEi.GetList());
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public IActionResult Export()
        {
            string filePath = $"{_hostingEnvironment.WebRootPath}/{_pySelfSetting.FileExcelExportPath}";
            string fileName = $"{Guid.NewGuid()}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(filePath, fileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //添加工作表
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PyStudio导入模版");
                //标题
                worksheet.Cells[1, 1].Value = "Col1";
                worksheet.Cells[1, 2].Value = "Col2";
                worksheet.Cells[1, 3].Value = "Col3";
                worksheet.Cells[1, 4].Value = "Col4";
                //添加内容
                worksheet.Cells["A2"].Value = 315979355;
                worksheet.Cells["B2"].Value = "暴龙";
                worksheet.Cells["C2"].Value = "暴龙";
                worksheet.Cells["D2"].Value = "315979355@qq.com";
                worksheet.Cells["D2"].Style.Font.Bold = true;

                worksheet.Cells["A3"].Value = 274351622;
                worksheet.Cells["B3"].Value = "Sinory";
                worksheet.Cells["C3"].Value = "猩猩";
                worksheet.Cells["D3"].Value = "kjasw2000@tom.com";
                worksheet.Cells["D3"].Style.Font.Bold = true;

                worksheet.Cells["A4"].Value = 444902302;
                worksheet.Cells["B4"].Value = "Bj";
                worksheet.Cells["C4"].Value = "奶皮";
                worksheet.Cells["D4"].Value = "None'";
                worksheet.Cells["D4"].Style.Font.Bold = true;

                package.Save();
            }
            return File($"{_pySelfSetting.FileExcelExportPath}/{fileName}", XlsxContentType);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="excelFile"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Import(IFormFile excelFile)
        {
            if (excelFile == null)
            {
                return View("ImportAndExportIndex", _repositoryEi.GetList());
            }
            string filePath = $"{_hostingEnvironment.WebRootPath}/{_pySelfSetting.FileExcelImportPath}";
            string fileName = $"{Guid.NewGuid()}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(filePath, fileName));
            using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
            {
                excelFile.CopyTo(fs);
                fs.Flush();
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                if (rowCount > 0)
                {
                    List<InfoEi> list = new List<InfoEi>();
                    for (int i = 1; i < rowCount; i++)
                    {
                        list.Add(new InfoEi
                        {
                            Eicol1 = worksheet.Cells[i + 1, 1].Value.ToString(),
                            Eicol2 = worksheet.Cells[i + 1, 2].Value.ToString(),
                            Eicol3 = worksheet.Cells[i + 1, 3].Value.ToString(),
                            Eicol4 = worksheet.Cells[i + 1, 4].Value.ToString(),
                        });
                    }
                    PySqlHelper.BulkInsert(_pySelfSetting.DbLink, "InfoEI", list);
                }
            }
            return RedirectToAction("ImportAndExportIndex");
        }

        #endregion
    }
}