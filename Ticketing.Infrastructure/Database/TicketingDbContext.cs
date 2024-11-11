﻿using Microsoft.EntityFrameworkCore;
using Ticketing.Domain.Entities;
using Ticketing.Infrastructure.EntityTypeConfiguration;

namespace Ticketing.Infrastructure.Database;

public class TicketingDbContext : DbContext
{
    public TicketingDbContext(DbContextOptions<TicketingDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Customer>().OwnsOne(p => p.CustomerInfo).Property(x=>x.FirstName).HasColumnType("varchar(50)");
        //new BlogEntityTypeConfiguration().Configure(modelBuilder.Entity<Blog>());
        modelBuilder.Entity<Project>().HasData(
           new Project
           {
               Id = 1,
               Name = "سامانه مدیریت پرونده ها",
           }, new Project
           {
               Id = 2,
               Name = "سامانه میز خدمت",
           }, new Project
           {
               Id = 3,
               Name = "سامانه امحا",
           }, new Project
           {
               Id = 4,
               Name = "سامانه تبادل اطلاعات",
           }, new Project
           {
               Id = 5,
               Name = "سامانه هوش تجاری",
           }, new Project
           {
               Id = 6,
               Name = "زیر ساخت",
           }
       );

        modelBuilder.Entity<ProjectRole>().HasData(
          new ProjectRole
          {
              Id = 1,
              RoleId = 2,
              ProjectId = 1,
          }, new ProjectRole
          {
              Id = 2,
              RoleId = 3,
              ProjectId = 2,
          }, new ProjectRole
          {
              Id = 3,
              RoleId = 6,
              ProjectId = 3,
          }, new ProjectRole
          {
              Id = 4,
              RoleId = 7,
              ProjectId = 4,
          }
      );

        modelBuilder.Entity<Status>().HasData(
          new Status
          {
              Id = 1,
              Name = "انجام شده"
          }, new Status
          {
              Id = 2,
              Name = "جدید"
          }, new Status
          {
              Id = 3,
              Name = "ارجاع به ویرا"
          }, new Status
          {
              Id = 4,
              Name = "ردشده"
          }, new Status
          {
              Id = 5,
              Name = "بازگشت از ویرا"
          }, new Status
          {
              Id = 6,
              Name = "انجام شد در انتظار تایید"
          }, new Status
          {
              Id = 7,
              Name = "در صف انجام پردازش"
          }, new Status
          {
              Id = 8,
              Name = "در حال انجام"
          }, new Status
          {
              Id = 9,
              Name = "رد شده در انتظار تایید"
          }
      );
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectEntityTypeConfiguration).Assembly);
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }
    public DbSet<Massage> Massages { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketFlow> TicketFlows { get; set; }
}