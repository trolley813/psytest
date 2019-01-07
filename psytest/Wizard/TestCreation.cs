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
        public string TestName { get; set; }
        public string TestInstruction { get; set; }
        public int PartCount { get; set; }
        public List<int> QuestionParts { get; set; }
        public List<string> QuestionTexts { get; set; }
    }
}
