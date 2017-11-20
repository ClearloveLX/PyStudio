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

        public async Task<IActionResult> MessageBoardIndex()
        {
            ViewBag.Messages = await GetMessageBoardData(0, 30);
            return View();
        }

        public async Task<List<InfoMessageBoard>> GetMessageBoardData(int page, int cont)
        {
            cont = cont == 0 || cont > 30 ? 30 : cont;
            var messages = await _context.InfoMessageBoard.OrderByDescending(e => e.MessageBoardCreateTime).Skip(page).Take(cont).AsNoTracking().ToListAsync();
            return messages;
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
    }
}