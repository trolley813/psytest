using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace psytest.Wizard
{
    public interface ITestCreationRepository
    {
        TestCreation StartNew(string userId);

        TestCreation FindByIdForUser(Guid testCreationId, string userId);

        void MoveNext(TestCreation testCreation);
        void MovePrevious(TestCreation testCreation);
    }
}
