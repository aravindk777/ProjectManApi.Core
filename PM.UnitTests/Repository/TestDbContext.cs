using Microsoft.EntityFrameworkCore;
using Moq;
using PM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM.UnitTests.Repository
{
    public class TestDbContext: DbContext
    {
        public virtual Mock<DbSet<Task>> MockTasks { get; set; }
        public virtual Mock<DbSet<User>> MockUsers { get; set; }
        public virtual Mock<DbSet<Project>> MockProjects { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options): base(options)
        {

        }

        
    }
}
