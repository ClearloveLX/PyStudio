using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Caching.Memory;
using PyStudio.Model.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.Extensions.Options;
using PyStudio.Model.ClientModel;
using PyStudio.Model.Repositories;

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CacheController : Controller
    {
        private IMemoryCache _cache;
        private readonly PyStudioDBContext _context;
        private IHostingEnvironment _hostingEnvironment;
        private readonly PySelfSetting _pySelfSetting;

        public CacheController(PyStudioDBContext context, IMemoryCache memoryCache, IHostingEnvironment hostingEnvironment, IOptions<PySelfSetting> pySelfSetting)
        {
            _cache = memoryCache;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _pySelfSetting = pySelfSetting.Value;
        }

        public IActionResult Index()
        {
            return RedirectToAction("CacheGet");
        }

        //#region snippet1
        //public IActionResult CacheTryGetValueSet()
        //{
        //    DateTime cacheEntry;

        //    // Look for cache key.
        //    if (!_cache.TryGetValue(CacheKeys.Entry, out cacheEntry))
        //    {
        //        // Key not in cache, so get data.
        //        cacheEntry = DateTime.Now;

        //        // Set cache options.
        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //            // Keep in cache for this time, reset time if accessed.
        //            .SetSlidingExpiration(TimeSpan.FromSeconds(3));

        //        // Save data in cache.
        //        _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
        //    }

        //    return View("Cache", cacheEntry);
        //}
        //#endregion

        //#region snippet_gct
        //public IActionResult CacheGet()
        //{
        //    var cacheEntry = _cache.Get<DateTime?>(CacheKeys.Entry);
        //    return View("Cache", cacheEntry);
        //}
        //#endregion

        //#region snippet2
        //public IActionResult CacheGetOrCreate()
        //{
        //    var cacheEntry = _cache.GetOrCreate(CacheKeys.Entry, entry =>
        //    {
        //        entry.SlidingExpiration = TimeSpan.FromSeconds(3);
        //        return DateTime.Now;
        //    });

        //    return View("Cache", cacheEntry);
        //}

        //public async Task<IActionResult> CacheGetOrCreateAsync()
        //{
        //    var cacheEntry = await
        //        _cache.GetOrCreateAsync(CacheKeys.Entry, entry =>
        //        {
        //            entry.SlidingExpiration = TimeSpan.FromSeconds(3);
        //            return Task.FromResult(DateTime.Now);
        //        });

        //    return View("Cache", cacheEntry);
        //}
        //#endregion

        //public IActionResult CacheRemove()
        //{
        //    _cache.Remove(CacheKeys.Entry);
        //    return RedirectToAction("CacheGet");
        //}

        //#region snippet_et
        //public IActionResult CreateCallbackEntry()
        //{
        //    var cacheEntryOptions = new MemoryCacheEntryOptions()
        //        // Pin to cache.
        //        .SetPriority(CacheItemPriority.NeverRemove)
        //        // Add eviction callback
        //        .RegisterPostEvictionCallback(callback: EvictionCallback, state: this);

        //    _cache.Set(CacheKeys.CallbackEntry, DateTime.Now, cacheEntryOptions);

        //    return RedirectToAction("GetCallbackEntry");
        //}

        //public IActionResult GetCallbackEntry()
        //{
        //    return View("Callback", new CallbackViewModel
        //    {
        //        CachedTime = _cache.Get<DateTime?>(CacheKeys.CallbackEntry),
        //        Message = _cache.Get<string>(CacheKeys.CallbackMessage)
        //    });
        //}

        //public IActionResult RemoveCallbackEntry()
        //{
        //    _cache.Remove(CacheKeys.CallbackEntry);
        //    return RedirectToAction("GetCallbackEntry");
        //}

        //private static void EvictionCallback(object key, object value,
        //    EvictionReason reason, object state)
        //{
        //    var message = $"Entry was evicted. Reason: {reason}.";
        //    ((CacheController)state)._cache.Set(CacheKeys.CallbackMessage, message);
        //}
        //#endregion

        //#region snippet_ed
        //public IActionResult CreateDependentEntries()
        //{
        //    var cts = new CancellationTokenSource();
        //    _cache.Set(CacheKeys.DependentCTS, cts);

        //    using (var entry = _cache.CreateEntry(CacheKeys.Parent))
        //    {
        //        // expire this entry if the dependant entry expires.
        //        entry.Value = DateTime.Now;
        //        entry.RegisterPostEvictionCallback(DependentEvictionCallback, this);

        //        _cache.Set(CacheKeys.Child,
        //            DateTime.Now,
        //            new CancellationChangeToken(cts.Token));
        //    }

        //    return RedirectToAction("GetDependentEntries");
        //}

        //public IActionResult GetDependentEntries()
        //{
        //    return View("Dependent", new DependentViewModel
        //    {
        //        ParentCachedTime = _cache.Get<DateTime?>(CacheKeys.Parent),
        //        ChildCachedTime = _cache.Get<DateTime?>(CacheKeys.Child),
        //        Message = _cache.Get<string>(CacheKeys.DependentMessage)
        //    });
        //}

        //public IActionResult RemoveChildEntry()
        //{
        //    _cache.Get<CancellationTokenSource>(CacheKeys.DependentCTS).Cancel();
        //    return RedirectToAction("GetDependentEntries");
        //}

        //private static void DependentEvictionCallback(object key, object value,
        //    EvictionReason reason, object state)
        //{
        //    var message = $"Parent entry was evicted. Reason: {reason}.";
        //    ((CacheController)state)._cache.Set(CacheKeys.DependentMessage, message);
        //}
        //#endregion

        //#region snippet_cancel
        //public IActionResult CancelTest()
        //{
        //    var cachedVal = DateTime.Now.Second.ToString();
        //    CancellationTokenSource cts = new CancellationTokenSource();
        //    _cache.Set<CancellationTokenSource>(CacheKeys.CancelTokenSource, cts);

        //    // Don't use previous message.
        //    _cache.Remove(CacheKeys.CancelMsg);

        //    _cache.Set(CacheKeys.Ticks, cachedVal,
        //        new MemoryCacheEntryOptions()
        //        .AddExpirationToken(new CancellationChangeToken(cts.Token))
        //        .RegisterPostEvictionCallback(
        //            (key, value, reason, substate) =>
        //            {
        //                var cm = $"'{key}':'{value}' was evicted because: {reason}";
        //                _cache.Set<string>(CacheKeys.CancelMsg, cm);
        //            }
        //        ));

        //    return RedirectToAction("CheckCancel");
        //}

        //public IActionResult CheckCancel(int? id = 0)
        //{
        //    if (id > 0)
        //    {
        //        CancellationTokenSource cts =
        //           _cache.Get<CancellationTokenSource>(CacheKeys.CancelTokenSource);
        //        cts.CancelAfter(100);
        //        // Cancel immediately with cts.Cancel();
        //    }

        //    ViewData["CachedTime"] = _cache.Get<string>(CacheKeys.Ticks);
        //    ViewData["Message"] = _cache.Get<string>(CacheKeys.CancelMsg); ;

        //    return View();
        //}
        //#endregion

        //public IActionResult PyTestCache(string SStr)
        //{
        //    List<InfoLogger> ilList = new List<InfoLogger>();
        //    if (!_cache.TryGetValue(CacheKeys.Entry, out ilList))
        //    {
        //        ilList = _context.InfoLogger.ToList();
        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //                                .SetSlidingExpiration(TimeSpan.FromSeconds(30));
        //        _cache.Set(CacheKeys.Entry, ilList, cacheEntryOptions);
        //    }
        //    if (!string.IsNullOrWhiteSpace(SStr))
        //    {
        //        ilList = ilList.Where(m => m.LoggerDescription.Contains(SStr)).ToList();
        //    }
        //    return View(ilList);
        //}

        public IActionResult Export()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("aspnetcore");
                //添加头
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Url";
                //添加值
                worksheet.Cells["A2"].Value = 1000;
                worksheet.Cells["B2"].Value = "LineZero";
                worksheet.Cells["C2"].Value = "http://www.cnblogs.com/linezero/";

                worksheet.Cells["A3"].Value = 1001;
                worksheet.Cells["B3"].Value = "LineZero GitHub";
                worksheet.Cells["C3"].Value = "https://github.com/linezero";
                worksheet.Cells["C3"].Style.Font.Bold = true;

                package.Save();
            }
            return File(sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public IActionResult Import()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Import(IFormFile excelfile)
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

            using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
            {
                excelfile.CopyTo(fs);
                fs.Flush();
            }
            //using (ExcelPackage package = new ExcelPackage(file))
            //{
            //    StringBuilder sb = new StringBuilder();
            //    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            //    int rowCount = worksheet.Dimension.Rows;
            //    int colCount = worksheet.Dimension.Columns;
            //    var test = new Test();
            //    List<Test> list = new List<Test>();
            //    for (int row = 1; row < rowCount; row++)
            //    {
            //        //test.Id = worksheet.Cells[row + 1, 1].Value.ToString();
            //        //test.Name = worksheet.Cells[row + 1, 2].Value.ToString();
            //        //test.Url = worksheet.Cells[row + 1, 3].Value.ToString();
            //        list.Add(new Test { Id = worksheet.Cells[row + 1, 1].Value.ToString(), Name = worksheet.Cells[row + 1, 2].Value.ToString(), Url = worksheet.Cells[row + 1, 3].Value.ToString() });
            //    }
            //    PySqlHelper.BulkInsert<Test>(_pySelfSetting.DbLink, "Test", list);
            return Content("");
            //}

        }
    }
}