﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using psytest.Models;

namespace psytest.Migrations
{
    [DbContext(typeof(TestContext))]
    [Migration("20181120092820_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("psytest.Models.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("TestId");

                    b.Property<string>("Text");

                    b.Property<int?>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.HasIndex("TypeId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("psytest.Models.QuestionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("QuestionTypes");

                    b.HasDiscriminator<string>("Discriminator").HasValue("QuestionType");
                });

            modelBuilder.Entity("psytest.Models.QuestionVariant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text");

                    b.Property<int?>("VariantQuestionTypeId");

                    b.HasKey("Id");

                    b.HasIndex("VariantQuestionTypeId");

                    b.ToTable("QuestionVariant");
                });

            modelBuilder.Entity("psytest.Models.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("psytest.Models.SliderQuestionType", b =>
                {
                    b.HasBaseType("psytest.Models.QuestionType");

                    b.Property<int>("MaxValue");

                    b.Property<int>("MinValue");

                    b.ToTable("SliderQuestionType");

                    b.HasDiscriminator().HasValue("SliderQuestionType");
                });

            modelBuilder.Entity("psytest.Models.VariantQuestionType", b =>
                {
                    b.HasBaseType("psytest.Models.QuestionType");


                    b.ToTable("VariantQuestionType");

                    b.HasDiscriminator().HasValue("VariantQuestionType");
                });

            modelBuilder.Entity("psytest.Models.Question", b =>
                {
                    b.HasOne("psytest.Models.Test")
                        .WithMany("Questions")
                        .HasForeignKey("TestId");

                    b.HasOne("psytest.Models.QuestionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("psytest.Models.QuestionVariant", b =>
                {
                    b.HasOne("psytest.Models.VariantQuestionType")
                        .WithMany("Variants")
                        .HasForeignKey("VariantQuestionTypeId");
                });
#pragma warning restore 612, 618
        }
    }
}
