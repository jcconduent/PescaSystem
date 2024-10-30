using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NPOI.XSSF.UserModel;
using PescaSystem.Data;
using PescaSystem.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PescaSystem.Controllers
{
    public class PescaLogController : Controller
    {
        private readonly MongoDbContext _context;

        public PescaLogController()
        {
            _context = new MongoDbContext();
        }

        public IActionResult Index(string? Fecha, string? Yate, string? Capitan)
        {
            TempData["Fecha"] = Fecha;
            TempData["Yate"] = Yate;
            TempData["Capitan"] = Capitan;
            // Inicia la consulta como IQueryable
            IQueryable<PescaLog> query = _context.PescaLogs.AsQueryable();

            // Solo realizar filtrado si se proporcionan parámetros
            if (!string.IsNullOrEmpty(Fecha) || !string.IsNullOrEmpty(Yate) || !string.IsNullOrEmpty(Capitan))
            {
                // Filtrar por Fecha
                if (!string.IsNullOrEmpty(Fecha))
                {
                    if (DateTime.TryParseExact(Fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaFiltrada))
                    {
                        query = query.Where(log => log.Fecha.Date == fechaFiltrada.Date);
                    }
                    else
                    {
                        ModelState.AddModelError("Fecha", "Formato de fecha no válido. Use el formato dd-MM-yyyy.");
                    }
                }

                // Filtrar por Yate
                if (!string.IsNullOrEmpty(Yate))
                {
                    query = query.Where(log => log.Yate == Yate);
                }

                // Filtrar por Capitán
                if (!string.IsNullOrEmpty(Capitan))
                {
                    query = query.Where(log => log.Capitan == Capitan);
                }
            }
            else
            {
                // Si no hay parámetros, puedes devolver una consulta vacía
                query = Enumerable.Empty<PescaLog>().AsQueryable();
            }

            // Devuelve la vista con la lista filtrada o vacía
            return View(query.ToList());
        }

        public IActionResult Create(string Fecha, string Yate, string Capitan)
        {
            TempData["Fecha"] = Fecha ?? DateTime.Now.ToString("yyyy-MM-dd"); // Usa un formato compatible con <input type="date">
            TempData["Yate"] = Yate ?? string.Empty;
            TempData["Capitan"] = Capitan ?? string.Empty;

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
        public IActionResult DownloadExcel(string? Fecha, string? Yate, string? Capitan)
        {
            IQueryable<PescaLog> query = _context.PescaLogs.AsQueryable();

            if (!string.IsNullOrEmpty(Fecha) || !string.IsNullOrEmpty(Yate) || !string.IsNullOrEmpty(Capitan))
            {
                if (!string.IsNullOrEmpty(Fecha) && DateTime.TryParseExact(Fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaFiltrada))
                {
                    query = query.Where(log => log.Fecha.Date == fechaFiltrada.Date);
                }

                if (!string.IsNullOrEmpty(Yate))
                {
                    query = query.Where(log => log.Yate == Yate);
                }

                if (!string.IsNullOrEmpty(Capitan))
                {
                    query = query.Where(log => log.Capitan == Capitan);
                }
            }

            using (var workbook = new XSSFWorkbook())
            {
                var worksheet = workbook.CreateSheet("Registro de Pesca");

                // Crear la fila de encabezado
                var headerRow = worksheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Fecha");
                headerRow.CreateCell(1).SetCellValue("Yate");
                headerRow.CreateCell(2).SetCellValue("Capitán");
                headerRow.CreateCell(3).SetCellValue("Grupo");
                headerRow.CreateCell(4).SetCellValue("Diesel Consumido");
                headerRow.CreateCell(5).SetCellValue("Millas Recorridas");
                headerRow.CreateCell(6).SetCellValue("Vela Flotados");
                headerRow.CreateCell(7).SetCellValue("Vela Piques");
                headerRow.CreateCell(8).SetCellValue("Vela Liberados");
                headerRow.CreateCell(9).SetCellValue("Vela Mosca Flotados");
                headerRow.CreateCell(10).SetCellValue("Vela Mosca Piques");
                headerRow.CreateCell(11).SetCellValue("Vela Mosca Liberados");
                headerRow.CreateCell(12).SetCellValue("Marlin Azul Flotados");
                headerRow.CreateCell(13).SetCellValue("Marlin Azul Piques");
                headerRow.CreateCell(14).SetCellValue("Marlin Azul Liberados");
                headerRow.CreateCell(15).SetCellValue("Marlin Mosca Azul Flotados");
                headerRow.CreateCell(16).SetCellValue("Marlin Mosca Azul Piques");
                headerRow.CreateCell(17).SetCellValue("Marlin Mosca Azul Liberados");
                headerRow.CreateCell(18).SetCellValue("Marlin Rayado Flotados");
                headerRow.CreateCell(19).SetCellValue("Marlin Rayado Piques");
                headerRow.CreateCell(20).SetCellValue("Marlin Rayado Liberados");
                headerRow.CreateCell(21).SetCellValue("Marlin Mosca Rayado Flotados");
                headerRow.CreateCell(22).SetCellValue("Marlin Mosca Rayado Piques");
                headerRow.CreateCell(23).SetCellValue("Marlin Mosca Rayado Liberados");
                headerRow.CreateCell(24).SetCellValue("Marlin Negro Flotados");
                headerRow.CreateCell(25).SetCellValue("Marlin Negro Piques");
                headerRow.CreateCell(26).SetCellValue("Marlin Negro Liberados");
                headerRow.CreateCell(27).SetCellValue("Dorado");
                headerRow.CreateCell(28).SetCellValue("Dorado Fly");
                headerRow.CreateCell(29).SetCellValue("Atún");
                headerRow.CreateCell(30).SetCellValue("Atún Fly");
                headerRow.CreateCell(31).SetCellValue("Gallos");
                headerRow.CreateCell(32).SetCellValue("Wahoo");
                headerRow.CreateCell(33).SetCellValue("Comentarios");

                // Llenar los datos
                int rowIndex = 1;
                foreach (var item in query.ToList())
                {
                    var row = worksheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.Fecha.ToString("dd-MM-yyyy"));
                    row.CreateCell(1).SetCellValue(item.Yate);
                    row.CreateCell(2).SetCellValue(item.Capitan);
                    row.CreateCell(3).SetCellValue(item.Grupo);
                    row.CreateCell(4).SetCellValue(item.DieselConsumido);
                    row.CreateCell(5).SetCellValue(item.MillasRecorridas);
                    row.CreateCell(6).SetCellValue(item.VelaFlotados);
                    row.CreateCell(7).SetCellValue(item.VelaPiques);
                    row.CreateCell(8).SetCellValue(item.VelaLiberados);
                    row.CreateCell(9).SetCellValue(item.VelaMoscaFlotados);
                    row.CreateCell(10).SetCellValue(item.VelaMoscaPiques);
                    row.CreateCell(11).SetCellValue(item.VelaMoscaLiberados);
                    row.CreateCell(12).SetCellValue(item.MarlinAzulFlotados);
                    row.CreateCell(13).SetCellValue(item.MarlinAzulPiques);
                    row.CreateCell(14).SetCellValue(item.MarlinAzulLiberados);
                    row.CreateCell(15).SetCellValue(item.MarlinMoscaAzulFlotados);
                    row.CreateCell(16).SetCellValue(item.MarlinMoscaAzulPiques);
                    row.CreateCell(17).SetCellValue(item.MarlinMoscaAzulLiberados);
                    row.CreateCell(18).SetCellValue(item.MarlinRayadoFlotados);
                    row.CreateCell(19).SetCellValue(item.MarlinRayadoPiques);
                    row.CreateCell(20).SetCellValue(item.MarlinRayadoLiberados);
                    row.CreateCell(21).SetCellValue(item.MarlinMoscaRayadoFlotados);
                    row.CreateCell(22).SetCellValue(item.MarlinMoscaRayadoPiques);
                    row.CreateCell(23).SetCellValue(item.MarlinMoscaRayadoLiberados);
                    row.CreateCell(24).SetCellValue(item.MarlinNegroFlotados);
                    row.CreateCell(25).SetCellValue(item.MarlinNegroPiques);
                    row.CreateCell(26).SetCellValue(item.MarlinNegroLiberados);
                    row.CreateCell(27).SetCellValue(item.Dorado);
                    row.CreateCell(28).SetCellValue(item.DoradoFly);
                    row.CreateCell(29).SetCellValue(item.Atun);
                    row.CreateCell(30).SetCellValue(item.AtunFly);
                    row.CreateCell(31).SetCellValue(item.Gallos);
                    row.CreateCell(32).SetCellValue(item.Wahoo);
                    row.CreateCell(33).SetCellValue(item.Comentario);
                }

                // Escribir el archivo en memoria
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var fileName = "RegistroDePesca.xlsx";
                    return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}
