using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PM.Api.Controllers;
using PM.BL.Common;
using PM.BL.Projects;
using PM.BL.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace PM.UnitTests.Controllers
{
    public class ProjectsApiTest
    {
        #region Test data for Setup
        private Mock<IProjectLogic> mockProjectsLogic;
        private Mock<ITaskLogic> mockTaskLogic;
        private Mock<ILogger<ProjectsController>> loggerInstance;
        private ProjectsController mockController;
        private List<Models.ViewModels.Project> mockProjectsList = new List<Models.ViewModels.Project>() { };
        #endregion

        #region SETUP
        public ProjectsApiTest()
        {
            mockProjectsLogic = new Mock<IProjectLogic>();
            mockTaskLogic = new Mock<ITaskLogic>();
            loggerInstance = new Mock<ILogger<ProjectsController>>();
            mockController = new ProjectsController(mockProjectsLogic.Object, loggerInstance.Object, mockTaskLogic.Object);

            // Create mock users
            mockProjectsList = new List<Models.ViewModels.Project>();
            for (int iCounter = 0; iCounter < 10; iCounter++)
            {
                mockProjectsList.Add(
                (new Models.DataModels.Project
                {
                    ProjectId = iCounter +1, 
                    ProjectName = $"TestProject-{iCounter + 1}",
                    Priority = (iCounter + 1),
                    ProjectStart = DateTime.Now.AddMonths(iCounter - 5),
                    ProjectEnd = DateTime.Today.AddDays(iCounter % 2),
                    ManagerId = Guid.NewGuid()
                }).AsViewModel());
            }
        }
        #endregion

        #region Get All Projects
        [Fact(DisplayName = "Get All Projects - success result")]
        //[TestCase(TestName = "Test for Get All Projects Returns Valid Results")]
        public void Test_GetAll_Projects()
        {
            // Arrange
            mockProjectsLogic.Setup(u => u.GetAllProjects()).Returns(mockProjectsList);

            // Act
            var result = mockController.Get();
            var actualResult = ((OkObjectResult)result).Value as IEnumerable<Models.ViewModels.Project>;

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotEmpty(actualResult);
            Assert.Equal(actualResult.Count(), mockProjectsList.Count());
        }

        [Fact(DisplayName = "Get All Projects - Throws Exception")]
        //[TestCase(TestName = "Test for Get All Projects Throwing Exception")]
        public void Test_GetAllUsers_Throws_Exception()
        {
            // Arrange
            var expectedErrMsg = "Test for Exception";
            mockProjectsLogic.Setup(u => u.GetAllProjects()).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.Get();
            var actualResult = (ObjectResult)result;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(expectedErrMsg, actualResult.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResult.StatusCode);
        }
        #endregion

        #region GET
        [Fact(DisplayName = "Get Project By Id returns Ok result")]
        //[TestCase(1, TestName = "Test for Get method by Id - valid scenario")]
        public void Test_Get_ProjectById_validUsecase()
        {
            // Arrange
            int testProjectId = 1;
            var validMockData = mockProjectsList.FirstOrDefault(p => p.ProjectId == testProjectId);
            mockProjectsLogic.Setup(u => u.GetProject(testProjectId)).Returns(validMockData);

            // Act
            var result = mockController.Get(testProjectId);
            var actualResult = ((OkObjectResult)result).Value as Models.ViewModels.Project;

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsType<Models.ViewModels.Project>(actualResult);
            Assert.Same(validMockData, actualResult);
        }

        [Fact(DisplayName = "Get Project By Id - throws exception")]
        //[TestCase(5, TestName = "Test for Get User Http method - throws Exception")]
        public void Test_GetProject_By_Id_ThrowsException()
        {
            // Arrange
            int testProjectId = 5;
            var expectedErrMsg = "Db connection failure test";
            mockProjectsLogic.Setup(u => u.GetProject(testProjectId)).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.Get(testProjectId);
            var actualResult = ((ObjectResult)result);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResult.StatusCode);
            Assert.Equal(expectedErrMsg, actualResult.Value);
        }
        #endregion

        #region POST
        [Fact(DisplayName = "Test for Create Project returns Valid result")]
        //[TestCase(Description = "Create a new Project", TestName = "Test for Create Project returns Valid result")]
        public void Test_Post_NewProject_Valid()
        {
            // Arrange
            var newTestProject = new Models.ViewModels.Project() { ProjectId = 20, ProjectName = "Test New Project", ManagerId = Guid.NewGuid(), Priority = 20, ProjectStart = DateTime.Today };
            var expectedTestResult = new CreatedResult(string.Concat("/", newTestProject.ProjectId), newTestProject);
            mockProjectsLogic.Setup(api => api.CreateProject(newTestProject)).Returns(newTestProject);

            // Act
            var actualResult = mockController.Post(newTestProject);
            var actualProjData = ((CreatedResult)actualResult).Value as Models.ViewModels.Project;

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsType<CreatedResult>(actualResult);
            Assert.Equal(newTestProject, actualProjData);
        }

        [Fact(DisplayName = "Test Create Invalid Project returns BadRequest result")]
        //[TestCase(Description = "Create a new Project", TestName = "Test Create Invalid Project returns BadRequest result")]
        public void Test_Post_New_Project_InValid()
        {
            // Arrange
            var newTestProject = new Models.ViewModels.Project() { ProjectId = 25, ProjectName = "Test New Project", Priority = 20 };
            var expectedErrMsg = "Invalid request information. Please verify the information entered.";
            mockController.ModelState.AddModelError("ManagerId", expectedErrMsg);
                //.AddModelError("ManagerId", expectedErrMsg);
            mockProjectsLogic.Setup(api => api.CreateProject(newTestProject));

            // Act
            var actualResult = mockController.Post(newTestProject);
            var actualData = (BadRequestObjectResult) actualResult;
            var actualModelState = (SerializableError)actualData.Value;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status400BadRequest, actualData.StatusCode);
            Assert.Contains("ManagerId", actualModelState.Keys);
            Assert.Contains(expectedErrMsg, (actualModelState["ManagerId"] as string[])[0]);
        }

        [Fact(DisplayName = "Test Create Project throws Exception")]
        //[TestCase(Description = "Create a new Project", TestName = "Test Create Project throws Exception")]
        public void Test_Post_New_Project_For_Exception()
        {
            // Arrange
            var newTestProject = new Models.ViewModels.Project() { ProjectId = 5, ProjectName = "Test New Project", ManagerId = Guid.NewGuid(),
                                Priority = 20, ProjectStart = DateTime.Today };
            var expectedErrMsg = "Test Exception raised";
            mockProjectsLogic.Setup(api => api.CreateProject(newTestProject)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Post(newTestProject);
            var actualData = ((ObjectResult)actualResult);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualData.StatusCode);
            Assert.Equal(actualData.Value, expectedErrMsg);
        }
        #endregion        
        
        #region DELETE
        [Fact(DisplayName = "Test for Delete a valid Project returns Ok result")]
        //[TestCase(2, Description = "Delete Project", TestName = "Test for Delete a valid Project returns Ok result")]
        public void Test_Delete_Project_Valid()
        {
            // Arrange
            int index = 2;
            var existingProject = mockProjectsList[index];
            var expectedTestResult = new OkObjectResult(true);
            mockProjectsLogic.Setup(api => api.Remove(existingProject.ProjectId)).Returns(true);

            // Act
            var actualResult = mockController.Delete(existingProject.ProjectId);
            var actualData = (bool) ((OkObjectResult)actualResult).Value;

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsType<OkObjectResult>(actualResult);
            Assert.True(actualData);
        }

        [Fact(DisplayName = "Test for Delete Project throws Exception")]
        //[TestCase(1, Description = "Delete Project", TestName = "Test for Delete Project throws Exception")]
        public void Test_Delete_Project_For_Exception()
        {
            // Arrange
            int index = 1;
            var existingProject = mockProjectsList[index];
            var expectedErrMsg = "Test Exception raised";
            mockProjectsLogic.Setup(api => api.Remove(existingProject.ProjectId)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Delete(existingProject.ProjectId);
            var actualData = ((ObjectResult)actualResult);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(expectedErrMsg, actualData.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualData.StatusCode);
        }
        #endregion
        
        #region PUT
        [Fact(DisplayName = "Test for Update Project returns Ok result")]
        //[TestCase(1, "Updated New Name",Description = "Updates existing Project", TestName = "Test for Update Project returns Ok result")]
        public void Test_Put_Project_Valid()
        {
            // Arrange
            int index = 1;
            string ProjNameToUpdate = "Updated New Name";
            var projToUpdate = mockProjectsList[index];
            projToUpdate.ProjectName = ProjNameToUpdate;

            var expectedTestResult = new OkObjectResult(true);
            mockProjectsLogic.Setup(api => api.Modify(projToUpdate.ProjectId, projToUpdate)).Returns(true);

            // Act
            var actualResult = mockController.Put(projToUpdate.ProjectId, projToUpdate);
            var actualUserData = ((OkObjectResult)actualResult).Value;

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsType<OkObjectResult>(actualResult);
            Assert.True((bool)actualUserData);
        }

        [Fact(DisplayName = "Test for Update Project returns BadRequest result")]
        //[TestCase(1, "", Description = "Updates existing Project", TestName = "Test for Update Project returns BadRequest result")]
        public void Test_Put_Project_InValid()
        {
            // Arrange
            int index = 1;
            string ProjNameToUpdate = string.Empty;
            var projToUpdate = mockProjectsList[index];
            projToUpdate.ProjectName = ProjNameToUpdate;

            var validationCxt = new ValidationContext(projToUpdate);
            var expectedErrMsg = "Invalid request information. Please verify the information entered.";
            mockController.ModelState.AddModelError("ProjectName", expectedErrMsg);
            mockProjectsLogic.Setup(api => api.Modify(projToUpdate.ProjectId, projToUpdate));

            // Act
            var actualResult = mockController.Put(projToUpdate.ProjectId, projToUpdate);
            var actualData = (BadRequestObjectResult)actualResult;
            var actualModelState = (SerializableError)actualData.Value;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status400BadRequest, actualData.StatusCode);
            Assert.Contains("ProjectName", actualModelState.Keys);
            Assert.Contains(expectedErrMsg, (actualModelState["ProjectName"] as string[])[0]);
        }

        [Fact(DisplayName = "Test for Update Project throws Exception")]
        //[TestCase(1, "Updated New Name", Description = "Updates existing Project", TestName = "Test for Update Project throws Exception")]
        public void Test_Put_Project_Throws_Exception()
        {
            // Arrange
            int index = 1;
            string ProjectNameToUpdate = "Updated New Name";
            var projToUpdate = mockProjectsList[index];
            projToUpdate.ProjectName = ProjectNameToUpdate;

            var expectedErrMsg = "Test Exception raised";
            mockProjectsLogic.Setup(api => api.Modify(projToUpdate.ProjectId, projToUpdate)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Put(projToUpdate.ProjectId, projToUpdate);
            var actualUserData = ((ObjectResult)actualResult);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(actualUserData.Value, expectedErrMsg);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualUserData.StatusCode);
        }

        [Fact(DisplayName = "Test for Update Project returns Not Found status")]
        //[TestCase(55, "Updated New Name", Description = "Updates existing Project", TestName = "Test for Update Project returns Not Found status")]
        public void Test_Put_Project_Not_Found()
        {
            // Arrange
            int index = 55;
            string ProjecttNameToUpdate = "Updated New Name";
            var projToUpdate = new Models.ViewModels.Project { ProjectId = index, ProjectName = ProjecttNameToUpdate, ManagerId = Guid.NewGuid(), Priority = 5};
            projToUpdate.ProjectName = ProjecttNameToUpdate;
            mockProjectsLogic.Setup(api => api.Modify(projToUpdate.ProjectId, projToUpdate)).Returns(false);

            // Act
            var actualResult = mockController.Put(projToUpdate.ProjectId, projToUpdate);
            var actualUserData = (NotFoundResult)actualResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsType<NotFoundResult>(actualUserData);
        }
        #endregion

        #region Get all Tasks for selected Project
        [Fact(DisplayName = "Test for Get User Projects returns Ok result")]
        public void Test_Get_AllTasks_For_Project()
        {
            // Arrange
            int projectId = 2;
            var expectedTestResult = new OkObjectResult(mockProjectsList);
            var mockTasks = new Models.ViewModels.Task[]
            {
                new Models.ViewModels.Task(){ProjectId = projectId, TaskId = 1, TaskName = "TestTask-1", TaskOwnerId = Guid.NewGuid(), Priority = 5},
                new Models.ViewModels.Task(){ProjectId = projectId, TaskId = 2, TaskName = "TestTask-2", TaskOwnerId = Guid.NewGuid(), Priority = 10},
                new Models.ViewModels.Task(){ProjectId = projectId, TaskId = 3, TaskName = "TestTask-3", TaskOwnerId = Guid.NewGuid(), Priority = 15},
                new Models.ViewModels.Task(){ProjectId = projectId, TaskId = 4, TaskName = "TestTask-4", TaskOwnerId = Guid.NewGuid(), Priority = 20},
                new Models.ViewModels.Task(){ProjectId = projectId, TaskId = 5, TaskName = "TestTask-5", TaskOwnerId = Guid.NewGuid(), Priority = 25},
            };
            mockTaskLogic.Setup(api => api.GetAllTasksForProject(projectId)).Returns(mockTasks);

            // Act
            var actualResult = mockController.GetAllTasksForProject(projectId);
            var actualResultData = (OkObjectResult)actualResult;
            var actualUserProjList = actualResultData.Value as Models.ViewModels.Task[];

            // Assert
            Assert.IsType<OkObjectResult>(actualResultData);
            Assert.Equal(StatusCodes.Status200OK, actualResultData.StatusCode);
            Assert.Equal(mockTasks, actualUserProjList);
        }

        [Fact(DisplayName = "Test for Get User Projects throws exception")]
        public void Test_Get_AllTasks_For_Project_Throws_Exception()
        {
            // Arrange
            int projectId = 5;
            var expectedErrMsg = "Test for Exception";
            mockTaskLogic.Setup(u => u.GetAllTasksForProject(projectId)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.GetAllTasksForProject(projectId);
            var actualResultData = (ObjectResult)actualResult;
            //var actualUserProjList = actualResultData.Value as Models.ViewModels.Task[];

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResultData.StatusCode);
            Assert.Equal(expectedErrMsg, actualResultData.Value);
        }
        #endregion

        #region End a Project
        [Theory(DisplayName = "Test for Ending a Project returns Ok")]
        [InlineData(20, true)]
        [InlineData(200, false)]
        
        public void Test_For_End_Project_Valid(int projectIdToEnd, bool expectedResult)
        {
            // Arrange
            var expectedTestResult = new OkObjectResult(expectedResult);
            mockProjectsLogic.Setup(api => api.EndProject(projectIdToEnd)).Returns(expectedResult);

            // Act
            var actualResult = mockController.EndProject(projectIdToEnd);
            var actualProjData = ((OkObjectResult)actualResult).Value;

            // Assert
            Assert.IsType<OkObjectResult>(actualResult);
            Assert.Equal(expectedResult, (bool)actualProjData);
        }

        [Fact(DisplayName = "Test for Ending a Project throws Exception")]
        public void Test_For_End_Project_For_Exception()
        {
            // Arrange
            var projectIdToEnd = 300;
            var expectedErrMsg = "Error during Ending the Project. Please try again";
            mockProjectsLogic.Setup(api => api.EndProject(projectIdToEnd)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.EndProject(projectIdToEnd);
            var actualData = ((ObjectResult)actualResult);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualData.StatusCode);
            Assert.Equal(actualData.Value, expectedErrMsg);
        }
        #endregion

        #region SEARCH Projects
        [Fact(DisplayName = "Test for Search a valid Project returns Ok result")]
        //[TestCase("User1First", Description = "Search User", TestName = "Test for Search a valid User returns Ok result")]
        public void Test_Search_Project_Valid()
        {
            // Arrange
            string testSearchKeyword = "TestProject";
            var validSearchResult = mockProjectsList.Where(p => p.ProjectName.ToLower().Contains(testSearchKeyword.ToLower()));
            var expectedTestResult = new OkObjectResult(validSearchResult);
            mockProjectsLogic.Setup(api => api.Search(testSearchKeyword, It.IsAny<bool>(), It.IsAny<string>())).Returns(validSearchResult);

            // Act
            var actualResult = mockController.Search(testSearchKeyword);
            var actualUserData = (OkObjectResult)actualResult;

            // Assert
            Assert.NotNull(actualUserData);
            Assert.Equal(StatusCodes.Status200OK, actualUserData.StatusCode);
            Assert.Equal(validSearchResult.Count(), (actualUserData.Value as IEnumerable<Models.ViewModels.Project>).Count());
        }

        [Fact(DisplayName = "Test for Search Project throws Exception")]
        //[TestCase("User1First", Description = "Search User", TestName = "Test for Search User throws Exception")]
        public void Test_Search_User_For_Exception()
        {
            // Arrange
            string testSearchKeyword = "TestProject";
            var searchResult = mockProjectsList.Any(p => p.ProjectName.Contains(testSearchKeyword));
            var expectedErrMsg = $"Error during Search by {testSearchKeyword}.";
            mockProjectsLogic.Setup(api => api.Search(testSearchKeyword, It.IsAny<bool>(), It.IsAny<string>())).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Search(testSearchKeyword);
            var actualUserData = (ObjectResult)actualResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualUserData.StatusCode);
            Assert.Equal(expectedErrMsg, actualUserData.Value);
        }
        #endregion  
    }
}
