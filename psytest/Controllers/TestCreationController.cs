using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using psytest.Models;
using psytest.Wizard;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace psytest.Controllers
{
    public class TestCreationController : Controller
    {
        private readonly TestContext _context;
        private readonly ITestCreationRepository testCreationRepository;

        public TestCreationController(TestContext context, ITestCreationRepository testCreationRepository)
        {
            this.testCreationRepository = testCreationRepository;
            _context = context;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            var testCreation = testCreationRepository.StartNew(User.Identity.Name);

            return RedirectToAction("Create", new { testCreationId = testCreation.TestCreationId });
        }

        // Step 1 --- basic properties
        // Step 2 --- parts
        // Step 3 --- question texts
        // Step 4 --- metrics

        [WizardStep(1)]
        [ActionName("Create")]
        public IActionResult CreateStep1Get(Guid testCreationId)
        {
            var testCreation = testCreationRepository.FindByIdForUser(testCreationId, User.Identity.Name);

            if (testCreation == null)
            {
                return NotFound();
            }

            ViewBag.TestName = testCreation.TestName;
            ViewBag.TestInstruction = testCreation.TestInstruction;
            ViewBag.PartCount = testCreation.PartCount;

            return View("Create1");
        }

        [WizardStep(1)]
        [ActionName("Create")]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateStep1Post(Guid testCreationId, string go, 
            string testName, string testInstruction, 
            int partCount, int metricCount,
            string questionType)
        {
            // potential check here, return view if failed, 
            if (!ModelState.IsValid)
            {
                return View("Create1");
            }

            var testCreation = testCreationRepository.FindByIdForUser(testCreationId, User.Identity.Name);

            if (testCreation == null)
            {
                return NotFound();
            }

            testCreation.TestName = testName;
            testCreation.TestInstruction = testInstruction;
            testCreation.PartCount = partCount;
            testCreation.MetricCount = metricCount;

            var qType = new QuestionTypeProxy { Type = questionType }; ;
            
            switch(questionType)
            {
                case "slider":
                    qType.MinValue = Convert.ToInt32(Request.Form["sliderMin"].ToString());
                    qType.MaxValue = Convert.ToInt32(Request.Form["sliderMax"].ToString());
                    break;
                case "antagonistic":
                    qType.MinValue = Convert.ToInt32(Request.Form["antaMin"].ToString());
                    qType.MaxValue = Convert.ToInt32(Request.Form["antaMax"].ToString());
                    break;
                case "variant":
                    qType.Variants = Request.Form["variants"].ToString();
                    break;
            }

            testCreation.QuestionType = qType;

            if (go == "Next")
            {
                testCreationRepository.MoveNext(testCreation);
            }
            else
            {
                testCreationRepository.MovePrevious(testCreation);
            }

            return RedirectToAction("Create", new { testCreationId });
        }

        [WizardStep(2)]
        [ActionName("Create")]
        public IActionResult CreateStep2Get(Guid testCreationId)
        {
            var testCreation = testCreationRepository.FindByIdForUser(testCreationId, User.Identity.Name);

            if (testCreation == null)
            {
                return NotFound();
            }

            ViewBag.PartCount = testCreation.PartCount;
            ViewBag.CountByPart = testCreation.CountByPart;

            return View("Create2");
        }

        [WizardStep(2)]
        [ActionName("Create")]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateStep2Post(Guid testCreationId, string go)
        {
            // potential check here, return view if failed, 
            if (!ModelState.IsValid)
            {
                return View("Create2");
            }

            var testCreation = testCreationRepository.FindByIdForUser(testCreationId, User.Identity.Name);

            if (testCreation == null)
            {
                return NotFound();
            }

            testCreation.CountByPart = Enumerable.Range(1, testCreation.PartCount)
                .Select(i => Convert.ToInt32(Request.Form[$"questionCount{i}"].ToString()))
                .ToList();

            if (go == "Next")
            {
                testCreationRepository.MoveNext(testCreation);
            }
            else
            {
                testCreationRepository.MovePrevious(testCreation);
            }

            return RedirectToAction("Create", new { testCreationId });
        }

        [WizardStep(3)]
        [ActionName("Create")]
        public IActionResult CreateStep3Get(Guid testCreationId)
        {
            var testCreation = testCreationRepository.FindByIdForUser(testCreationId, User.Identity.Name);

            if (testCreation == null)
            {
                return NotFound();
            }

            ViewBag.QuestionTexts = testCreation.QuestionTexts;
            ViewBag.QuestionCount = testCreation.QuestionCount;

            return View("Create3");
        }

        [WizardStep(3)]
        [ActionName("Create")]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateStep3Post(Guid testCreationId, string go)
        {
            // potential check here, return view if failed, 
            if (!ModelState.IsValid)
            {
                return View("Create1");
            }

            var testCreation = testCreationRepository.FindByIdForUser(testCreationId, User.Identity.Name);

            if (testCreation == null)
            {
                return NotFound();
            }

            testCreation.QuestionTexts = Enumerable.Range(1, testCreation.PartCount)
                .Select(i => Request.Form[$"questionCount{i}"].ToString())
                .ToList();

            if (go == "Next")
            {
                testCreationRepository.MoveNext(testCreation);
            }
            else
            {
                testCreationRepository.MovePrevious(testCreation);
            }

            return RedirectToAction("Create", new { testCreationId });
        }

        [WizardStep(4)]
        [ActionName("Create")]
        public IActionResult CreateStep4Get(Guid testCreationId)
        {
            var testCreation = testCreationRepository.FindByIdForUser(testCreationId, User.Identity.Name);

            if (testCreation == null)
            {
                return NotFound();
            }

            ViewBag.MetricCount = testCreation.MetricCount;

            return View("Create4");
        }

        [WizardStep(4)]
        [ActionName("Create")]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateStep4Post(Guid testCreationId, string go)
        {
            // potential check here, return view if failed, 
            if (!ModelState.IsValid)
            {
                return View("Create4");
            }

            var testCreation = testCreationRepository.FindByIdForUser(testCreationId, User.Identity.Name);

            if (testCreation == null)
            {
                return NotFound();
            }

            // TODO: JS validation
            testCreation.Metrics = Enumerable.Range(1, testCreation.PartCount)
                .Select(i => new Metric {
                    Name = Request.Form[$"name{i}"].ToString(),
                    DirectQuestions = Request.Form[$"directQuestions{i}"].ToString().AsRangesList(),
                    InverseQuestions = Request.Form[$"inverseQuestions{i}"].ToString().AsRangesList(),
                    AdditionalComputeExpression = Request.Form[$"expr{i}"].ToString()
                })
                .ToList();

            if (go == "Finish")
            {
                testCreationRepository.MoveNext(testCreation);
                return RedirectToAction("CreateTest", new { testCreationId });
            }
            else
            {
                testCreationRepository.MovePrevious(testCreation);
                return RedirectToAction("Create", new { testCreationId });
            }

            
        }

        public IActionResult CreateTest(Guid testCreationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var testCreation = testCreationRepository.FindByIdForUser(testCreationId, User.Identity.Name);

            if (testCreation == null)
            {
                return NotFound();
            }

            try
            {
                TestGenerator.GenerateTest(testCreation, _context);
                return View();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }

    static class StringRangeExtension
    {
        // Helper method for converting an enumerating string (e.g. 1-3,5,8-13) to list
        public static List<int> AsRangesList(this string s)
        {
            return s.Split(",").Select(x => {
                var r = x.Split("-");
                switch (r.Length)
                {
                    case 1:
                        Int32.TryParse(r[0], out int y0);
                        return Enumerable.Range(y0, 1);
                    case 2:
                        Int32.TryParse(r[0], out int y1);
                        Int32.TryParse(r[1], out int y2);
                        return Enumerable.Range(y1, y2 - y1 + 1);
                    default:
                        throw new Exception("Incorrect expression");
                }
            }).Aggregate((res, next) => res.Concat(next)).ToList();
        }
    }
}
