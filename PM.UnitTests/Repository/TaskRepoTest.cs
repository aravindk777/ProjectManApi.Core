using Microsoft.EntityFrameworkCore;
using PM.Data.Entities;
using PM.Data.Repos.Tasks;
using PM.Models.DataModels;
using System;
using System.Linq;
using Xunit;

namespace PM.UnitTests.Repository
{
    public class TaskRepoTest : RepoTestSetup
    {
        private ITaskRepository testingTaskRepo;
        public TaskRepoTest() : base()
        {
            InitializeTaskContext();
            testingTaskRepo = new TaskRepository(mockContext);
        }

        void InitializeTaskContext()
        {
            var testTasks = Enumerable.Range(1, 10)
                            .Select(counter => new Task
                            {
                                TaskId = counter,
                                ParentTaskId = (counter % 2) * 2,
                                ProjectId = counter,
                                TaskOwnerId = Guid.NewGuid(),
                                StartDate = DateTime.Today,
                                TaskName = $"TestTask{counter}",
                                Priority = counter
                            }).AsQueryable();
            mockContext.Tasks.AddRange(testTasks);
            mockContext.SaveChanges(true);            
        }

        [Theory(DisplayName = "Test for End Task")]
        [InlineData(4, true)]
        [InlineData(21, false)]
        public void Test_For_EndTask(int testTaskIdToEnd, bool expectedResult)
        {
            var actualResult = testingTaskRepo.EndTask(testTaskIdToEnd);

            Assert.Equal(expectedResult, actualResult);
        }

        [Theory(DisplayName = "Test for Exists method")]
        [InlineData(9, true)]
        [InlineData(21, false)]
        public void Test_For_Exists(int testTaskToCheck, bool expectedResult)
        {
            var actualResult = testingTaskRepo.Exists(testTaskToCheck);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void Test_For_GetCount()
        {
            var expectedCount = (mockContext).Tasks.Count();
            var actualCount = testingTaskRepo.Count();
            Assert.Equal(expectedCount, actualCount);
        }
    }
}
