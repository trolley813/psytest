using System;
using System.Collections.Generic;

namespace psytest.Models
{
    public class Test
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public List<Question> Questions { get; set; }
    }
}