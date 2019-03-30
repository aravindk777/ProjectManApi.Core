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
        private IProjectLogic projectLogicTest;
        #endregion

        public ProjectLogicUnitTests()
        {
            mockProjectRepository = new Mock<IProjectRepo>();
            projectLogicTest = new ProjectLogic(mockProjectRepository.Object);
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

        [Fact(DisplayName = "Test - Get All Projects")]
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
            Models.DataModels.Project projectDMObject;
            if (expectedResult)
                projectDMObject = projectToUpdate.AsDataModel();
            else
                projectDMObject = null;
            mockProjectRepository.Setup(repo => repo.GetById(projectIdToUpdate)).Returns(projectDMObject);
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

        [Theory(DisplayName = "Test - End a Project")]
        [InlineData(3, true)]
        [InlineData(90, false)]
        public void Test_For_End_Project(int projectIdToEnd, bool expectedResult)
        {
            // Arrange
            var projectToEnd = new Models.ViewModels.Project() { ProjectId = projectIdToEnd, ProjectName = "TestProject-XXX", ManagerId = Guid.NewGuid(), Priority = 20 };
            var projectDataModel = expectedResult ? projectToEnd.AsDataModel() : null;
            mockProjectRepository.Setup(repo => repo.GetById(projectIdToEnd)).Returns(projectDataModel);
            mockProjectRepository.Setup(repo => repo.Update(It.IsAny<Models.DataModels.Project>())).Returns(expectedResult);

            // Act
            var actualResult = projectLogicTest.EndProject(projectIdToEnd);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact(DisplayName = "Test - Get Count for Projects")]
        public void Test_For_GetCount_Projects()
        {
            // Arrange
            int expectedCount = 10;
            mockProjectRepository.Setup(repo => repo.Count()).Returns(expectedCount);
            // Act
            var actualCount = projectLogicTest.Count();
            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact(DisplayName = "Test for Get User Projects")]
        public void Test_For_Get_UserProjects()
        {
            // Arrange
            var mgrId = Guid.NewGuid();
            var testUserId = "TestUser1";
            var projectsList = new Models.DataModels.Project[] {
                new Models.DataModels.Project() {ProjectId = 1, ProjectName = "TestProject-1", ManagerId = mgrId, Priority = 10 },
                new Models.DataModels.Project() {ProjectId = 2, ProjectName = "TestProject-2", ManagerId = mgrId, Priority = 5 },
                new Models.DataModels.Project() {ProjectId = 3, ProjectName = "TestProject-3", ManagerId = mgrId, Priority = 15 },
                new Models.DataModels.Project() {ProjectId = 4, ProjectName = "TestProject-4", ManagerId = mgrId, Priority = 20 },
                new Models.DataModels.Project() {ProjectId = 5, ProjectName = "TestProject-5", ManagerId = mgrId, Priority = 30 },
            };
            mockProjectRepository.Setup(repo => repo.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Models.DataModels.Project, bool>>>())).Returns(projectsList);
            // Act
            var actualResult = projectLogicTest.GetUserProjects(testUserId);
            // Assert
            Assert.NotNull(actualResult);
            Assert.IsAssignableFrom<IEnumerable<Models.ViewModels.Project>>(actualResult);
            Assert.Equal(projectsList.Count(), actualResult.Count());
        }

        [Theory(DisplayName = "Test for Search Projects")]
        [InlineData("TestProject", false)]
        [InlineData("Project", false)]
        [InlineData("TestLastName", false)]
        [InlineData("TestProject-2", true)]
        public void Test_For_Search_Projects(string searchText, bool exactMatch)
        {
            // Arrange
            var projectsList = new Models.ViewModels.Project[] {
                new Models.ViewModels.Project() {ProjectId = 1, ProjectName = "TestProject-1", ManagerId = Guid.NewGuid(), Priority = 10 },
                new Models.ViewModels.Project() {ProjectId = 2, ProjectName = "TestProject-2", ManagerId = Guid.NewGuid(), Priority = 5 },
                new Models.ViewModels.Project() {ProjectId = 3, ProjectName = "TestProject-3", ManagerId = Guid.NewGuid(), Priority = 15 },
                new Models.ViewModels.Project() {ProjectId = 4, ProjectName = "TestProject-4", ManagerId = Guid.NewGuid(), Priority = 20 },
                new Models.ViewModels.Project() {ProjectId = 5, ProjectName = "TestProject-5", ManagerId = Guid.NewGuid(), Priority = 30 },
            }.AsEnumerable();

            if (exactMatch)
                projectsList = projectsList.Where(p => p.ProjectName.Contains(searchText));

            mockProjectRepository.Setup(repo => repo.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Models.DataModels.Project, bool>>>())).Returns(projectsList.AsDataModel());
            // Act
            var actualResult = projectLogicTest.Search(searchText, exactMatch);
            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(projectsList.Count(), actualResult.Count());
        }

        [Fact(DisplayName = "Test for Model Converter - AsDataModel list")]
        public void Test_For_Converting_AsDataModel_List()
        {
            // Arrange
            var projectsList = new Models.ViewModels.Project[] {
                new Models.ViewModels.Project() {ProjectId = 1, ProjectName = "TestProject-1", ManagerId = Guid.NewGuid(), Priority = 10 },
                new Models.ViewModels.Project() {ProjectId = 2, ProjectName = "TestProject-2", ManagerId = Guid.NewGuid(), Priority = 5 },
                new Models.ViewModels.Project() {ProjectId = 3, ProjectName = "TestProject-3", ManagerId = Guid.NewGuid(), Priority = 15 },
                new Models.ViewModels.Project() {ProjectId = 4, ProjectName = "TestProject-4", ManagerId = Guid.NewGuid(), Priority = 20 },
                new Models.ViewModels.Project() {ProjectId = 5, ProjectName = "TestProject-5", ManagerId = Guid.NewGuid(), Priority = 30 },
            }.AsEnumerable();

            var testDataModelList = new Models.DataModels.Project[] {
                new Models.DataModels.Project() {ProjectId = 1, ProjectName = "As-Is-Project-1", ManagerId = Guid.NewGuid(), Priority = 1 },
                new Models.DataModels.Project() {ProjectId = 2, ProjectName = "As-Is-Project-2", ManagerId = Guid.NewGuid(), Priority = 2 },
                new Models.DataModels.Project() {ProjectId = 3, ProjectName = "As-Is-Project-3", ManagerId = Guid.NewGuid(), Priority = 3 },
                new Models.DataModels.Project() {ProjectId = 4, ProjectName = "As-Is-Project-4", ManagerId = Guid.NewGuid(), Priority = 4 },
                new Models.DataModels.Project() {ProjectId = 5, ProjectName = "As-Is-Project-5", ManagerId = Guid.NewGuid(), Priority = 5 },
            }.AsEnumerable();

            // Act
            var actualDataModelResult = projectsList.AsDataModel(testDataModelList);

            // Assert
            Assert.Equal(projectsList.Select(t => t.ProjectName), actualDataModelResult.Select(t => t.ProjectName));
            Assert.Equal(projectsList.Select(t => t.Priority), actualDataModelResult.Select(t => t.Priority));
        }
    }
}
