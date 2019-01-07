using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using psytest.Wizard;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace psytest.Controllers
{
    public class TestCreationController : Controller
    {
        private readonly ITestCreationRepository testCreationRepository;

        public TestCreationController(ITestCreationRepository testCreationRepository) => 
            this.testCreationRepository = testCreationRepository;


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

            return View("Create1");
        }

        [WizardStep(1)]
        [ActionName("Create")]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateStep1Post(Guid testCreationId, string go)
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

            if (go == "Finish")
            {
                testCreationRepository.MoveNext(testCreation);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                testCreationRepository.MovePrevious(testCreation);
                return RedirectToAction("Create", new { testCreationId });
            }

            
        }
    }
}
