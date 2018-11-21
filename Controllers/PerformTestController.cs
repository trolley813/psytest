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

        public IActionResult Test(int testID, int questionNumber)
        {
            Console.WriteLine($"Test ID = {testID}, Question No. {questionNumber}");
            Test test = _context.Tests.Include(t => t.Questions).FirstOrDefault(m => m.Id == testID);
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
            return View();
        }
    }
}