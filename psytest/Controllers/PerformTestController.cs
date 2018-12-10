using Jint;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using psytest.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace psytest.Controllers
{
    public class PerformTestController : Controller
    {
        private readonly TestContext _context;

        public PerformTestController(TestContext context)
        {
            _context = context;
        }

        public IActionResult Test(int testID, int questionNumber, bool? clearCookies)
        {
            // DON'T WORK
            // if (clearCookies)
            // {
            //     foreach (var cookie in Request.Cookies.Keys)
            //     {
            //         Response.Cookies.Append(cookie, "");
            //     }
            // }
            Console.WriteLine($"Test ID = {testID}, Question No. {questionNumber}");
            Test test = _context.Tests.Include(t => t.Questions).ThenInclude(q => q.Type).FirstOrDefault(m => m.Id == testID);
            if (test == null)
            {
                return NotFound($"Test with id {testID} Not found");
            }
            int questionsCount = test.Questions?.Count ?? 0;
            if (questionNumber > questionsCount || questionNumber < 1)
            {
                return NotFound($"Test with id {testID} has only "
                + $"{questionsCount} questions, {questionNumber} is out of range");
            }
            Question question = test.Questions[questionNumber - 1];
            ViewBag.TestID = testID;
            ViewBag.QuestionNumber = questionNumber;
            ViewBag.QuestionsCount = questionsCount;
            ViewBag.Question = question;
            ViewBag.QuestionType = question.Type;
            ViewBag.Cookies = Request.Cookies;
            ViewBag.ShouldClearCookies = clearCookies ?? false;
            return View();
        }

        [HttpPost]
        public IActionResult Submit(int testID, [FromForm] Dictionary<int, int> results)
        {
            Console.WriteLine($"Test ID = {testID}");
            foreach (var res in results)
            {
                Console.WriteLine($"Answer {res.Key} = {res.Value}");
            }
            Test test = _context.Tests.FirstOrDefault(m => m.Id == testID);
            if (test == null)
            {
                return NotFound($"Test with id {testID} Not found");
            }
            var metrics = new Engine()
                .SetValue("questions", results)
                .SetValue("metrics", new Dictionary<String, Object>())
                .Execute(test.MetricsComputeScript)
                .GetValue("metrics");
            ViewBag.Results = results;
            ViewBag.Metrics = metrics;
            ViewBag.MetricDescriptions = test.MetricsDescriptions;
            return View();
        }
    }
}