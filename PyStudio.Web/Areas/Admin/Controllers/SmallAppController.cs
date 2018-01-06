using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PyStudio.Model.Models;
using PyStudio.Model.Models.SmallApp;
using PyStudio.Model.Repositories;
using Microsoft.EntityFrameworkCore;
using PyStudio.Web.Extends;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace PyStudio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SmallAppController : Controller
    {
        private readonly PyStudioDBContext _context;
        private readonly IRepository<InfoMessageBoard> _repository;
        public SmallAppController(PyStudioDBContext context)
        {
            _context = context;
            _repository = new Repository<InfoMessageBoard>(_context);
        }

        #region 留言板

        public async Task<IActionResult> MessageBoardIndex()
        {
            ViewBag.Messages = await GetMessageBoardData<InfoMessageBoard>();
            return View();
        }

        public async Task<List<InfoMessageBoard>> GetData(int page = 0, int cont = 30)
        {
            return await GetMessageBoardData<InfoMessageBoard>(page, cont);
        }

        public async Task<List<T>> GetMessageBoardData<T>(int page = 0, int cont = 30
                                                          , Expression<Func<T, bool>> filter = null
                                                          , Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null
                                                          , string includeProperties = "") where T : class
        {
            cont = cont == 0 || cont > 30 ? 30 : cont;
            //
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).Skip(page).Take(cont).ToListAsync();
            }
            else
            {
                return await query.Skip(page).Take(cont).ToListAsync();
            }
        }

        [HttpPost]
        public async Task<string> RecordMessageBoard(string msg, string userName, string token)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                var ip = this.GetUserIp();
                _context.InfoMessageBoard.Add(new InfoMessageBoard()
                {
                    MessageBoardCreateTime = DateTime.Now,
                    MessageBoardIp = ip,
                    MessageBoardUser = userName ?? "蒙面人",
                    MessageBoardContent = msg
                });
                await _context.SaveChangesAsync();
            }
            return "提交成功";
        }

        #endregion

        public IActionResult ChatRoom()
        {
            ViewBag.HistoricalMessg = JsonConvert.SerializeObject(SocketHandler.historicalMessg);
            return View();
        }
    }
}