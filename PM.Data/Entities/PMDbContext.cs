using Microsoft.EntityFrameworkCore;
using PM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Data.Entities
{
    public class PMDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }

        public PMDbContext(DbContextOptions<PMDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Project>()
                .HasOne(u => u.Manager)
                .WithMany()
                .HasForeignKey(fk => fk.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Task>()
                .HasOne(t => t.Project)
                .WithMany()
                .HasForeignKey(fk => fk.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Task>()
                .HasOne(t => t.TaskOwner)
                .WithMany()
                .HasForeignKey(fk => fk.TaskOwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            //builder.Entity<Users>()
        }
    }
}
