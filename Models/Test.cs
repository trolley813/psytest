using System;
using System.Collections.Generic;

namespace psytest.Models
{
    public class Test
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public List<Question> Questions { get; set; }

        // JS code
        public String MetricsComputeScript { get; set; }

        public Dictionary<String, String> MetricsDescriptions { get; set; }
    }
}