using System;
using System.Collections.Generic;
using System.Linq;

namespace psytest.Wizard
{
    public struct Metric
    {
        public String Name { get; set; }
        public List<int> DirectQuestions { get; set; }
        public List<int> InverseQuestions { get; set; }
        public String AdditionalComputeExpression { get; set; }
    }

    public class TestCreation
    {
        public Guid TestCreationId { get; set; }

        public string UserId { get; set; }

        public int CurrentStep { get; set; }

        // TODO: Parts, question type, question texts, metrics, etc.
        public string TestName { get; set; }
        public string TestInstruction { get; set; }
        public int PartCount { get; set; }
        public int MetricCount { get; set; }
        public List<int> QuestionParts
        {
            get
            {
                return CountByPart.Select((val, idx) => Enumerable.Repeat(idx + 1, val))
                    .Aggregate((res, next) => res.Concat(next))
                    .ToList();
            }
        }

        public int QuestionCount
        {
            get
            {
                return CountByPart.Sum();
            }
        }

        public List<int> CountByPart { get; set; }
        public List<string> QuestionTexts { get; set; }
        public List<Metric> Metrics { get; set; }
    }
}
