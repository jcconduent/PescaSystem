﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NPOI.XSSF.UserModel;
using PescaSystem.Data;
using PescaSystem.Models;
using NPOI.SS.UserModel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Drawing;
using Shape = DocumentFormat.OpenXml.Presentation.Shape;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using DocumentFormat.OpenXml;
using SharpCompress.Common;
using System.Text;
using NPOI.SS.Formula.Functions;
using DocumentFormat.OpenXml.Office2010.Excel;
using Path = System.IO.Path;
using NPOI.XWPF.UserModel;

namespace PescaSystem.Controllers
{
    public class PescaLogController : Controller
    {
        private readonly MongoDbContext _context;

        public PescaLogController()
        {
            _context = new MongoDbContext();
        }

        private IQueryable<PescaLog> BuscarRegistros(string? Fecha, string? FechaHasta, string? Yate, string? Capitan, string? Grupo)
        {
            IQueryable<PescaLog> query = _context.PescaLogs.AsQueryable();
            if (!string.IsNullOrEmpty(Fecha) || !string.IsNullOrEmpty(Yate) || !string.IsNullOrEmpty(Capitan) || !string.IsNullOrEmpty(Grupo))
            {
                if (!string.IsNullOrEmpty(Fecha))
                {
                    if (!string.IsNullOrEmpty(FechaHasta))
                    {
                        DateTime.TryParse(Fecha, out DateTime FechaDT);
                        DateTime.TryParse(FechaHasta, out DateTime FechaHastaDT);
                        var logsLista = query.ToList();
                        var logsFiltrados = logsLista.Where(log => DateTime.TryParse(log.Fecha, out DateTime fechaLog) && fechaLog >= FechaDT && fechaLog <= FechaHastaDT).ToList();
                        query = logsFiltrados.AsQueryable();
                    }
                    else
                    {
                        query = query.Where(log => log.Fecha == Fecha);
                    }
                }
                if (!string.IsNullOrEmpty(Yate))
                {
                    query = query.Where(log => log.Yate == Yate);
                }
                if (!string.IsNullOrEmpty(Capitan))
                {
                    query = query.Where(log => log.Capitan == Capitan);
                }
                if (!string.IsNullOrEmpty(Grupo))
                {
                    query = query.Where(log => log.Grupo == Grupo);
                }
            }
            else
            {
                query = Enumerable.Empty<PescaLog>().AsQueryable();
            }
            return query;
        }
        private MemoryStream ArmarWorkBook(IQueryable<PescaLog> query)
        {
            var memoryStream = new MemoryStream();
            using (var workbook = new XSSFWorkbook())
            {
                var worksheet = workbook.CreateSheet("Registro de Pesca");
                worksheet.CreateFreezePane(0, 5);
                // Estilos
                var headerStyle = workbook.CreateCellStyle();
                var font = workbook.CreateFont();
                font.IsBold = true;
                headerStyle.SetFont(font);
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.BorderTop = BorderStyle.Thin;
                headerStyle.BorderBottom = BorderStyle.Thin;
                headerStyle.BorderLeft = BorderStyle.Thin;
                headerStyle.BorderRight = BorderStyle.Thin;

                // Estilo para la fila 2
                var titleStyle = workbook.CreateCellStyle();
                var titleFont = workbook.CreateFont();
                titleFont.FontName = "Calibri";
                titleFont.FontHeightInPoints = 18;
                titleFont.IsBold = true;
                titleStyle.SetFont(titleFont);
                titleStyle.Alignment = HorizontalAlignment.Center;
                titleStyle.VerticalAlignment = VerticalAlignment.Center;

                // Estilo para celdas de datos
                var dataStyle = workbook.CreateCellStyle();
                dataStyle.BorderTop = BorderStyle.Thin;
                dataStyle.BorderBottom = BorderStyle.Thin;
                dataStyle.BorderLeft = BorderStyle.Thin;
                dataStyle.BorderRight = BorderStyle.Thin;

                // Crear fila de título con el nuevo estilo
                var fila2 = worksheet.CreateRow(1);
                fila2.CreateCell(0).SetCellValue("Estadística de Pesca, Pacific Fins");
                worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, 33));
                fila2.GetCell(0).CellStyle = titleStyle;

