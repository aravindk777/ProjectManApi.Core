using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PM.Api.Controllers;
using PM.BL.Common;
using PM.BL.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace PM.UnitTests.Controllers
{
    public class TaskApiTest
    {
        #region Test data for Setup
        private Mock<ITaskLogic> mockTaskLogic;
        private TasksController mockController;
        private Mock<ILogger<TasksController>> mockLogger;
        private List<Models.ViewModels.Task> mockTasksList = new List<Models.ViewModels.Task>() { };
        #endregion

        #region SETUP        
        public TaskApiTest()
        {
            mockTaskLogic = new Mock<ITaskLogic>();
            mockLogger = new Mock<ILogger<TasksController>>(MockBehavior.Loose);
            mockController = new TasksController(mockTaskLogic.Object, mockLogger.Object);

            //Create mock User Guids
            var mockUserGuids = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // Create mock Tasks
            mockTasksList = new List<Models.ViewModels.Task>();
            for (int iCounter = 0; iCounter < 10; iCounter++)
            {
                mockTasksList.Add(
                (new Models.DataModels.Task
                {
                    TaskId = (iCounter + 1),
                    TaskName = $"TestTask-{iCounter + 1}",
                    Priority = (iCounter + 1),
                    StartDate = DateTime.Now.AddMonths(iCounter - 5),
                    EndDate = DateTime.Today.AddDays(iCounter % 2),
                    TaskOwnerId = mockUserGuids[iCounter % 3],
                    ProjectId = (iCounter * 10 + 1)
                }).AsViewModel());
            }
        }
        #endregion

        #region Get All Tasks
        [Fact(DisplayName = "Get All Tasks Returns Valid Results")]
        //[TestCase(TestName = "Test for Get All Tasks Returns Valid Results")]
        public void Test_GetAll_Tasks()
        {
            // Arrange
            mockTaskLogic.Setup(u => u.GetTasks()).Returns(mockTasksList);

            // Act
            var result = mockController.Get();
            var actualResult = ((OkObjectResult)result).Value as IEnumerable<Models.ViewModels.Task>;

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotEmpty(actualResult);
            Assert.Equal(mockTasksList.Count(), actualResult.Count());
        }

        [Fact(DisplayName = "Get All Tasks Throwing Exception")]
        //[TestCase(TestName = "Test for Get All Tasks Throwing Exception")]
        public void Test_GetAllUsers_Throws_Exception()
        {
            // Arrange
            var expectedErrMsg = "Test for Exception";
            mockTaskLogic.Setup(u => u.GetTasks()).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.Get();
            var actualResult = (ObjectResult)result;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResult.StatusCode);
            Assert.Equal(expectedErrMsg, actualResult.Value);
        }
        #endregion

        #region GET
        [Fact(DisplayName = "Get Task By Id - returns Ok result")]
        //[TestCase(1, TestName = "Test for Get method by Id - valid scenario")]
        public void Test_Get_TaskById_validUsecase()
        {
            // Arrange
            int testTaskId = 1;
            var validMockData = mockTasksList.FirstOrDefault(p => p.TaskId == testTaskId);
            mockTaskLogic.Setup(u => u.GetTask(testTaskId)).Returns(validMockData);

            // Act
            var result = mockController.Get(testTaskId);
            var actualResult = ((OkObjectResult)result).Value;

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsType<Models.ViewModels.Task>(actualResult);
            Assert.Equal(validMockData, actualResult);
        }

        [Fact(DisplayName = "Get Task By Id - throws Exception")]
        //[TestCase(5, TestName = "Test for Get User Http method - throws Exception")]
        public void Test_GetTask_By_Id_ThrowsException()
        {
            // Arrange
            int testTaskId = 5;
            var expectedErrMsg = "Db connection failure test";
            mockTaskLogic.Setup(u => u.GetTask(testTaskId)).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.Get(testTaskId);
            var actualResult = (ObjectResult)result;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResult.StatusCode);
            Assert.Equal(expectedErrMsg, actualResult.Value);
        }

        [Fact(DisplayName = "Get Task By Id - returns Notfound")]
        //[TestCase(40, Description = "Get Task By Id", TestName = "Test for Get User Http method - Throws NotFound")]
        public void Test_GetTask_By_Id_Throws_Not_Found()
        {
            // Arrange
            int index = 40;
            mockTaskLogic.Setup(api => api.GetTask(index)).Returns(It.Is<Models.ViewModels.Task>(null));

            // Act
            var actualResult = mockController.Get(index);
            var actualUserData = (NotFoundResult)actualResult;

            // Assert
            Assert.IsType<NotFoundResult>(actualUserData);
            Assert.Equal(StatusCodes.Status404NotFound, actualUserData.StatusCode);
        }
        #endregion

        #region POST
        [Fact(DisplayName = "POST - Create Task returns Valid result")]
        //[TestCase(Description = "Create a new Task", TestName = "Test for Create Task returns Valid result")]
        public void Test_Post_NewTask_Valid()
        {
            // Arrange
            var newTestTask = new Models.ViewModels.Task() { TaskId = 20, TaskName = "Test New Task", TaskOwnerId = Guid.NewGuid(), Priority = 20, StartDate = DateTime.Today, ProjectId = 100 };
            var expectedTestResult = new CreatedResult(string.Concat("/", newTestTask.TaskId), newTestTask);
            mockTaskLogic.Setup(api => api.CreateTask(newTestTask)).Returns(newTestTask);

            // Act
            var actualResult = mockController.Post(newTestTask);
            var actualTaskResult = (CreatedResult)actualResult;

            // Assert
            Assert.NotNull(actualTaskResult);
            Assert.Equal(StatusCodes.Status201Created, actualTaskResult.StatusCode);
            Assert.Equal(newTestTask, actualTaskResult.Value);
        }

        [Fact]
        //[TestCase(Description = "Create a new Task", TestName = "Test Create Invalid Task returns BadRequest result")]
        public void Test_Post_New_Task_InValid()
        {
            // Arrange
            var newTestTask = new Models.ViewModels.Task() { TaskId = 25, TaskName = "Test New Task", Priority = 20 };
            var validationCxt = new ValidationContext(newTestTask);
            var expectedErrMsg = "Invalid request information. Please verify the information entered.";
            mockController.ModelState.AddModelError("ProjectId", expectedErrMsg);
            mockTaskLogic.Setup(api => api.CreateTask(newTestTask));

            // Act
            var actualStatus = mockController.Post(newTestTask);
            var actualResult = (BadRequestObjectResult)actualStatus;
            var actualModelState = (SerializableError)actualResult.Value;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Contains("ProjectId", actualModelState.Keys);
            Assert.Equal(StatusCodes.Status400BadRequest, actualResult.StatusCode);
            Assert.Equal(expectedErrMsg, (actualModelState["ProjectId"] as string[])[0]);
        }

        [Fact]
        //[TestCase(Description = "Create a new Task", TestName = "Test Create Task throws Exception")]
        public void Test_Post_New_Task_For_Exception()
        {
            // Arrange
            var newTestTask = new Models.ViewModels.Task() { TaskId = 5, TaskName = "Test New Task", TaskOwnerId = Guid.NewGuid(), Priority = 20, StartDate = DateTime.Today };
            var expectedErrMsg = "Test Exception raised missing Project Id";
            mockTaskLogic.Setup(api => api.CreateTask(newTestTask)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Post(newTestTask);
            var actualData = (ObjectResult)actualResult;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualData.StatusCode);
            Assert.Equal(expectedErrMsg, actualData.Value);
        }

        [Theory(DisplayName = "Test for Ending a Task")]
        [InlineData(2, true)]
        [InlineData(4, false)]
        //[TestCase(2, true, Description = "End a Task", TestName = "Test for Ending a Task returns Valid result with Success")]
        //[TestCase(4, false, Description = "End a Task", TestName = "Test for Ending a Task returns Valid result with failure")]
        public void Test_Post_EndTask_Valid(int taskIdx, bool expectedStatus)
        {
            // Arrange
            var testTaskToEnd = mockTasksList[taskIdx];
            testTaskToEnd.EndDate = DateTime.Now;
            var expectedTestResult = new OkObjectResult(expectedStatus);
            mockTaskLogic.Setup(api => api.EndTask(testTaskToEnd.TaskId)).Returns(expectedStatus);

            // Act
            var actualTestResult = mockController.EndTask(testTaskToEnd.TaskId);
            var actualResult = (OkObjectResult)actualTestResult;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status200OK, actualResult.StatusCode);
            Assert.Equal(expectedStatus, actualResult.Value);
        }

        [Theory(DisplayName = "Test for Ending a Task throws DB Exception")]
        [InlineData(2)]
        //[TestCase(2, true, Description = "End a Task", TestName = "Test for Ending a Task throws DB Exception")]
        public void Test_Post_EndTask_ThrowsException(int taskIdx)
        {
            // Arrange
            var testTaskToEnd = mockTasksList[taskIdx];
            testTaskToEnd.EndDate = DateTime.Now;
            var expectedErrMsg = "Test Database Exception raised";
            mockTaskLogic.Setup(api => api.EndTask(testTaskToEnd.TaskId)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualTestResult = mockController.EndTask(testTaskToEnd.TaskId);
            var actualResult = (ObjectResult)actualTestResult;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResult.StatusCode);
            Assert.Equal(expectedErrMsg, actualResult.Value);
        }
        #endregion

        #region DELETE
        [Fact(DisplayName = "Test for Delete a valid Task returns Ok result")]
        //[TestCase(2, Description = "Delete Task", TestName = "Test for Delete a valid Task returns Ok result")]
        public void Test_Delete_Task_Valid()
        {
            // Arrange
            int index = 2;
            var existingTask = mockTasksList[index];
            var expectedTestResult = new OkObjectResult(true);
            mockTaskLogic.Setup(api => api.DeleteTask(existingTask.TaskId)).Returns(true);

            // Act
            var actualResult = mockController.Delete(existingTask.TaskId);
            var actualData = (OkObjectResult)actualResult;

            // Assert
            Assert.NotNull(actualData);
            Assert.Equal(StatusCodes.Status200OK, actualData.StatusCode);
            Assert.True((bool)actualData.Value);
        }

        [Fact(DisplayName = "Test for Delete Task throws Exception")]
        //[TestCase(1, Description = "Delete Task", TestName = "Test for Delete Task throws Exception")]
        public void Test_Delete_Task_For_Exception()
        {
            // Arrange
            int index = 1;
            var existingTask = mockTasksList[index];
            var expectedErrMsg = "Test Exception raised";
            mockTaskLogic.Setup(api => api.DeleteTask(existingTask.TaskId)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Delete(existingTask.TaskId);
            var actualData = (ObjectResult)actualResult;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, actualData.StatusCode);
            Assert.Equal(expectedErrMsg, actualData.Value);
        }
        #endregion

        #region PUT
        [Fact(DisplayName = "Test for Update Task returns Ok result")]
        //[TestCase(1, "Updated New Name", Description = "Updates existing Task", TestName = "Test for Update Task returns Ok result")]
        public void Test_Put_Task_Valid()
        {
            // Arrange
            int index = 1;
            string ProjNameToUpdate = "Updated New Name";
            var projToUpdate = mockTasksList[index];
            projToUpdate.TaskName = ProjNameToUpdate;

            var expectedTestResult = new OkObjectResult(true);
            mockTaskLogic.Setup(api => api.UpdateTask(projToUpdate.TaskId, projToUpdate)).Returns(true);

            // Act
            var actualResult = mockController.Put(projToUpdate.TaskId, projToUpdate);
            var actualUserData = (OkObjectResult)actualResult;

            // Assert
            Assert.NotNull(actualUserData);
            Assert.Equal(StatusCodes.Status200OK, actualUserData.StatusCode);
            Assert.True((bool)actualUserData.Value);
        }

        [Fact(DisplayName = "Test for Update Task returns BadRequest result")]
        //[TestCase(1, "", Description = "Updates existing Task", TestName = "Test for Update Task returns BadRequest result")]
        public void Test_Put_Task_InValid()
        {
            // Arrange
            int index = 1;
            string ProjNameToUpdate = string.Empty;
            var projToUpdate = mockTasksList[index];
            projToUpdate.TaskName = ProjNameToUpdate;

            var validationCxt = new ValidationContext(projToUpdate);
            var expectedErrMsg = "Invalid request information. Please verify the information entered.";
            mockController.ModelState.AddModelError("TaskName", expectedErrMsg);
            mockTaskLogic.Setup(api => api.UpdateTask(projToUpdate.TaskId, projToUpdate));

            // Act
            var actualResult = mockController.Put(projToUpdate.TaskId, projToUpdate);
            var actualData = (BadRequestObjectResult)actualResult;
            var actualModelState = (SerializableError)actualData.Value;

            // Assert
            Assert.NotNull(actualData);
            Assert.Contains("TaskName", actualModelState.Keys);
            Assert.Equal(StatusCodes.Status400BadRequest, actualData.StatusCode);
            Assert.Equal(expectedErrMsg, (actualModelState["TaskName"] as string[])[0]);
        }

        [Fact(DisplayName = "Test for Update Task throws Exception")]
        //[TestCase(1, "Updated New Name", Description = "Updates existing Task", TestName = "Test for Update Task throws Exception")]
        public void Test_Put_Task_Throws_Exception()
        {
            // Arrange
            int index = 1;
            string TaskNameToUpdate = "Updated New Name";
            var taskToUpdate = mockTasksList[index];
            taskToUpdate.TaskName = TaskNameToUpdate;

            var expectedErrMsg = "Test Exception raised";
            mockTaskLogic.Setup(api => api.UpdateTask(taskToUpdate.TaskId, taskToUpdate)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Put(taskToUpdate.TaskId, taskToUpdate);
            var actualTaskData = (ObjectResult)actualResult;

            // Assert
            Assert.NotNull(actualTaskData);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualTaskData.StatusCode);
            Assert.Equal(expectedErrMsg, actualTaskData.Value);
        }

        //[Fact]
        ////[TestCase(55, "Updated New Name", Description = "Updates existing Task", TestName = "Test for Update Task returns Not Found status")]
        //public void Test_Put_Task_Not_Found(int index, string TasktNameToUpdate)
        //{
        //    // Arrange
        //    var taskToUpdate = new Models.ViewModels.Task { TaskId = index, TaskName = TasktNameToUpdate, TaskOwnerId = Guid.NewGuid(), Priority = 5 };
        //    taskToUpdate.TaskName = TasktNameToUpdate;
        //    mockTaskLogic.Setup(api => api.UpdateTask(taskToUpdate.TaskId, taskToUpdate)).Returns(false);

        //    // Act
        //    var actualResult = mockController.Put(taskToUpdate.TaskId, taskToUpdate);
        //    var actualTaskData = (NotFoundResult)actualResult;

        //    // Assert
        //    Assert.NotNull(actualResult);
        //    Assert.IsType(typeof(NotFoundResult), actualTaskData);
        //}
        #endregion
    }
}
