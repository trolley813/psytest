using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psytest.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Required]
        public QuestionType Type { get; set; }
        [Required]
        public String Text { get; set; }

        [Required]
        public Test Test {get; set; }
    }
}