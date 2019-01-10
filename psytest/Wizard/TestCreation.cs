using psytest.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace psytest.Wizard
{
    public struct QuestionTypeProxy
    {
        public string Type { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public String Variants { get; set; }

        public QuestionType GetQuestionType()
        {
            switch (Type)
            {
                case "slider":
                    return new SliderQuestionType { MinValue = MinValue, MaxValue = MaxValue };
                case "antagonistic":
                    return new AntagonisticQuestionType { MinValue = MinValue, MaxValue = MaxValue };
                case "variant":
                    return new VariantQuestionType { Variants = Variants.Split("\n").ToList() };
                default:
                    throw new Exception("Unknown question type");
            }
        }
    }

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
        public QuestionTypeProxy QuestionType { get; set; }
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

    static class TestGenerator
    {
        public static void GenerateTest(TestCreation testCreation, TestContext testContext)
        {
            QuestionType questionType = testCreation.QuestionType.GetQuestionType();
            int qTypeSubtrahend = 0;
            switch (questionType)
            {
                case SliderQuestionType sqt:
                    qTypeSubtrahend = sqt.MinValue + sqt.MaxValue;
                    break;
                case AntagonisticQuestionType aqt:
                    qTypeSubtrahend = aqt.MinValue + aqt.MaxValue;
                    break;
                case VariantQuestionType vqt:
                    qTypeSubtrahend = vqt.Variants.Count + 1;
                    break;
            }
            Test test = new Test { Name = testCreation.TestName, Instruction = testCreation.TestInstruction };
            int count = testCreation.QuestionCount;
            List<Question> questions = new List<Question>();
            for (int i = 0; i < count; i++)
            {
                Question q = new Question {
                    Test = test,
                    Number = i + 1,
                    Type = questionType,
                    Part = testCreation.QuestionParts[i],
                    Text = testCreation.QuestionTexts[i]
                };
                questions.Append(q);
            }
            string metricScript = "";
            Dictionary<string, string> metricsDescriptions = new Dictionary<string, string>();
            for (int i = 0; i < testCreation.MetricCount; i++)
            {
                metricsDescriptions[$"m{i + 1}"] = testCreation.Metrics[i].Name;
                metricScript += $"var X =";
                foreach (var q in testCreation.Metrics[i].DirectQuestions)
                {
                    metricScript += $" + questions[{q}]";
                }
                foreach (var q in testCreation.Metrics[i].DirectQuestions)
                {
                    metricScript += $" + {qTypeSubtrahend} - questions[{q}]";
                }
                var trans = testCreation.Metrics[i].AdditionalComputeExpression;
                if (trans == "") trans = "X"; 
                metricScript += $";\n" +
                    $"metrics[\"m{i + 1}\"] = {trans};\n";
            }
            test.MetricsDescriptions = metricsDescriptions;
            test.MetricsComputeScript = metricScript;
            test.Questions = questions;

            testContext.Add(questionType);
            testContext.Add(test);
            foreach(var q in questions) testContext.Add(q);
        }
    }
}
