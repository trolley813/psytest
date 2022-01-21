using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using psytest.Areas.Identity.Data;
using psytest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace psytest.Controllers
{
    public class ResultsController : Controller
    {
        private readonly TestContext testContext;
        private readonly TestResultContext resultContext;
        private readonly UserContext userContext;
        private UserManager<TestingUser> userManager;
        private readonly IHostingEnvironment hostingEnvironment;

        public ResultsController(TestContext testContext, TestResultContext resultContext,
            UserContext userContext, UserManager<TestingUser> userManager,
            IHostingEnvironment hostingEnvironment)
        {
            this.testContext = testContext;
            this.resultContext = resultContext;
            this.userContext = userContext;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var users = userManager.Users.Select(u => new { u.Id, User = u }).ToDictionary(u => u.Id, u => u.User);
            var tests = testContext.Tests.Select(t => new { t.Id, t.Name, t.MetricsDescriptions }).ToDictionary(t => t.Id, t => new { t.Name, t.MetricsDescriptions });
            ViewBag.Results = resultContext.TestResults.Select(
                t => new Dictionary<string, object>
                {
                    { "UserName", users[t.UserId].Name },
                    { "UserDOB", users[t.UserId].DOB },
                    { "TestingDate", t.TestingDate },
                    { "UserAge", StatsHelper.GetAge(t.TestingDate, users[t.UserId].DOB) },
                    { "Test", tests.GetValueOrDefault(t.TestId, new {Name = $"(no data - ID {t.TestId})", MetricsDescriptions = new Dictionary<String, String>()}).Name },
                    { "Metrics", t.Metrics.Select(m => new {
                        Key = tests.GetValueOrDefault(t.TestId, new {Name = $"(no data - ID {t.TestId})", MetricsDescriptions = new Dictionary<String, String>()})
                              .MetricsDescriptions.GetValueOrDefault(m.Key, $"(unknown metric - {m.Key})"),
                        Value = m.Value
                         }).ToDictionary(m => m.Key, m => m.Value) }
                }).ToArray();
            return View();
        }

        public IActionResult Stats()
        {
            var stats = StatsHelper.GetStatsByTests(testContext, resultContext);
            ViewBag.Statistics = testContext.Tests
                .Select(t => new { t.Name, Stats = stats.GetValueOrDefault(t.Id) })
                .ToDictionary(t => t.Name, t => t.Stats);
            return View();
        }

        public IActionResult ExcelExport(bool delete = false)
        {
            var users = userManager.Users.Select(u => new { u.Id, User = u }).ToDictionary(u => u.Id, u => u.User);
            var tests = testContext.Tests.Select(t => new { t.Id, t.Name, t.MetricsDescriptions }).ToDictionary(t => t.Id, t => new { t.Name, t.MetricsDescriptions });
            var results = resultContext.TestResults.Select(
                t => new Dictionary<string, object>
                {
                    { "UserName", users.ContainsKey(t.UserId) ? users[t.UserId].Name : "(unknown user)" },
                    { "UserDOB",  users.ContainsKey(t.UserId) ? users[t.UserId].DOB : DateTime.MinValue },
                    { "TestingDate", t.TestingDate },
                    { "UserAge", StatsHelper.GetAge(t.TestingDate, users.ContainsKey(t.UserId) ? users[t.UserId].DOB : DateTime.MinValue) },
                    { "TestID", t.TestId },
                    { "Test", tests.GetValueOrDefault(t.TestId, new {Name = $"(no data - ID {t.TestId})", MetricsDescriptions = new Dictionary<String, String>()}).Name },
                    { "Metrics", t.Metrics.Select(m => new {
                        Key = tests.GetValueOrDefault(t.TestId, new {Name = $"(no data - ID {t.TestId})", MetricsDescriptions = new Dictionary<String, String>()})
                              .MetricsDescriptions.GetValueOrDefault(m.Key, $"(unknown metric - {m.Key})"),
                        Value = m.Value
                         }).ToDictionary(m => m.Key, m => m.Value) }
                }).ToArray();
            string webRootFolder = hostingEnvironment.WebRootPath;
            string fileName = $"results_{DateTime.Now.ToString("yyyyMMdd_HHmmss_fff")}.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(webRootFolder, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(webRootFolder, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                Dictionary<int, ISheet> excelSheets = tests.Select(t => new { t.Key, Value = workbook.CreateSheet(t.Value.Name) })
                    .ToDictionary(s => s.Key, s => s.Value);
                IDataFormat dataFormatCustom = workbook.CreateDataFormat();

                foreach (var t in tests)
                {
                    IRow row = excelSheets[t.Key].CreateRow(0);
                    row.CreateCell(0).SetCellValue("User Name");
                    row.CreateCell(1).SetCellValue("User DOB");
                    row.CreateCell(2).SetCellValue("Testing Date");
                    row.CreateCell(3).SetCellValue("Age");
                    int idx = 4;
                    foreach (var m in t.Value.MetricsDescriptions ?? new Dictionary<string, string>())
                    {
                        row.CreateCell(idx++).SetCellValue(m.Value);
                    }
                }

                Dictionary<int, int> rowCounts = tests.Keys.Select(k => new { Key = k, Value = 0 }).ToDictionary(rc => rc.Key, rc => rc.Value);

                for (int i = 0; i < results.Count(); i++)
                {
                    var testID = results[i]["TestID"] as int? ?? 0;
                    if (!excelSheets.ContainsKey(testID))
                    {
                        excelSheets.Add(testID, workbook.CreateSheet($"Test {testID} - DELETED"));
                        rowCounts[testID] = 0;
                    }
                    var row = excelSheets[testID].CreateRow(++rowCounts[testID]);
                    row.CreateCell(0).SetCellValue((results[i]["UserName"] as String) ?? "[unknown user]");
                    row.CreateCell(1).SetCellValue((results[i]["UserDOB"] as DateTime?) ?? DateTime.MinValue);
                    row.CreateCell(2).SetCellValue((results[i]["TestingDate"] as DateTime?) ?? DateTime.MinValue);
                    row.CreateCell(3).SetCellValue(StatsHelper.AgeFormatForExcel((results[i]["UserAge"] as (int, int)?) ?? (0, 0)));
                    int idx = 4;
                    foreach(var metric in results[i]["Metrics"] as Dictionary<String, Object>)
                    {
                        row.CreateCell(idx++).SetCellValue(metric.Value as Double? ?? 0.0);
                    }

                    row.GetCell(1).CellStyle.DataFormat = dataFormatCustom.GetFormat("yyyyMMdd HH:mm:ss");
                    row.GetCell(2).CellStyle.DataFormat = dataFormatCustom.GetFormat("yyyyMMdd HH:mm:ss");
                }

                workbook.Write(fs);
            }

            using (var stream = new FileStream(Path.Combine(webRootFolder, fileName), FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            // Danger
            if (delete)
            {
                var allResults = resultContext.TestResults.ToArray();
                resultContext.TestResults.RemoveRange(allResults);
                resultContext.SaveChanges();
                // restart the sequence from 1 (currently hardcoded to Postgres)
                resultContext.Database.ExecuteSqlCommand("SELECT setval('\"TestResults_Id_seq\"', 1, FALSE)");
            }

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }

    static class StatsHelper
    {
        public static Dictionary<int, (Dictionary<String, Double>, Dictionary<String, Double>)>
            GetStatsByTests(TestContext testContext, TestResultContext resultContext)
        {
            var grouped = resultContext.TestResults.GroupBy(tr => tr.TestId).Select(g => new
            {
                TestId = g.Key,
                Metrics = g.Select(r => r.Metrics)
            });
            var result = new Dictionary<int, (Dictionary<String, Double>, Dictionary<String, Double>)>();
            foreach (var g in grouped)
            {
                result[g.TestId] = (new Dictionary<String, Double>(), new Dictionary<String, Double>());
                foreach (var m in testContext.Tests.First(t => t.Id == g.TestId).MetricsDescriptions)
                {
                    var allResultsForMetric = g.Metrics.Select(met => (met[m.Key] as double?) ?? 0.0);
                    var avg = allResultsForMetric.Average();
                    var count = allResultsForMetric.Count();
                    var devSum = allResultsForMetric.Sum(d => (d - avg) * (d - avg));
                    var stdDev = Math.Sqrt(devSum / count);
                    result[g.TestId].Item1[m.Value] = avg;
                    result[g.TestId].Item2[m.Value] = stdDev;
                }
            }
            return result;
        }

        public static (int, int) GetAge(DateTime current, DateTime dob)
        {
            int years = current.Year - dob.Year;
            if (dob > current.AddYears(-years)) years--;
            int days = (current - dob.AddYears(years)).Days;
            return (years, days);
        }

        public static string AgeFormatForExcel((int, int) age) => $"{age.Item1}y {age.Item2}d";
    }
}
