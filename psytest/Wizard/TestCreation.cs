using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace psytest.Wizard
{
    public class TestCreation
    {
        public Guid TestCreationId { get; set; }

        public string UserId { get; set; }

        public int CurrentStep { get; set; }

        // TODO: Parts, question type, question texts, metrics, etc.
    }
}
