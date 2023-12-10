﻿// <auto-generated />
using System;
using CodePulse.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CodePulse.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231209194807_UpdatedSearchTable")]
    partial class UpdatedSearchTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CodePulse.Models.Domain.Search", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("keyword")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("result")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("searchs");
                });
#pragma warning restore 612, 618
        }
    }
}