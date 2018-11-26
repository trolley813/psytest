using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using psytest.Models;

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
            if (question?.Type is VariantQuestionType)
            {
                ViewBag.Temp = _context.Questions.Where(q => q.Type is VariantQuestionType)
                    .Select(q => new { q, (q.Type as VariantQuestionType).Variants })
                    .AsEnumerable()
                    .Select(qv => qv.q)
                    .ToList()
                    ;
            }
            ViewBag.TestID = testID;
            ViewBag.QuestionNumber = questionNumber;
            ViewBag.QuestionsCount = questionsCount;
            ViewBag.Question = question;
            ViewBag.QuestionType = question.Type;
            ViewBag.Cookies = Request.Cookies;
            ViewBag.ShouldClearCookies = clearCookies ?? false;
            return View();
        }
    }
}