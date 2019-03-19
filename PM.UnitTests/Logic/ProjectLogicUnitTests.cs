using Moq;
using PM.BL.Common;
using PM.BL.Projects;
using PM.Data.Repos.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace PM.UnitTests.Logic
{
    public class ProjectLogicUnitTests
    {
        #region SETUP
        private Mock<IProjectRepo> mockProjectRepository;
        private Mock<ICommonLogic> mockCommonRepository;
        private IProjectLogic projectLogicTest;
        #endregion

        public ProjectLogicUnitTests()
        {
            mockProjectRepository = new Mock<IProjectRepo>();
            mockCommonRepository = new Mock<ICommonLogic>();
            projectLogicTest = new ProjectLogic(mockProjectRepository.Object);
        }

        void Test_For_GetCount()
        {
            // Arrange
            mockProjectRepository.Setup(repo => repo.Count()).Returns(10);

            // Act
            //var actualResponse = projectLogicTest.
        }

        [Fact(DisplayName = "Test - CreateProject Logic")]
        public void Test_For_CreateProject()
        {
            // Arrange
            var newProjectViewModel = new Models.ViewModels.Project() { ProjectId = 1, ProjectName = "TestProject-1", ManagerId = Guid.NewGuid(), ManagerName = "TestUser", Priority = 10 };
            var newProjectDataModel = newProjectViewModel.AsDataModel();
            mockProjectRepository.Setup(repo => repo.Create(It.IsAny<Models.DataModels.Project>())).Returns(newProjectDataModel);

            // Act
            var actualResponse = projectLogicTest.CreateProject(newProjectViewModel);

            // Assert
            Assert.NotNull(actualResponse);
            Assert.Equal(newProjectViewModel.ProjectName, actualResponse.ProjectName);
        }

        [Fact(DisplayName ="Test - Get All Projects")]
        public void Test_For_GetAllProjects()
        {
            // Arrange
            var projectsList = new Models.ViewModels.Project[] {
                new Models.ViewModels.Project() {ProjectId = 1, ProjectName = "TestProject-1", ManagerId = Guid.NewGuid(), Priority = 10 },
                new Models.ViewModels.Project() {ProjectId = 2, ProjectName = "TestProject-2", ManagerId = Guid.NewGuid(), Priority = 5 },
                new Models.ViewModels.Project() {ProjectId = 3, ProjectName = "TestProject-3", ManagerId = Guid.NewGuid(), Priority = 15 },
                new Models.ViewModels.Project() {ProjectId = 4, ProjectName = "TestProject-4", ManagerId = Guid.NewGuid(), Priority = 20 },
                new Models.ViewModels.Project() {ProjectId = 5, ProjectName = "TestProject-5", ManagerId = Guid.NewGuid(), Priority = 30 },
            }.AsEnumerable();
            var projectsListDataModel = projectsList.AsDataModel();
            mockProjectRepository.Setup(repo => repo.GetAll()).Returns(projectsListDataModel);

            // Act
            var actualResult = projectLogicTest.GetAllProjects();

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(projectsList.Count(), actualResult.Count());
        }

        [Fact(DisplayName = "Test - Get Project By Id")]
        public void Test_GetProject_By_Id()
        {
            // Arrange
            int projectIdToGet = 10;
            var projectToGet = new Models.ViewModels.Project() { ProjectId = projectIdToGet, ProjectName = "TestProject-10", ManagerId = Guid.NewGuid(), ManagerName = "", Priority = 10 };
            var getProjectDataModel = projectToGet.AsDataModel();
            mockProjectRepository.Setup(repo => repo.GetById(projectIdToGet)).Returns(getProjectDataModel);

            // Act
            var actualResult = projectLogicTest.GetProject(projectIdToGet);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(projectToGet.ProjectName, actualResult.ProjectName);
        }

        [Theory(DisplayName = "Test - Update Project")]
        [InlineData(5, true)]
        [InlineData(50, false)]
        public void Test_For_Update_Project_Returns_True(int projectIdToUpdate, bool expectedResult)
        {
            // Arrange
            var projectToUpdate = new Models.ViewModels.Project() { ProjectId = projectIdToUpdate, ProjectName = "TestProject-10", ManagerId = Guid.NewGuid(), ManagerName = "TestUser5", Priority = 10 };
            mockProjectRepository.Setup(repo => repo.GetById(projectIdToUpdate)).Returns(projectToUpdate.AsDataModel());
            mockProjectRepository.Setup(repo => repo.Update(It.IsAny<Models.DataModels.Project>())).Returns(expectedResult);

            // Act
            var actualResult = projectLogicTest.Modify(projectIdToUpdate, projectToUpdate);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory(DisplayName = "Test - Delete Project")]
        [InlineData(25, true)]
        [InlineData(90, false)]
        public void Test_For_Delete_Project(int projectIdToDelete, bool expectedResult)
        {
            // Arrange
            var projectToDelete = new Models.ViewModels.Project() { ProjectId = projectIdToDelete, ProjectName = "TestProject-25", ManagerId = Guid.NewGuid(), ManagerName = "TestUser15", Priority = 20 };
            var projectDataModel = expectedResult ? projectToDelete.AsDataModel() : null;
            mockProjectRepository.Setup(repo => repo.GetById(projectIdToDelete)).Returns(projectDataModel);
            mockProjectRepository.Setup(repo => repo.Delete(projectDataModel)).Returns(expectedResult);

            // Act
            var actualResult = projectLogicTest.Remove(projectIdToDelete);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
