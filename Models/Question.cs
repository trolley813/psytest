using System;

namespace psytest.Models
{
    public class Question
    {
        public int Id { get; set; }
        public QuestionType Type { get; set; }
        public String Text { get; set; }
    }
}