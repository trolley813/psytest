using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace psytest.Wizard
{
    public class TestWizardStepProvider : IWizardStepProvider
    {
        private readonly ITestCreationRepository testCreationRepository;

        public TestWizardStepProvider(ITestCreationRepository testCreationRepository)
        {
            this.testCreationRepository = testCreationRepository;
        }

        public int GetCurrentStep(HttpContext httpContext)
        {
            string username = httpContext.User.Identity.Name;
            string rawTestCreationId = httpContext.Request.Query["testCreationId"];


            if (!Guid.TryParse(rawTestCreationId, out Guid testCreationId))
            {
                return 0;
            }

            var checkout = testCreationRepository.FindByIdForUser(testCreationId, username);

            if (checkout == null)
            {
                return 0;
            }

            return checkout.CurrentStep;
        }
    }
}
