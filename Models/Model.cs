using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace psytest.Models
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
                    : base(options)
        { }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SliderQuestionType>();
            builder.Entity<VariantQuestionType>();

            base.OnModelCreating(builder);
        }
    }
}