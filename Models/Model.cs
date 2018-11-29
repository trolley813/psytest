using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Newtonsoft.Json;

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
            builder.Entity<VariantQuestionType>().Property(qt => qt.Variants).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<String>>(v)
            );

            builder.Entity<Test>().HasMany(t => t.Questions).WithOne(q => q.Test);
            builder.Entity<Test>().Property(t => t.MetricsDescriptions).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<String, String>>(v)
            );

            base.OnModelCreating(builder);
        }
    }
}