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

    public class VariantQuestionType : QuestionType
    {
        public List<QuestionVariant> Variants { get; set; }
    }

    public class QuestionVariant
    {
        public int Id { get; set; }
        public String Text { get; set; }
    }
}