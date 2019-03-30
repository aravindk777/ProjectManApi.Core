using Microsoft.EntityFrameworkCore;
using PM.Data.Entities;
using PM.Models.DataModels;
using System;
using System.Linq;

namespace PM.UnitTests.Repository
{
    public abstract class RepoTestSetup : IDisposable
    {
        //public Mock<PMDbContext> mockContext;
        public PMDbContext mockContext;

        public RepoTestSetup()
        {
            InitializeContext();
        }

        public void Dispose()
        {
            if (mockContext.Database.EnsureDeleted())
                mockContext.Dispose();
        }

        public virtual PMDbContext InitializeContext()
        {
            //var builder = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestPMDb");
            //var cxt = new TestDbContext(builder.Options);

            if (mockContext == null)
            {
                //var mockUserSet = new Mock<DbSet<User>>();            
                //var testUsers = Enumerable.Range(1, 10)
                //                .Select(counter => new User
                //                {
                //                    Id = Guid.NewGuid(),
                //                    UserId = $"TestUser{counter}",
                //                    Created = DateTime.Today,
                //                    FirstName = $"User{counter}FirstName",
                //                    LastName = $"User{counter}LastName"
                //                }).AsQueryable();
                //cxt.MockUsers.As<IQueryable<User>>().Setup(u => u.Provider).Returns(testUsers.Provider);
                //cxt.MockUsers.As<IQueryable<User>>().Setup(u => u.Expression).Returns(testUsers.Expression);
                //cxt.MockUsers.As<IQueryable<User>>().Setup(u => u.ElementType).Returns(testUsers.ElementType);
                //cxt.MockUsers.As<IQueryable<User>>().Setup(u => u.GetEnumerator()).Returns(testUsers.GetEnumerator());
                //mockUserSet.As<IQueryable<User>>().Setup(u => u.Provider).Returns(testUsers.Provider);
                //mockUserSet.As<IQueryable<User>>().Setup(u => u.Expression).Returns(testUsers.Expression);
                //mockUserSet.As<IQueryable<User>>().Setup(u => u.ElementType).Returns(testUsers.ElementType);
                //mockUserSet.As<IQueryable<User>>().Setup(u => u.GetEnumerator()).Returns(testUsers.GetEnumerator());

                //var mockProjectSet = new Mock<DbSet<Project>>();
                //var testProjects = Enumerable.Range(1, 10)
                //                .Select(counter => new Project
                //                {
                //                    ProjectId = counter,
                //                    ManagerId = Guid.NewGuid(),
                //                    ProjectStart = DateTime.Today,
                //                    ProjectName = $"TestProject{counter}",
                //                    Priority = counter
                //                }).AsQueryable();
                //cxt.MockProjects.As<IQueryable<Project>>().Setup(u => u.Provider).Returns(testProjects.Provider);
                //cxt.MockProjects.As<IQueryable<Project>>().Setup(u => u.Expression).Returns(testProjects.Expression);
                //cxt.MockProjects.As<IQueryable<Project>>().Setup(u => u.ElementType).Returns(testProjects.ElementType);
                //cxt.MockProjects.As<IQueryable<Project>>().Setup(u => u.GetEnumerator()).Returns(testProjects.GetEnumerator());
                //mockProjectSet.As<IQueryable<Project>>().Setup(u => u.Provider).Returns(testProjects.Provider);
                //mockProjectSet.As<IQueryable<Project>>().Setup(u => u.Expression).Returns(testProjects.Expression);
                //mockProjectSet.As<IQueryable<Project>>().Setup(u => u.ElementType).Returns(testProjects.ElementType);
                //mockProjectSet.As<IQueryable<Project>>().Setup(u => u.GetEnumerator()).Returns(testProjects.GetEnumerator());

                //var mockTaskSet = new Mock<DbSet<Task>>();
                //var testTasks = Enumerable.Range(1, 10)
                //                .Select(counter => new Task
                //                {
                //                    TaskId = counter,
                //                    ParentTaskId = (counter % 2) * 2,
                //                    ProjectId = counter,
                //                    TaskOwnerId = Guid.NewGuid(),
                //                    StartDate = DateTime.Today,
                //                    TaskName = $"TestTask{counter}",
                //                    Priority = counter
                //                }).AsQueryable();
                //mockTaskSet.As<IQueryable<Task>>().Setup(u => u.Provider).Returns(testTasks.Provider);
                //mockTaskSet.As<IQueryable<Task>>().Setup(u => u.Expression).Returns(testTasks.Expression);
                //mockTaskSet.As<IQueryable<Task>>().Setup(u => u.ElementType).Returns(testTasks.ElementType);
                //mockTaskSet.As<IQueryable<Task>>().Setup(u => u.GetEnumerator()).Returns(testTasks.GetEnumerator());

                var builder = new DbContextOptionsBuilder<PMDbContext>().UseInMemoryDatabase("TestPMDb");
                //mockContext = new Mock<PMDbContext>(builder.Options);
                mockContext = new PMDbContext(builder.Options);
                //mockContext.Users.AddRange(testUsers); //mockUserSet.Object;
                //mockContext.Projects.AddRange(testProjects); //mockProjectSet.Object;
                //mockContext.Tasks.AddRange(testTasks); //mockTaskSet.Object;
                //mockContext.SaveChanges(true);
                //mockContext.Setup(u => u.Users).Returns(mockUserSet.Object);
                //mockContext.Setup(p => p.Projects).Returns(mockProjectSet.Object);
                //mockContext.Setup(t => t.Tasks).Returns(mockTaskSet.Object);
            }
            return mockContext;
        }
    }
}
