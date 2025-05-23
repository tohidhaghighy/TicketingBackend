﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ticketing.Infrastructure.Database;

#nullable disable

namespace Ticketing.Infrastructure.Migrations
{
    [DbContext(typeof(TicketingDbContext))]
    [Migration("20250108115832_CreateTicketing")]
    partial class CreateTicketing
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Ticketing.Domain.Entities.Massage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FilePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("InsertDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TicketId");

                    b.ToTable("Massages");
                });

            modelBuilder.Entity("Ticketing.Domain.Entities.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Projects");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "سامانه مدیریت پرونده ها"
                        },
                        new
                        {
                            Id = 2,
                            Name = "سامانه میز خدمت"
                        },
                        new
                        {
                            Id = 3,
                            Name = "سامانه امحا"
                        },
                        new
                        {
                            Id = 4,
                            Name = "سامانه تبادل اطلاعات"
                        },
                        new
                        {
                            Id = 5,
                            Name = "سامانه هوش تجاری"
                        },
                        new
                        {
                            Id = 6,
                            Name = "زیر ساخت"
                        });
                });

            modelBuilder.Entity("Ticketing.Domain.Entities.ProjectRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ProjectRoles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ProjectId = 1,
                            RoleId = 2
                        },
                        new
                        {
                            Id = 2,
                            ProjectId = 2,
                            RoleId = 3
                        },
                        new
                        {
                            Id = 3,
                            ProjectId = 3,
                            RoleId = 6
                        },
                        new
                        {
                            Id = 4,
                            ProjectId = 4,
                            RoleId = 7
                        });
                });

            modelBuilder.Entity("Ticketing.Domain.Entities.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Statuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "انجام شده"
                        },
                        new
                        {
                            Id = 2,
                            Name = "جدید"
                        },
                        new
                        {
                            Id = 3,
                            Name = "ارجاع به ویرا"
                        },
                        new
                        {
                            Id = 4,
                            Name = "ردشده"
                        },
                        new
                        {
                            Id = 5,
                            Name = "بازگشت از ویرا"
                        },
                        new
                        {
                            Id = 6,
                            Name = "انجام شد در انتظار تایید"
                        },
                        new
                        {
                            Id = 7,
                            Name = "در صف انجام پردازش"
                        },
                        new
                        {
                            Id = 8,
                            Name = "در حال انجام"
                        },
                        new
                        {
                            Id = 9,
                            Name = "رد شده در انتظار تایید"
                        });
                });

            modelBuilder.Entity("Ticketing.Domain.Entities.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CloseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CurrentRoleId")
                        .HasColumnType("int");

                    b.Property<int>("DeveloperId")
                        .HasColumnType("int");

                    b.Property<int?>("ExcelRow")
                        .HasColumnType("int");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("InsertDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("InsertedRoleId")
                        .HasColumnType("int");

                    b.Property<int>("IsSchedule")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastChangeDatetime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ProcessEndDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int?>("RequestTypeId")
                        .HasColumnType("int");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TicketNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TicketRowNumber")
                        .HasColumnType("int");

                    b.Property<string>("TicketTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("StatusId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Ticketing.Domain.Entities.TicketFlow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CurrentRoleId")
                        .HasColumnType("int");

                    b.Property<DateTime>("InsertDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PreviousRoleId")
                        .HasColumnType("int");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TicketFlows");
                });

            modelBuilder.Entity("Ticketing.Domain.Entities.Massage", b =>
                {
                    b.HasOne("Ticketing.Domain.Entities.Ticket", null)
                        .WithMany()
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Ticketing.Domain.Entities.Ticket", b =>
                {
                    b.HasOne("Ticketing.Domain.Entities.Status", null)
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
