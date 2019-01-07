using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace psytest.Wizard
{
    public class TestCreationRepository
    {
        private Dictionary<string, TestCreation> testCreations;

        public TestCreationRepository()
        {
            testCreations = new Dictionary<string, TestCreation>();
        }

        public TestCreation StartNew(string userId)
        {
            var testCreation = new TestCreation
            {
                TestCreationId = Guid.NewGuid(),
                UserId = userId,
                CurrentStep = 1
            };

            testCreations.Add(GetKey(testCreation), testCreation);

            return testCreation;
        }

        private string GetKey(TestCreation testCreation)
        {
            return testCreation.TestCreationId.ToString() + testCreation.UserId;
        }

        public TestCreation FindByIdForUser(Guid testCreationId, string userId)
        {
            var key = testCreationId.ToString() + userId;

            if (!testCreations.ContainsKey(key))
            {
                return null;
            }

            return testCreations[key];
        }

        public void MoveNext(TestCreation testCreation)
        {
            testCreation.CurrentStep++;
        }

        public void MovePrevious(TestCreation testCreation)
        {
            testCreation.CurrentStep--;
        }
    }
}
