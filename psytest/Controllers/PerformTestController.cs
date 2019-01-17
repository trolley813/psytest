using Jint;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using psytest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using psytest.Areas.Identity.Data;

namespace psytest.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class PerformTestController : Controller
    {
        private readonly TestContext testContext;
        private readonly TestResultContext resultContext;
        private readonly UserContext userContext;
        private UserManager<TestingUser> userManager;

        public PerformTestController(TestContext testContext, TestResultContext resultContext, 
            UserContext userContext, UserManager<TestingUser> userManager)
        {
            this.testContext = testContext;
            this.resultContext = resultContext;
            this.userContext = userContext;
            this.userManager = userManager;
        }

        public IActionResult Test(int testID, int questionNumber, bool? clearCookies)
        {
            // DOESN'T WORK
            // if (clearCookies)
            // {
            //     foreach (var cookie in Request.Cookies.Keys)
            //     {
            //         Response.Cookies.Append(cookie, "");
            //     }
            // }
            Console.WriteLine($"Test ID = {testID}, Question No. {questionNumber}");
            Test test = testContext.Tests.Include(t => t.Questions).ThenInclude(q => q.Type).FirstOrDefault(m => m.Id == testID);
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
            Question question = test.Questions.OrderBy(q => q.Number).ToArray()[questionNumber - 1];
            ViewBag.TestID = testID;
            ViewBag.Instruction = test.Instruction;
            ViewBag.QuestionNumber = questionNumber;
            ViewBag.QuestionsCount = questionsCount;
            ViewBag.Question = question;
            ViewBag.QuestionType = question.Type;
            ViewBag.Cookies = Request.Cookies;
            ViewBag.MinQuestionNumber = test.Questions.Where(q => q.Part == question.Part).Select(q => q.Number).Min();
            ViewBag.MaxQuestionNumber = test.Questions.Where(q => q.Part == question.Part).Select(q => q.Number).Max();
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
            Test test = testContext.Tests.FirstOrDefault(m => m.Id == testID);
            if (test == null)
            {
                return NotFound($"Test with id {testID} Not found");
            }
            var metrics = new Engine()
                .SetValue("questions", results)
                .SetValue("metrics", new Dictionary<String, Object>())
                .Execute(test.MetricsComputeScript)
                .GetValue("metrics");

            
            var testResult = new TestResult
            {
                UserId = userManager.GetUserId(User),
                TestId = testID,
                Metrics = metrics.ToObject() as Dictionary<String, Object>,
                TestingDate = DateTime.Now
            };

            resultContext.Add(testResult);
            resultContext.SaveChanges();

            ViewBag.Results = results;
            ViewBag.Metrics = metrics;
            ViewBag.MetricDescriptions = test.MetricsDescriptions;
            return View();
        }

    }
}