                // Fila de encabezados combinados
                var fila4 = worksheet.CreateRow(3);
                string[] headerTitles = { "Vela", "Vela en la mosca", "Marlin Azul", "Marlin Azul en la mosca",
                                  "Marlin Rayado", "Marlin Rayado en la mosca", "Marlin Negro" };
                int startCol = 6;

                // Agregar encabezados combinados y aplicar estilo a cada celda en la región
                foreach (var title in headerTitles)
                {
                    worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(3, 3, startCol, startCol + 2));
                    for (int i = startCol; i <= startCol + 2; i++)
                    {
                        var cell = fila4.CreateCell(i);
                        cell.SetCellValue(i == startCol ? title : ""); // Solo poner texto en la primera celda
                        cell.CellStyle = headerStyle;
                    }
                    startCol += 3;
                }

                // Encabezados específicos en la fila siguiente
                var fila5 = worksheet.CreateRow(4);
                string[] headers = { "Fecha", "Yate", "Capitán", "Grupo", "Diesel", "Millas",
                                    "Flotados", "Piques", "Liberados",
                                    "Flotados", "Piques", "Liberados",
                                    "Flotados", "Piques", "Liberados",
                                    "Flotados", "Piques", "Liberados",
                                    "Flotados", "Piques", "Liberados",
                                    "Flotados", "Piques", "Liberados",
                                    "Flotados", "Piques", "Liberados",
                                    "Dorado", "Dorado Fly", "Atún", "Atún Fly", "Gallos", "Wahoo", "Comentarios" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = fila5.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = headerStyle;
                }

                // Llenar los datos comenzando en la fila 6
                int rowIndex = 5;
                foreach (var item in query.ToList())
                {
                    var row = worksheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.Fecha);
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

