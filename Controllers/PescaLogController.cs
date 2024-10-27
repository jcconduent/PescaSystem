using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using PescaSystem.Data;
using PescaSystem.Models;
using System.Collections.Generic;

namespace PescaSystem.Controllers
{
    public class PescaLogController : Controller
    {
        private readonly MongoDbContext _context;

        public PescaLogController()
        {
            _context = new MongoDbContext();
        }

        public IActionResult Index()
        {
            var logs = _context.PescaLogs.Find(_ => true).ToList();
            return View(logs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PescaLog pescaLog)
        {
            _context.PescaLogs.InsertOne(pescaLog);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            var log = _context.PescaLogs.Find(log => log.Id == ObjectId.Parse(id)).FirstOrDefault();
            return View(log);
        }

        [HttpPost]
        public IActionResult Edit(PescaLog pescaLog)
        {
            _context.PescaLogs.ReplaceOne(log => log.Id == pescaLog.Id, pescaLog);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string id)
        {
            var pescaLog = _context.PescaLogs.Find(log => log.Id == ObjectId.Parse(id)).FirstOrDefault();
            if (pescaLog == null)
            {
                return NotFound();
            }
            return View(pescaLog);
        }

        // POST: PescaLog/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var result = _context.PescaLogs.DeleteOne(log => log.Id == ObjectId.Parse(id));
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
