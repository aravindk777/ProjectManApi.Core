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
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            Projects.Include(m => m.Manager).Load();
            Tasks.Include(p => p.Project).Load();
            Tasks.Include(t => t.ParentTask).Load();
            Tasks.Include(u => u.TaskOwner).Load();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>()
                .HasOne(m => m.Manager)
                .WithMany(p => p.Projects)
                .HasForeignKey(fk => fk.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Task>()
                .HasOne(t => t.Project)
                .WithMany(t => t.Tasks)
                .HasForeignKey(fk => fk.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Task>()
                .HasOne(t => t.TaskOwner)
                .WithMany(t => t.Tasks)
                .HasForeignKey(fk => fk.TaskOwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull);


        }
    }
}
