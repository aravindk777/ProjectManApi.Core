using Microsoft.EntityFrameworkCore;
using PM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Data.Entities
{
    public class PMDbContext : DbContext
    {
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }

        public PMDbContext(DbContextOptions<PMDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<Users>()
        }
    }
}
