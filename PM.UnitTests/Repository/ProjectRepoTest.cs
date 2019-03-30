using PM.Data.Repos.Projects;
using PM.Models.DataModels;
using System;
using System.Linq;
using Xunit;

namespace PM.UnitTests.Repository
{
    public class ProjectRepoTest : RepoTestSetup
    {
        private IProjectRepo testingProjectRepo;
        public ProjectRepoTest() : base()
        {
            InitializeProjectContext();
            testingProjectRepo = new ProjectRepo(mockContext);
        }
        void InitializeProjectContext()
        {
            //base.InitializeContext();
            var testProjects = Enumerable.Range(1, 10)
                                .Select(counter => new Project
                                {
                                    ProjectId = counter,
                                    ManagerId = Guid.NewGuid(),
                                    ProjectStart = DateTime.Today,
                                    ProjectName = $"TestProject{counter}",
                                    Priority = counter
                                }).AsQueryable();

            //var builder = new DbContextOptionsBuilder<PMDbContext>().UseInMemoryDatabase("TestPMDb");
            //var context = new PMDbContext(builder.Options);
            //context.Projects.AddRange(testProjects);
            mockContext.Projects.AddRange(testProjects);
            mockContext.SaveChanges(true);
        }

        [Fact]
        public void Test_For_GetCount()
        {
            var expectedCount = (mockContext).Projects.Count();
            var actualCount = testingProjectRepo.Count();
            Assert.Equal(expectedCount, actualCount);
        }

        [Theory(DisplayName = "Test for Delete Repo method With No Exception")]
        [InlineData(10, true)]
        public void Test_For_DeleteEntity_Valid(int testProjectId, bool expectedResult)
        {
            var testProjectToDelete = testingProjectRepo.GetById(testProjectId);
            var actualResult = testingProjectRepo.Delete(testProjectToDelete);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact(DisplayName = "Test for Delete Repo method Throws Exception")]
        public void Test_For_DeleteEntity_Throws_Exception()
        {
            int testProjectId = 30;
            var testProjectToDelete = testingProjectRepo.GetById(testProjectId);
            Assert.ThrowsAny<Exception>(() => testingProjectRepo.Delete(testProjectToDelete));
        }

        [Fact(DisplayName = "Test for Update Entity returns success")]
        public void Test_For_UpdateEntity_Returns_Success()
        {
            var projectToUpdate = mockContext.Projects.FirstOrDefault();
            projectToUpdate.Priority = 20;
            projectToUpdate.ProjectEnd = DateTime.Today.AddMonths(1);

            var actualResult = testingProjectRepo.Update(projectToUpdate);
            Assert.True(actualResult);
        }

        [Fact(DisplayName = "Test for Update Entity throws Exception")]
        public void Test_For_UpdateEntity_Throws_Exception()
        {
            var projectToUpdate = mockContext.Projects.LastOrDefault();
            projectToUpdate.ProjectId = 5;
            Assert.ThrowsAny<Exception>(() => testingProjectRepo.Update(projectToUpdate));            
        }
    }
}
