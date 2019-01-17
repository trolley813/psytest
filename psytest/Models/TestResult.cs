using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace psytest.Models
{
    public class TestResult
    {
        public int Id { get; set; }

        [Required]
        public String UserId { get; set; }
        [Required]
        public int TestId { get; set; }
        [Required]
        public Dictionary<String, Object> Metrics { get; set; }
        [Required]
        public DateTime TestingDate { get; set; }
    }
}
