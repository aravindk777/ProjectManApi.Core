using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PM.Api.Controllers;
using PM.BL.Common;
using PM.BL.Projects;
using PM.BL.Tasks;
using PM.BL.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace PM.UnitTests.Controllers
{
    public class UserApiTest
    {
        #region Test data for Setup
        private Mock<IUserLogic> mockUserLogic;
        private Mock<IProjectLogic> mockProjectsLogic;
        private Mock<ITaskLogic> mockTaskLogic;
        private Mock<ILogger<UsersController>> loggerInstance;
        private UsersController mockController;
        private List<Models.ViewModels.User> mockUsersList = new List<Models.ViewModels.User>() { };
        #endregion

        #region SETUP
        public UserApiTest()
        {
            mockUserLogic = new Mock<IUserLogic>();
            mockProjectsLogic = new Mock<IProjectLogic>();
            mockTaskLogic = new Mock<ITaskLogic>();
            loggerInstance = new Mock<ILogger<UsersController>>(MockBehavior.Loose);
            mockController = new UsersController(mockUserLogic.Object, loggerInstance.Object, mockProjectsLogic.Object, mockTaskLogic.Object);

            // Create mock users
            mockUsersList = new List<Models.ViewModels.User>();
            for (int iCounter = 0; iCounter < 10; iCounter++)
            {
                mockUsersList.Add(
                (new Models.DataModels.User
                {
                    FirstName = $"User{iCounter}First",
                    LastName = $"User{iCounter}Last",
                    UserId = $"TestUser{iCounter}",
                    Id = System.Guid.NewGuid(),
                    Created = System.DateTime.Now.AddMonths(iCounter - 5),
                    EndDate = System.DateTime.Today.AddDays(iCounter % 2)
                }).AsViewModel());
            }
        }
        #endregion

        #region Get All Users
        [Fact(DisplayName = "Get All Users - valid Result")]
        public void Test_GetAllUsers()
        {
            // Arrange
            mockUserLogic.Setup(u => u.GetUsers(It.IsAny<bool>())).Returns(mockUsersList);

            // Act
            var result = mockController.GetAllUsers();
            var actualResult = ((OkObjectResult)result).Value as IEnumerable<Models.ViewModels.User>;

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotEmpty(actualResult);
            Assert.Equal(mockUsersList.Count(), actualResult.Count());
        }

        [Fact(DisplayName = "Get All Users - Throws Exception")]
        //[TestCase(TestName = "Test for Get All Throwing Exception")]
        public void Test_GetAllUsers_Throws_Exception()
        {
            // Arrange
            var expectedErrMsg = "Test for Exception";
            mockUserLogic.Setup(u => u.GetUsers(It.IsAny<bool>())).Throws(new System.Exception(expectedErrMsg));

            // Act
            var result = mockController.GetAllUsers();
            var actualResult = (ObjectResult)result;

            // Assert
            Assert.Equal(expectedErrMsg, actualResult.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResult.StatusCode);
        }
        #endregion

        #region Get Active Users
        [Fact(DisplayName = "Get Only Active Users - Valid result")]
        //[TestCase(TestName = "Get Only Active Users")]
        public void Test_GetActiveUsers()
        {
            // Arrange
            var activeUsersMockData = mockUsersList.Where(u => u.Active);
            mockUserLogic.Setup(u => u.GetUsers(true)).Returns(activeUsersMockData);

            // Act
            var result = mockController.GetActiveUsers();
            var actualResult = ((OkObjectResult)result).Value as IEnumerable<Models.ViewModels.User>;

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotEmpty(actualResult);
            Assert.Equal(activeUsersMockData.Count(), actualResult.Count());
        }

        [Fact(DisplayName = "Get Only Active Users - Throws Exception")]
        //[TestCase(TestName = "Test for Get Active Users Throwing Exception")]
        public void Test_GetActiveUsers_ThrowsException()
        {
            // Arrange
            var activeUsersMockData = mockUsersList.Where(u => u.Active);
            var expectedErrMsg = "No data causing Null Reference during Where filter";
            mockUserLogic.Setup(u => u.GetUsers(true)).Throws(new System.NullReferenceException(expectedErrMsg));

            // Act
            var result = mockController.GetActiveUsers();
            var actualResult = (ObjectResult)result;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResult.StatusCode);
            Assert.Equal(expectedErrMsg, actualResult.Value);
        }
        #endregion

        #region POST
        [Fact(DisplayName = "Test Create valid User returns Ok result")]
        //[TestCase(Description = "Create a new User", TestName = "Test Create valid User returns Ok result")]
        public void Test_Post_NewUser_Valid()
        {
            // Arrange
            var newTestUser = new Models.ViewModels.User() { FirstName = "TestUserAddFirst", LastName = "TestUserAddLastName", UserId = "TestUserNew1" };
            var expectedTestResult = new CreatedResult(string.Concat("/", newTestUser.UserId), newTestUser);
            mockUserLogic.Setup(api => api.AddUser(newTestUser)).Returns(newTestUser);

            // Act
            var actualResult = mockController.Post(newTestUser);
            var actualUserData = (CreatedResult)actualResult;

            // Assert
            Assert.IsType<CreatedResult>(actualResult);
            Assert.Equal(StatusCodes.Status201Created, actualUserData.StatusCode);
            Assert.Equal(newTestUser, (actualUserData.Value as Models.ViewModels.User));
        }

        [Fact(DisplayName = "Test Create Invalid User returns BadRequest result")]
        //[TestCase(Description = "Create a new User", TestName = "Test Create Invalid User returns BadRequest result")]
        public void Test_Post_New_User_InValid()
        {
            // Arrange
            var newTestUser = new Models.ViewModels.User() { LastName = "TestUserAddLastName", UserId = "TestUserNew1" };
            var validationCxt = new ValidationContext(newTestUser);
            var expectedErrMsg = "Invalid request information. Please verify the information entered.";
            mockController.ModelState.AddModelError("FirstName", expectedErrMsg);
            mockUserLogic.Setup(api => api.AddUser(newTestUser));

            // Act
            var actualResult = mockController.Post(newTestUser);
            var actualUserData = (BadRequestObjectResult)actualResult;
            var actualModelState = (SerializableError)actualUserData.Value;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, actualUserData.StatusCode);
            Assert.Contains("FirstName", actualModelState.Keys);
            Assert.Contains(expectedErrMsg, (actualModelState["FirstName"] as string[])[0]);
        }

        [Fact]
        //[TestCase(Description = "Create a new User", TestName = "Test Create User throws Exception")]
        public void Test_Post_New_User_For_Exception()
        {
            // Arrange
            var newTestUser = new Models.ViewModels.User() { FirstName = "TestAddFirstNameNew", LastName = "TestUserAddLastName", UserId = "TestUserNew1" };
            var expectedErrMsg = "Test Exception raised";
            mockUserLogic.Setup(api => api.AddUser(newTestUser)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Post(newTestUser);
            var actualUserData = (ObjectResult)actualResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualUserData.StatusCode);
            Assert.Equal(expectedErrMsg, actualUserData.Value);
        }
        #endregion

        #region GET
        [Fact(DisplayName = "Get User - Valid Result")]
        //[TestCase(0, TestName = "Test for Get Http method - valid scenario")]
        public void Test_GetUser()
        {
            // Arrange
            int index = 0;
            mockUserLogic.Setup(u => u.GetUserById(mockUsersList[index].UserId)).Returns(mockUsersList[index]);

            // Act
            var result = mockController.Get(mockUsersList[index].UserId);
            var actualResult = ((OkObjectResult)result).Value as Models.ViewModels.User;

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotEmpty(actualResult.UserId);
            Assert.IsType<Models.ViewModels.User>(actualResult);
            Assert.Equal(mockUsersList[index], actualResult);
        }

        [Fact(DisplayName = "Get User - throws Exception")]
        //[TestCase(1, TestName = "Test for Get User Http method - throws Exception")]
        public void Test_GetUser_ThrowsException()
        {
            // Arrange
            int index = 1;
            var expectedErrMsg = "Db connection failure test";
            mockUserLogic.Setup(u => u.GetUserById(mockUsersList[index].UserId)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var result = mockController.Get(mockUsersList[index].UserId);
            var actualResult = (ObjectResult)result;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResult.StatusCode);
            Assert.Equal(expectedErrMsg, actualResult.Value);
        }
        #endregion

        #region DELETE
        [Fact(DisplayName = "Delete a valid User returns Ok result")]
        //[TestCase(2, Description = "Delete User", TestName = "Test for Delete a valid User returns Ok result")]
        public void Test_Delete_User_Valid()
        {
            // Arrange
            int index = 2;
            var existingUser = mockUsersList[index];
            var expectedTestResult = new OkObjectResult(true);
            mockUserLogic.Setup(api => api.DeleteUser(existingUser.UserId)).Returns(true);

            // Act
            var actualResult = mockController.Delete(existingUser.UserId);
            var actualUserData = (bool) ((OkObjectResult)actualResult).Value;

            // Assert
            Assert.NotNull(actualResult);
            Assert.True(actualUserData);
        }

        [Fact(DisplayName = "Test for Delete User throws Exception")]
        //[TestCase(1, Description = "Delete User", TestName = "Test for Delete User throws Exception")]
        public void Test_Delete_User_For_Exception()
        {
            // Arrange
            int index = 1;
            var existingUser = mockUsersList[index];
            var expectedErrMsg = "Test Exception raised";
            mockUserLogic.Setup(api => api.DeleteUser(existingUser.UserId)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Delete(existingUser.UserId);
            var actualUserData = (ObjectResult)actualResult;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualUserData.StatusCode);
            Assert.Equal(expectedErrMsg, actualUserData.Value);
        }
        #endregion

        #region PUT
        [Fact(DisplayName = "Test for Update User returns Ok result")]
        //[TestCase(1, "Updated FirstName", "Updated LastName", Description = "Updates existing User", TestName = "Test for Update User returns Ok result")]
        public void Test_Put_User_Valid()
        {
            // Arrange
            int index = 1;
            string FirstNameToUpdate = "Updated FirstName", LastNameToUpdate = "Updated LastName";
            var userToUpdate = mockUsersList[index];
            userToUpdate.FirstName = FirstNameToUpdate;
            userToUpdate.LastName = LastNameToUpdate;
            var expectedTestResult = new OkObjectResult(true);
            mockUserLogic.Setup(api => api.EditUser(userToUpdate.UserId, userToUpdate)).Returns(true);

            // Act
            var actualResult = mockController.Put(userToUpdate.UserId, userToUpdate);
            var actualUserData = (bool) ((OkObjectResult)actualResult).Value;

            // Assert
            //Assert.NotNull(actualResult);
            Assert.True(actualUserData);
        }

        [Fact(DisplayName = "Test for Update User returns BadRequest result")]
        //[TestCase(1, "", "Updated LastName", Description = "Updates existing User", TestName = "Test for Update User returns BadRequest result")]
        public void Test_Put_User_InValid()
        {
            // Arrange
            int index = 1;
            string FirstNameToUpdate = string.Empty, LastNameToUpdate = "Updated lastname";
            var userToUpdate = mockUsersList[index];
            userToUpdate.FirstName = FirstNameToUpdate;
            userToUpdate.LastName = LastNameToUpdate;

            var validationCxt = new ValidationContext(userToUpdate);
            var expectedUserData = "Invalid request information. Please verify the information entered.";
            mockController.ModelState.AddModelError("FirstName", expectedUserData);
            mockUserLogic.Setup(api => api.EditUser(userToUpdate.UserId, userToUpdate));

            // Act
            var actualResult = mockController.Put(userToUpdate.UserId, userToUpdate);
            var actualUserData = (BadRequestObjectResult)actualResult;
            var actualModelState = (SerializableError)actualUserData.Value;

            // Assert
            Assert.NotNull(actualUserData);
            Assert.Equal(StatusCodes.Status400BadRequest, actualUserData.StatusCode);
            Assert.Contains("FirstName", actualModelState.Keys);
            Assert.Contains(expectedUserData, (actualModelState["FirstName"] as string[])[0]);
            //Assert.Equal(newTestUser, actualUserData);
        }

        [Fact(DisplayName = "Test for Update User throws Exception")]
        //[TestCase(1, "Updated FirstName", "Updated LastName", Description = "Updates existing User", TestName = "Test for Update User throws Exception")]
        public void Test_Put_User_Throws_Exception()
        {
            // Arrange
            int index = 1;
            string FirstNameToUpdate = "UpdatedFirstName", LastNameToUpdate = "Updated Lastname";
            var userToUpdate = mockUsersList[index];
            userToUpdate.FirstName = FirstNameToUpdate;
            userToUpdate.LastName = LastNameToUpdate;

            var expectedErrMsg = "Test Exception raised";
            mockUserLogic.Setup(api => api.EditUser(userToUpdate.UserId, userToUpdate)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Put(userToUpdate.UserId, userToUpdate);
            var actualUserData = (ObjectResult)actualResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualUserData.StatusCode);
            Assert.Equal(expectedErrMsg, actualUserData.Value);
        }

        [Fact(DisplayName = "Test for Update User returns Not Found status")]
        //[TestCase(5, "Updated FirstName", "Updated LastName", Description = "Updates existing User", TestName = "Test for Update User returns Not Found status")]
        public void Test_Put_User_Not_Found()
        {
            // Arrange
            int index = 5;
            string FirstNameToUpdate = "UpdatedFirstName", LastNameToUpdate = "Updated Lastname";
            var userToUpdate = mockUsersList[index];
            userToUpdate.FirstName = FirstNameToUpdate;
            userToUpdate.LastName = LastNameToUpdate;
            mockUserLogic.Setup(api => api.EditUser(userToUpdate.UserId, userToUpdate)).Returns(false);

            // Act
            var actualResult = mockController.Put(userToUpdate.UserId, userToUpdate);
            var actualUserData = (NotFoundResult)actualResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status404NotFound, actualUserData.StatusCode);
        }
        #endregion

        #region Search
        [Fact(DisplayName = "Test for Search a valid User returns Ok result")]
        //[TestCase("User1First", Description = "Search User", TestName = "Test for Search a valid User returns Ok result")]
        public void Test_Search_User_Valid()
        {
            // Arrange
            string testSearchKeyword = "User1First";
            var validSearchResult = mockUsersList.Where(u => u.FirstName.ToLower().Contains(testSearchKeyword.ToLower()));
            var expectedTestResult = new OkObjectResult(validSearchResult);
            mockUserLogic.Setup(api => api.Search(testSearchKeyword, It.IsAny<bool>(), It.IsAny<string>())).Returns(validSearchResult);

            // Act
            var actualResult = mockController.Search(testSearchKeyword);
            var actualUserData = (OkObjectResult)actualResult;

            // Assert
            Assert.NotNull(actualUserData);
            Assert.Equal(StatusCodes.Status200OK, actualUserData.StatusCode);
            Assert.Equal(validSearchResult.Count(), (actualUserData.Value as IEnumerable<Models.ViewModels.User>).Count());
        }

        [Fact(DisplayName = "Test for Search User throws Exception")]
        //[TestCase("User1First", Description = "Search User", TestName = "Test for Search User throws Exception")]
        public void Test_Search_User_For_Exception()
        {
            // Arrange
            string testSearchKeyword = "User1First";
            var searchResult = mockUsersList.Where(u => u.FirstName.ToLower().Contains(testSearchKeyword.ToLower()));
            var expectedErrMsg = "Test Exception raised";
            mockUserLogic.Setup(api => api.Search(testSearchKeyword, It.IsAny<bool>(), It.IsAny<string>())).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Search(testSearchKeyword);
            var actualUserData = (ObjectResult)actualResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualUserData.StatusCode);
            Assert.Equal(expectedErrMsg, actualUserData.Value);
        }
        #endregion

        #region Get Users Projects and Tasks
        [Fact(DisplayName = "Test Get User's projects return Ok result")]
        public void Test_Get_User_Projects()
        {
            // Arrange
            string testUserId = "User1Test";
            var testUserGuid = Guid.NewGuid();
            var mockedProjectList = new Models.ViewModels.Project[] {
                new Models.ViewModels.Project(){ProjectId = 1, ProjectName = "TestProject-1", Priority = 5, ManagerId = testUserGuid, ManagerName = "User1Test"},
                new Models.ViewModels.Project(){ProjectId = 2, ProjectName = "TestProject-2", Priority = 15, ManagerId = testUserGuid, ManagerName = "User1Test"},
                new Models.ViewModels.Project(){ProjectId = 16, ProjectName = "TestProject-16", Priority = 25, ManagerId = testUserGuid, ManagerName = "User1Test"},
                new Models.ViewModels.Project(){ProjectId = 20, ProjectName = "TestProject-20", Priority = 10, ManagerId = testUserGuid, ManagerName = "User1Test"},
            };
            mockProjectsLogic.Setup(api => api.GetUserProjects(testUserId)).Returns(mockedProjectList);

            // Act
            var actualResult = mockController.GetUserProjects(testUserId);
            var actualData = (OkObjectResult)actualResult;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, actualData.StatusCode);
            Assert.Equal(mockedProjectList.Count(), (actualData.Value as IEnumerable<Models.ViewModels.Project>).Count());
        }

        [Fact(DisplayName = "Test Get User's Tasks return Ok result")]
        public void Test_Get_User_Tasks()
        {
            // Arrange
            string testUserId = "User1Test";
            var testUserGuid = Guid.NewGuid();
            var mockedTaskList = new Models.ViewModels.Task[] {
                new Models.ViewModels.Task(){TaskId = 1, TaskName = "TestTask-1", Priority = 5, TaskOwnerId = testUserGuid, OwnerFullName = "User1Test", ProjectId = 100},
                new Models.ViewModels.Task(){TaskId = 2, TaskName = "TestTask-2", Priority = 15, TaskOwnerId = testUserGuid, OwnerFullName = "User1Test", ProjectId = 100},
                new Models.ViewModels.Task(){TaskId = 16, TaskName = "TestTask-16", Priority = 25, TaskOwnerId = testUserGuid, OwnerFullName = "User1Test", ProjectId = 200},
                new Models.ViewModels.Task(){TaskId = 20, TaskName = "TestTask-20", Priority = 10, TaskOwnerId = testUserGuid, OwnerFullName = "User1Test", ProjectId = 250},
            };
            mockTaskLogic.Setup(api => api.GetAllTasksForUser(testUserId)).Returns(mockedTaskList);

            // Act
            var actualResult = mockController.GetAllTasksForUser(testUserId);
            var actualData = (OkObjectResult)actualResult;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, actualData.StatusCode);
            Assert.Equal(mockedTaskList.Count(), (actualData.Value as IEnumerable<Models.ViewModels.Task>).Count());
        }

        [Fact(DisplayName = "Test Get User's Tasks throws Exception")]
        public void Test_Get_User_Tasks_Throws_Exception()
        {
            // Arrange
            string testUserId = "User1Test";
            var testUserGuid = Guid.NewGuid();
            var expectedErrMsg = $"Error during GET Tasks by User Id - {testUserId}";
            mockTaskLogic.Setup(api => api.GetAllTasksForUser(testUserId)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.GetAllTasksForUser(testUserId);
            var actualData = (ObjectResult)actualResult;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualData.StatusCode);
            Assert.Equal(expectedErrMsg, actualData.Value);
        }

        [Fact(DisplayName = "Test Get User's project Tasks return Ok result")]
        public void Test_Get_User_Project_Tasks()
        {
            // Arrange
            string testUserId = "User1Test";
            int testProjectId = 100;
            var testUserGuid = Guid.NewGuid();
            var mockedTaskList = new Models.ViewModels.Task[] {
                new Models.ViewModels.Task(){TaskId = 1, TaskName = "TestTask-1", Priority = 5, TaskOwnerId = testUserGuid, OwnerFullName = "User1Test", ProjectId = 100},
                new Models.ViewModels.Task(){TaskId = 2, TaskName = "TestTask-2", Priority = 15, TaskOwnerId = testUserGuid, OwnerFullName = "User1Test", ProjectId = 100},
                new Models.ViewModels.Task(){TaskId = 16, TaskName = "TestTask-16", Priority = 25, TaskOwnerId = testUserGuid, OwnerFullName = "User1Test", ProjectId = 200},
                new Models.ViewModels.Task(){TaskId = 20, TaskName = "TestTask-20", Priority = 10, TaskOwnerId = testUserGuid, OwnerFullName = "User1Test", ProjectId = 250},
            };
            var expectedTasksResult = mockedTaskList.Where(p => p.ProjectId == testProjectId);
            mockTaskLogic.Setup(api => api.GetUserProjectTasks(testUserId, testProjectId)).Returns(expectedTasksResult);

            // Act
            var actualResult = mockController.GetAllTasksForUserByProject(testUserId, testProjectId);
            var actualData = (OkObjectResult)actualResult;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, actualData.StatusCode);
            Assert.Equal(expectedTasksResult.Count(), (actualData.Value as IEnumerable<Models.ViewModels.Task>).Count());
        }

        [Fact(DisplayName = "Test Get User's project Tasks throws Exception")]
        public void Test_Get_User_Project_Tasks_Throws_Exception()
        {
            // Arrange
            string testUserId = "User1Test";
            var testUserGuid = Guid.NewGuid();
            int testProjectId = 300;
            var expectedErrMsg = $"Error during GET Tasks by User Id - {testUserId} by Project Id - {testProjectId}";
            mockTaskLogic.Setup(api => api.GetUserProjectTasks(testUserId, testProjectId)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.GetAllTasksForUserByProject(testUserId, testProjectId);
            var actualData = (ObjectResult)actualResult;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualData.StatusCode);
            Assert.Equal(expectedErrMsg, actualData.Value);
        }
        #endregion
    }
}