                    // Aplicar estilo con bordes a las celdas de datos
                    for (int i = 0; i < headers.Length; i++)
                    {
                        row.GetCell(i).CellStyle = dataStyle;
                    }
                }

                for (int i = 0; i < headers.Length; i++)
                {
                    //worksheet.AutoSizeColumn(i);
                }
                worksheet.SetColumnWidth(0, 12 * 256);
                worksheet.SetColumnWidth(1, 12 * 256);
                worksheet.SetColumnWidth(2, 24 * 256);
                worksheet.SetColumnWidth(3, 20 * 256);
                worksheet.SetColumnWidth(33, 100 * 256);
                using (memoryStream)
                {
                    workbook.Write(memoryStream);
                }
            }
            return memoryStream;
        }
        private byte[] GenerarReporte(IQueryable<PescaLog> query,string? FFecha, string? FFechaHasta)
        {
            int vRaises = 0;
            int vBites = 0;
            int vReleases = 0;
            int vMarlin = 0;
            int vDorado = 0;
            int vTuna = 0;
            foreach (var item in query.ToList())
            {
                vRaises += item.VelaFlotados + item.VelaMoscaFlotados;
                vBites += item.VelaPiques + item.VelaMoscaPiques;
                vReleases += item.VelaLiberados + item.VelaMoscaLiberados;
                vMarlin += item.MarlinAzulFlotados + item.MarlinMoscaAzulFlotados + item.MarlinRayadoFlotados + item.MarlinMoscaRayadoFlotados + item.MarlinNegroFlotados;
                vDorado += item.Dorado + item.DoradoFly;
                vTuna += item.Atun;
            }
            string templatePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "plantilla.pptx");
            string PPTPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "plantilla2.pptx");
            System.IO.File.Copy(templatePath, PPTPath, true);
            using (PresentationDocument presentationDocument = PresentationDocument.Open(PPTPath, true))
            {
                var shapes = presentationDocument.PresentationPart.SlideParts.FirstOrDefault().Slide.CommonSlideData.ShapeTree.ChildElements.OfType<Shape>();
                foreach (var shape in shapes)
                {
                    string valor = " ";
                    bool esFecha = false;
                    switch (shape.NonVisualShapeProperties.NonVisualDrawingProperties.Name.ToString())
                    {
                        case "Raises":
                            valor = vRaises.ToString();
                            break;
                        case "Bites":
                            valor = vBites.ToString();
                            break;
                        case "Releases":
                            valor = vReleases.ToString();
                            break;
                        case "Marlin":
                            valor = vMarlin.ToString();
                            break;
                        case "Dorado":
                            valor = vDorado.ToString();
                            break;
                        case "Tuna":
                            valor = vTuna.ToString();
                            break;
                        case "Fecha":
                            valor = FFecha[8..10] + "/" + FFecha[5..7] + "/" + FFecha[2..4] + " - " + FFechaHasta[8..10] + "/" + FFechaHasta[5..7] + "/" + FFechaHasta[2..4]; ;
                            esFecha = true;
                            break;
                    }
                    shape.TextBody.RemoveAllChildren();
                    var paragraph = new DocumentFormat.OpenXml.Drawing.Paragraph();
                    var run = new DocumentFormat.OpenXml.Drawing.Run();
                    
                    if (esFecha)
                    {
                        var runProperties = new DocumentFormat.OpenXml.Drawing.RunProperties
                        {
                            FontSize = new Int32Value(1600),
                        };
                        var solidFill = new SolidFill(new RgbColorModelHex() { Val = "2F5597" }); 
                        runProperties.Append(solidFill);
                        runProperties.Append(new DocumentFormat.OpenXml.Spreadsheet.Bold());
                        run.Append(runProperties);
                    }                    
                    run.Append(new DocumentFormat.OpenXml.Drawing.Text(valor));
                    paragraph.Append(run);
                    shape.TextBody.Append(paragraph);
                }
                presentationDocument.Save();
            }
            return System.IO.File.ReadAllBytes(PPTPath);
        }
        private List<string> BuscarYates()
        {
            return new List<string> { "KnotWork", "Maverick", "Amante", "Chechos", "Mojo", "Libertad" };
        }
        private StringBuilder ListaDeYates(string selectedYate)
        {
            var optionsYate = new StringBuilder();
            var yates = BuscarYates();
            
            optionsYate.Append("<option value=\"\"> </option>");
            foreach (var yate in yates)
            {
                var selected = yate == selectedYate ? " selected=\"selected\"" : "";
                optionsYate.Append($"<option value=\"{yate}\"{selected}>{yate}</option>");
            }
            return optionsYate;
        }
        private List<string> BuscarCapitanes()
        {
            return new List<string> { "Jorge Pineda", "Rosendo López", "Nestor García", "Jimmy Fajardo" };
        }
        private StringBuilder ListaDeCapitanes(string selectedCapitan)
        {
            var optionsCapitan = new StringBuilder();
            var capitanes = BuscarCapitanes();

            optionsCapitan.Append("<option value=\"\"> </option>");
            foreach (var capitan in capitanes)
            {
                var selected = capitan == selectedCapitan ? " selected=\"selected\"" : "";
                optionsCapitan.Append($"<option value=\"{capitan}\"{selected}>{capitan}</option>");
            }
            return optionsCapitan;
        }
        public void ImportFromExcel(string filePath)
        {
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(file);
                var sheet = workbook.GetSheetAt(0); // Cambia el índice si es necesario

                var pescaLogs = new List<PescaLog>();

                for (int row = 6; row <= sheet.LastRowNum; row++) 
                {
                    var currentRow = sheet.GetRow(row);
                    if (currentRow == null)
                    {
                        break;
                    }
                    else if (currentRow.GetCell(1) == null)
                    {
                        break;
                    } else if (string.IsNullOrWhiteSpace(currentRow.GetCell(1).ToString())) {
                        break;                     
                    }
                var pescaLog = new PescaLog
                {
                        Fecha = currentRow.GetCell(1)?.StringCellValue ?? string.Empty,
                        Yate = currentRow.GetCell(2)?.StringCellValue ?? string.Empty,
                        Grupo = currentRow.GetCell(3)?.StringCellValue ?? string.Empty,
                        VelaFlotados = (int)(currentRow.GetCell(4)?.NumericCellValue ?? 0),
                        VelaPiques = (int)(currentRow.GetCell(5)?.NumericCellValue ?? 0),
                        VelaLiberados = (int)(currentRow.GetCell(6)?.NumericCellValue ?? 0),
                        VelaMoscaFlotados = (int)(currentRow.GetCell(8)?.NumericCellValue ?? 0),
                        VelaMoscaPiques = (int)(currentRow.GetCell(9)?.NumericCellValue ?? 0),
                        VelaMoscaLiberados = (int)(currentRow.GetCell(10)?.NumericCellValue ?? 0),
                        MarlinAzulFlotados = (int)(currentRow.GetCell(12)?.NumericCellValue ?? 0),
                        MarlinAzulPiques = (int)(currentRow.GetCell(13)?.NumericCellValue ?? 0),
                        MarlinAzulLiberados = (int)(currentRow.GetCell(14)?.NumericCellValue ?? 0),
                        MarlinMoscaAzulFlotados = (int)(currentRow.GetCell(16)?.NumericCellValue ?? 0),
                        MarlinMoscaAzulPiques = (int)(currentRow.GetCell(17)?.NumericCellValue ?? 0),
                        MarlinMoscaAzulLiberados = (int)(currentRow.GetCell(18)?.NumericCellValue ?? 0),
                        MarlinRayadoFlotados = (int)(currentRow.GetCell(20)?.NumericCellValue ?? 0),
                        MarlinRayadoPiques = (int)(currentRow.GetCell(21)?.NumericCellValue ?? 0),
                        MarlinRayadoLiberados = (int)(currentRow.GetCell(22)?.NumericCellValue ?? 0),
                        MarlinMoscaRayadoFlotados = (int)(currentRow.GetCell(24)?.NumericCellValue ?? 0),
                        MarlinMoscaRayadoPiques = (int)(currentRow.GetCell(25)?.NumericCellValue ?? 0),
                        MarlinMoscaRayadoLiberados = (int)(currentRow.GetCell(26)?.NumericCellValue ?? 0),
                        MarlinNegroFlotados = (int)(currentRow.GetCell(28)?.NumericCellValue ?? 0),
                        MarlinNegroPiques = (int)(currentRow.GetCell(29)?.NumericCellValue ?? 0),
                        MarlinNegroLiberados = (int)(currentRow.GetCell(30)?.NumericCellValue ?? 0),
                        Dorado = (int)(currentRow.GetCell(32)?.NumericCellValue ?? 0),
                        DoradoFly = (int)(currentRow.GetCell(34)?.NumericCellValue ?? 0),
                        Atun = (int)(currentRow.GetCell(36)?.NumericCellValue ?? 0),
                        AtunFly = (int)(currentRow.GetCell(37)?.NumericCellValue ?? 0),
                        Gallos = (int)(currentRow.GetCell(38)?.NumericCellValue ?? 0),
                        Wahoo = (int)(currentRow.GetCell(39)?.NumericCellValue ?? 0),
                        DieselConsumido = currentRow.GetCell(40)?.NumericCellValue ?? 0,
                        MillasRecorridas = currentRow.GetCell(41)?.NumericCellValue ?? 0,
                        Comentario = currentRow.GetCell(42)?.StringCellValue ?? string.Empty,
                        Capitan = currentRow.GetCell(43)?.StringCellValue ?? string.Empty,
                    };
                    pescaLog.Fecha = pescaLog.Fecha[^4..] + "-" + pescaLog.Fecha[3..5] + "-" + pescaLog.Fecha[0..2];
                    pescaLogs.Add(pescaLog);
                }
                _context.PescaLogs.InsertMany(pescaLogs); // Inserta todos los registros en la base de datos
            }
        }
        public IActionResult Index(string? FFecha, string? FFechaHasta, string? FYate, string? FCapitan, string? FGrupo)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            TempData["LYate"] = ListaDeYates(FYate?? "");
            TempData["LCapitan"] = ListaDeCapitanes(FCapitan?? "");
            TempData["FFecha"] = FFecha;
            TempData["FFechaHasta"] = FFechaHasta; 
            TempData["FYate"] = FYate;
            TempData["FCapitan"] = FCapitan;
            TempData["FGrupo"] = FGrupo;
            IQueryable<PescaLog> query = BuscarRegistros(FFecha, FFechaHasta, FYate, FCapitan, FGrupo);
            return View(query.ToList());
        }

        public IActionResult Create(string FFecha, string? FFechaHasta, string FYate, string FCapitan, string? FGrupo)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            TempData["LYate"] = ListaDeYates(FYate);
            TempData["LCapitan"] = ListaDeCapitanes(FCapitan);
            TempData["FFecha"] = FFecha;
            TempData["FFechaHasta"] = FFechaHasta; 
            TempData["FYate"] = FYate;
            TempData["FCapitan"] = FCapitan;
            TempData["FGrupo"] = FGrupo;
            return View();
        }

        [HttpPost]
        public IActionResult Create(PescaLog pescaLog, string? FFecha, string? FFechaHasta, string? FYate, string? FCapitan, string? FGrupo)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            _context.PescaLogs.InsertOne(pescaLog);
            return RedirectToAction(nameof(Index), new { FFecha, FFechaHasta, FYate, FCapitan, FGrupo });
        }

        public IActionResult Edit(string id, string? FFecha, string? FFechaHasta, string? FYate, string? FCapitan, string? FGrupo)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var log = _context.PescaLogs.Find(log => log.Id == ObjectId.Parse(id)).FirstOrDefault();
            TempData["LYate"] = ListaDeYates(log.Yate);
            TempData["LCapitan"] = ListaDeCapitanes(log.Capitan);
            TempData["FFecha"] = FFecha;
            TempData["FFechaHasta"] = FFechaHasta;
            TempData["FYate"] = FYate;
            TempData["FCapitan"] = FCapitan;
            TempData["FGrupo"] = FGrupo;            
            return View(log);
        }

        [HttpPost]
        public IActionResult Edit(PescaLog pescaLog, string? FFecha, string? FFechaHasta, string? FYate, string? FCapitan , string? FGrupo)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            _context.PescaLogs.ReplaceOne(log => log.Id == pescaLog.Id, pescaLog);
            return RedirectToAction(nameof(Index), new { FFecha, FFechaHasta, FYate, FCapitan, FGrupo });
        }

        public IActionResult Delete(string id, string? FFecha, string? FFechaHasta, string? FYate, string? FCapitan , string? FGrupo)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            TempData["FFecha"] = FFecha;
            TempData["FFechaHasta"] = FFechaHasta;
            TempData["FYate"] = FYate;
            TempData["FCapitan"] = FCapitan;
            TempData["FGrupo"] = FGrupo;
            var pescaLog = _context.PescaLogs.Find(log => log.Id == ObjectId.Parse(id)).FirstOrDefault();
            if (pescaLog == null)
            {
                return NotFound();
            }
            return View(pescaLog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id, string? FFecha, string? FFechaHasta, string? FYate, string? FCapitan , string? FGrupo)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var result = _context.PescaLogs.DeleteOne(log => log.Id == ObjectId.Parse(id));
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index), new { FFecha, FFechaHasta, FYate, FCapitan, FGrupo });
        }

        public IActionResult Reporte(string? FFecha, string? FFechaHasta, string? FYate, string? FCapitan , string? FGrupo)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            IQueryable<PescaLog> query = BuscarRegistros(FFecha, FFechaHasta, FYate, FCapitan, FGrupo);
            byte[] data = GenerarReporte(query,FFecha, FFechaHasta);
            return File(data, "application/vnd.openxmlformats-officedocument.presentationml.presentation", "presentacion_editada.pptx");
        }
        public IActionResult DownloadExcel(string? FFecha, string? FFechaHasta, string? FYate, string? FCapitan , string? FGrupo)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            IQueryable<PescaLog> query = BuscarRegistros(FFecha, FFechaHasta, FYate, FCapitan, FGrupo);
            MemoryStream memoryStream = ArmarWorkBook(query);
            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RegistroDePesca.xlsx");
        }
        [HttpPost]
        public IActionResult UploadExcel(IFormFile file)
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            // Genera un nombre único para evitar colisiones y guárdalo en una carpeta temporal
            var tempFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var tempPath = System.IO.Path.Combine(Path.GetTempPath(), tempFileName);

            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Llama al método de importación con el nombre del archivo temporal
                ImportFromExcel(tempPath);
            }
            finally
            {
                // Elimina el archivo temporal después de importarlo
                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }
            }

            return RedirectToAction("Index");
        }

    }
}
