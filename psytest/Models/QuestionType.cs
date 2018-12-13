using System;
using System.Collections.Generic;

namespace psytest.Models
{
    public abstract class QuestionType
    {
        public int Id { get; set; }
    }

    public class SliderQuestionType : QuestionType
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }

    public class AntagonisticQuestionType: QuestionType
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }

    public class VariantQuestionType : QuestionType
    {
        public List<String> Variants { get; set; }
    }
}