using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using psytest.Areas.Identity.Data;
using psytest.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace psytest.Controllers
{
    public class ResultsController : Controller
    {
        private readonly TestContext testContext;
        private readonly TestResultContext resultContext;
        private readonly UserContext userContext;
        private UserManager<TestingUser> userManager;

        public ResultsController(TestContext testContext, TestResultContext resultContext,
            UserContext userContext, UserManager<TestingUser> userManager)
        {
            this.testContext = testContext;
            this.resultContext = resultContext;
            this.userContext = userContext;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = userManager.Users.Select(u => new { u.Id, User = u }).ToDictionary(u => u.Id, u => u.User);
            ViewBag.Results = resultContext.TestResults.Select(
                t => new Dictionary<string, object>
                {
                    { "UserName", users[t.UserId].Name },
                    { "UserDOB", users[t.UserId].DOB },
                    { "TestingDate", t.TestingDate },
                    { "UserAge", StatsHelper.GetAge(t.TestingDate, users[t.UserId].DOB) },
                    { "Metrics", t.Metrics }
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
    }
}