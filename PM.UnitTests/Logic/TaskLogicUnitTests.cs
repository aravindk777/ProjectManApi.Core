using Moq;
using PM.BL.Common;
using PM.BL.Tasks;
using PM.Data.Repos.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PM.UnitTests.Logic
{
    public class TaskLogicUnitTests
    {
        private Mock<ITaskRepository> mockTaskRepo;
        private ITaskLogic tasksLogicTest;

        public TaskLogicUnitTests()
        {
            mockTaskRepo = new Mock<ITaskRepository>();
            tasksLogicTest = new TaskLogic(mockTaskRepo.Object);
        }

        [Theory(DisplayName = "Test for Search Tasks")]
        [InlineData("TestProject", false)]
        [InlineData("TestTask", false)]
        [InlineData("TestLastName", false)]
        [InlineData("TestParentTask", false)]
        [InlineData("TestProject-5", true)]
        public void Test_For_Search_Projects(string searchText, bool exactMatch)
        {
            // Arrange
            var tasksList = new Models.ViewModels.Task[] {
                new Models.ViewModels.Task() {ProjectId = 1, ProjectName = "TestProject-1", TaskOwnerId = Guid.NewGuid(), Priority = 10 , TaskId = 10, TaskName = "TestTask1"},
                new Models.ViewModels.Task() {ProjectId = 2, ProjectName = "TestProject-2", TaskOwnerId = Guid.NewGuid(), Priority = 5 , TaskId = 11, TaskName = "TestTask2"},
                new Models.ViewModels.Task() {ProjectId = 3, ProjectName = "TestProject-3", TaskOwnerId = Guid.NewGuid(), Priority = 15 , TaskId = 9, TaskName = "TestTask3"},
                new Models.ViewModels.Task() {ProjectId = 4, ProjectName = "TestProject-4", TaskOwnerId = Guid.NewGuid(), Priority = 20 , TaskId = 7, TaskName = "TestTask4"},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask5", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask6", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask7", ParentTaskName = "TestParentTask2", ParentTaskId = 202},
            }.AsEnumerable();
            IEnumerable<Models.ViewModels.Task> expectedResult;
            if (exactMatch)
                expectedResult = tasksList.Where(p => p.ProjectName.ToLower().Equals(searchText.ToLower()));
            else
                expectedResult = tasksList;

            mockTaskRepo.Setup(repo => repo.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Models.DataModels.Task, bool>>>())).Returns(expectedResult.AsDataModel());
            // Act
            var actualResult = tasksLogicTest.Search(searchText, exactMatch);
            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }

        #region Test for Tasks under projects and User
        [Fact(DisplayName = "Test for Get All Tasks for a Project")]
        public void Test_For_GetAllTasksForProject()
        {
            // Arrange
            int testProjectId = 10;
            var tasksList = new Models.ViewModels.Task[] {
                new Models.ViewModels.Task() {ProjectId = 10, ProjectName = "TestProject-10", TaskOwnerId = Guid.NewGuid(), Priority = 10 , TaskId = 10, TaskName = "TestTask1"},
                new Models.ViewModels.Task() {ProjectId = 2, ProjectName = "TestProject-2", TaskOwnerId = Guid.NewGuid(), Priority = 5 , TaskId = 11, TaskName = "TestTask2"},
                new Models.ViewModels.Task() {ProjectId = 10, ProjectName = "TestProject-10", TaskOwnerId = Guid.NewGuid(), Priority = 15 , TaskId = 9, TaskName = "TestTask3"},
                new Models.ViewModels.Task() {ProjectId = 4, ProjectName = "TestProject-4", TaskOwnerId = Guid.NewGuid(), Priority = 20 , TaskId = 7, TaskName = "TestTask4"},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask5", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 10, ProjectName = "TestProject-10", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask6", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask7", ParentTaskName = "TestParentTask2", ParentTaskId = 202},
            }.AsEnumerable();

            IEnumerable<Models.ViewModels.Task> expectedResult;
            expectedResult = tasksList.Where(p => p.ProjectId == testProjectId);
            mockTaskRepo.Setup(repo => repo.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Models.DataModels.Task, bool>>>())).Returns(expectedResult.AsDataModel());
            // Act
            var actualResult = tasksLogicTest.GetAllTasksForProject(testProjectId);
            // Assert
            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }

        [Fact(DisplayName = "Test for Get All Tasks for a User")]
        public void Test_For_GetAllTasksForUser()
        {
            // Arrange
            Guid testUserGuid = Guid.NewGuid();
            var tasksList = new Models.ViewModels.Task[] {
                new Models.ViewModels.Task() {ProjectId = 10, ProjectName = "TestProject-10", TaskOwnerId = testUserGuid, Priority = 10 , TaskId = 10, TaskName = "TestTask1"},
                new Models.ViewModels.Task() {ProjectId = 2, ProjectName = "TestProject-2", TaskOwnerId = Guid.NewGuid(), Priority = 5 , TaskId = 11, TaskName = "TestTask2"},
                new Models.ViewModels.Task() {ProjectId = 10, ProjectName = "TestProject-10", TaskOwnerId = testUserGuid, Priority = 15 , TaskId = 9, TaskName = "TestTask3"},
                new Models.ViewModels.Task() {ProjectId = 4, ProjectName = "TestProject-4", TaskOwnerId = Guid.NewGuid(), Priority = 20 , TaskId = 7, TaskName = "TestTask4"},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask5", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 10, ProjectName = "TestProject-10", TaskOwnerId = testUserGuid, Priority = 30 , TaskId = 5, TaskName = "TestTask6", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask7", ParentTaskName = "TestParentTask2", ParentTaskId = 202},
            }.AsEnumerable();

            IEnumerable<Models.ViewModels.Task> expectedResult;
            expectedResult = tasksList.Where(p => p.TaskOwnerId == testUserGuid);
            mockTaskRepo.Setup(repo => repo.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Models.DataModels.Task, bool>>>())).Returns(expectedResult.AsDataModel());
            // Act
            var actualResult = tasksLogicTest.GetAllTasksForUser("TestUser1");
            // Assert
            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }

        [Fact(DisplayName = "Test for Get All Tasks under a Project for a given user")]
        public void Test_For_GetUserProjectTasks()
        {
            // Arrange
            Guid testUserGuid = Guid.NewGuid();
            int testProjectId = 10;
            var tasksList = new Models.ViewModels.Task[] {
                new Models.ViewModels.Task() {ProjectId = 10, ProjectName = "TestProject-10", TaskOwnerId = testUserGuid, Priority = 10 , TaskId = 10, TaskName = "TestTask1"},
                new Models.ViewModels.Task() {ProjectId = 2, ProjectName = "TestProject-2", TaskOwnerId = Guid.NewGuid(), Priority = 5 , TaskId = 11, TaskName = "TestTask2"},
                new Models.ViewModels.Task() {ProjectId = 10, ProjectName = "TestProject-10", TaskOwnerId = testUserGuid, Priority = 15 , TaskId = 9, TaskName = "TestTask3"},
                new Models.ViewModels.Task() {ProjectId = 4, ProjectName = "TestProject-4", TaskOwnerId = Guid.NewGuid(), Priority = 20 , TaskId = 7, TaskName = "TestTask4"},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = testUserGuid, Priority = 30 , TaskId = 5, TaskName = "TestTask5", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 10, ProjectName = "TestProject-10", TaskOwnerId = testUserGuid, Priority = 30 , TaskId = 5, TaskName = "TestTask6", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = testUserGuid, Priority = 30 , TaskId = 5, TaskName = "TestTask7", ParentTaskName = "TestParentTask2", ParentTaskId = 202},
            }.AsEnumerable();

            IEnumerable<Models.ViewModels.Task> expectedResult;
            expectedResult = tasksList.Where(p => p.TaskOwnerId == testUserGuid && p.ProjectId == testProjectId);
            mockTaskRepo.Setup(repo => repo.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Models.DataModels.Task, bool>>>())).Returns(expectedResult.AsDataModel());
            // Act
            var actualResult = tasksLogicTest.GetUserProjectTasks("TestUser1", testProjectId);
            // Assert
            Assert.Equal(expectedResult.Count(), actualResult.Count());
        }
        #endregion

        #region Task Repo tests
        [Fact(DisplayName = "Test - Get Count for Tasks")]
        public void Test_For_GetCount_Tasks()
        {
            // Arrange
            int expectedCount = 10;
            mockTaskRepo.Setup(repo => repo.Count()).Returns(expectedCount);
            // Act
            var actualCount = tasksLogicTest.Count();
            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact(DisplayName = "Test - Add Task")]
        public void Test_For_AddTask()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var testTaskVM = new Models.ViewModels.Task() { TaskName = "TestTask1", Priority = 20, ProjectName = "TestProject100", OwnerFullName = "First Last", StartDate = DateTime.Today };
            var testTaskDM = new Models.DataModels.Task()
            {
                TaskName = "TestTask1",
                Priority = 20,
                ProjectId = 100,
                TaskOwnerId = userGuid,
                StartDate = DateTime.Today,
                Project = new Models.DataModels.Project() { ProjectId = 100, ProjectName = "TestProject100", Priority = 1, ManagerId = userGuid },
                TaskOwner = new Models.DataModels.User() { Id = userGuid, UserId = "TestUser1", FirstName = "First", LastName = "Last" }
            };
            mockTaskRepo.Setup(repo => repo.Create(It.IsAny<Models.DataModels.Task>())).Returns(testTaskDM);
            // Act
            var actualTask = tasksLogicTest.CreateTask(testTaskVM);
            // Assert
            Assert.NotNull(actualTask);
            Assert.Equal(testTaskVM.ProjectName, actualTask.ProjectName);
            Assert.Equal(testTaskVM.OwnerFullName, actualTask.OwnerFullName);
        }

        [Theory(DisplayName = "Test for Updating a Task")]
        [InlineData(100, true, true)]
        [InlineData(200, true, false)]
        [InlineData(300, false, false)]
        public void Test_For_EditTask(int testTaskIdForEdit, bool expectedExistsResult, bool expectedUpdateResult)
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            Models.DataModels.Task testTaskForEdit;
            if (expectedExistsResult || expectedUpdateResult)
            {
                testTaskForEdit = new Models.DataModels.Task()
                {
                    TaskId = testTaskIdForEdit,
                    TaskName = "TestTask" + testTaskIdForEdit,
                    Priority = 20,
                    ProjectId = 100,
                    TaskOwnerId = userGuid,
                    StartDate = DateTime.Today,
                    Project = new Models.DataModels.Project() { ProjectId = 100, ProjectName = "TestProject-XY", Priority = 1, ManagerId = userGuid },
                    TaskOwner = new Models.DataModels.User() { Id = userGuid, UserId = "TestUser1", FirstName = "First", LastName = "Last" }
                };
            }
            else
                testTaskForEdit = null;
            mockTaskRepo.Setup(repo => repo.GetById(testTaskIdForEdit)).Returns(testTaskForEdit);
            mockTaskRepo.Setup(repo => repo.Update(It.IsAny<Models.DataModels.Task>())).Returns(expectedUpdateResult);
            // Act
            var actualResult = tasksLogicTest.UpdateTask(testTaskIdForEdit, testTaskForEdit.AsViewModel());
            // Assert
            Assert.Equal(expectedUpdateResult, actualResult);
        }

        [Theory(DisplayName = "Test for Delete Task")]
        [InlineData(10, true)]
        [InlineData(200, false)]
        public void Test_For_DeleteTask(int testTaskIdForDelete, bool expectedResult)
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var testTaskVM = new Models.ViewModels.Task()
            {
                TaskId = testTaskIdForDelete,
                TaskName = "TestTask1",
                Priority = 20,
                ProjectName = "TestProject100",
                OwnerFullName = "First Last",
                StartDate = DateTime.Today
            };
            Models.DataModels.Task testTaskForDelete;
            if (expectedResult)
            {
                testTaskForDelete = new Models.DataModels.Task()
                {
                    TaskId = testTaskIdForDelete,
                    TaskName = "TestTask1",
                    Priority = 20,
                    ProjectId = 100,
                    TaskOwnerId = userGuid,
                    StartDate = DateTime.Today,
                    Project = new Models.DataModels.Project() { ProjectId = 100, ProjectName = "TestProject100", Priority = 1, ManagerId = userGuid },
                    TaskOwner = new Models.DataModels.User() { Id = userGuid, UserId = "TestUser1", FirstName = "First", LastName = "Last" }
                };
            }
            else
                testTaskForDelete = null;
            mockTaskRepo.Setup(repo => repo.GetById(testTaskIdForDelete)).Returns(testTaskForDelete);
            mockTaskRepo.Setup(repo => repo.Delete(It.IsAny<Models.DataModels.Task>())).Returns(expectedResult);
            // Act
            var actualResult = tasksLogicTest.DeleteTask(testTaskIdForDelete);
            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory(DisplayName = "Test for Ending a task")]
        [InlineData(20, true)]
        [InlineData(200, false)]
        public void Test_For_EndTask(int testTaskIdForDelete, bool expectedResult)
        {
            // Arrange
            mockTaskRepo.Setup(repo => repo.EndTask(testTaskIdForDelete)).Returns(expectedResult);
            // Act
            var actualResult = tasksLogicTest.EndTask(testTaskIdForDelete);
            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact(DisplayName = "Test for Get Task By Id")]
        public void Test_For_Get_Task_By_Id()
        {
            // Arrange
            var testTaskId = 5;
            var testTaskVM = new Models.ViewModels.Task()
            {
                TaskId = testTaskId,
                TaskName = "TestTask1",
                Priority = 20,
                ProjectName = "TestProject100",
                OwnerFullName = "First Last",
                StartDate = DateTime.Today
            };
            mockTaskRepo.Setup(repo => repo.GetById(testTaskId)).Returns(testTaskVM.AsDataModel());
            // Act
            var actualTask = tasksLogicTest.GetTask(testTaskId);
            // Assert
            Assert.NotNull(actualTask);
            Assert.Equal(testTaskVM.TaskName, actualTask.TaskName);
        }

        [Fact(DisplayName = "Test For Get all Tasks")]
        public void Test_For_Get_AllTasks()
        {
            // Arrange
            var testTasksList = new Models.DataModels.Task[]
                {
                    new Models.DataModels.Task() { TaskName = "TestTask1", Priority = 20, ProjectId = 100, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = null},
                    new Models.DataModels.Task() { TaskName = "TestTask2", Priority = 20, ProjectId = 200, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = 100},
                    new Models.DataModels.Task() { TaskName = "TestTask3", Priority = 20, ProjectId = 130, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = null},
                    new Models.DataModels.Task() { TaskName = "TestTask4", Priority = 20, ProjectId = 400, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = 140},
                    new Models.DataModels.Task() { TaskName = "TestTask5", Priority = 20, ProjectId = 160, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = 100},
                    new Models.DataModels.Task() { TaskName = "TestTask6", Priority = 20, ProjectId = 140, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = null},
                };
            mockTaskRepo.Setup(repo => repo.GetAll()).Returns(testTasksList);
            // Act
            var actualResult = tasksLogicTest.GetTasks();
            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(testTasksList.Count(), actualResult.Count());
        }

        [Fact(DisplayName = "Test For Get Active Tasks")]
        public void Test_For_Get_Active_Tasks()
        {
            // Arrange
            var testTasksList = new Models.DataModels.Task[]
                {
                    new Models.DataModels.Task() { TaskName = "TestTask1", Priority = 20, ProjectId = 100, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today.AddDays(-5), ParentTaskId = null, EndDate = DateTime.Today},
                    new Models.DataModels.Task() { TaskName = "TestTask2", Priority = 20, ProjectId = 200, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today.AddDays(-5), ParentTaskId = 100, EndDate = DateTime.Today.AddDays(-1)},
                    new Models.DataModels.Task() { TaskName = "TestTask3", Priority = 20, ProjectId = 130, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = null},
                    new Models.DataModels.Task() { TaskName = "TestTask4", Priority = 20, ProjectId = 400, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = 140},
                    new Models.DataModels.Task() { TaskName = "TestTask5", Priority = 20, ProjectId = 160, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = 100},
                    new Models.DataModels.Task() { TaskName = "TestTask6", Priority = 20, ProjectId = 140, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ParentTaskId = null},
                };
            mockTaskRepo.Setup(repo => repo.GetAll()).Returns(testTasksList);
            var expectedCount = testTasksList.AsViewModel().Count(t => t.IsActive);
            // Act
            var actualResult = tasksLogicTest.GetTasks(true);
            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(expectedCount, actualResult.Count());
        }
        #endregion

        #region Common - Converter Tests

        [Fact(DisplayName = "Test for Model Converter - AsDataModel list")]
        public void Test_For_Converting_AsDataModel_List()
        {
            // Arrange
            var tasksList = new Models.ViewModels.Task[] {
                new Models.ViewModels.Task() {ProjectId = 1, ProjectName = "TestProject-1", TaskOwnerId = Guid.NewGuid(), Priority = 10 , TaskId = 10, TaskName = "TestTask1"},
                new Models.ViewModels.Task() {ProjectId = 2, ProjectName = "TestProject-2", TaskOwnerId = Guid.NewGuid(), Priority = 5 , TaskId = 11, TaskName = "TestTask2"},
                new Models.ViewModels.Task() {ProjectId = 3, ProjectName = "TestProject-3", TaskOwnerId = Guid.NewGuid(), Priority = 15 , TaskId = 9, TaskName = "TestTask3"},
                new Models.ViewModels.Task() {ProjectId = 4, ProjectName = "TestProject-4", TaskOwnerId = Guid.NewGuid(), Priority = 20 , TaskId = 7, TaskName = "TestTask4"},
                new Models.ViewModels.Task() {ProjectId = 5, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask5", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 6, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask6", ParentTaskName = "TestParentTask1", ParentTaskId = 200},
                new Models.ViewModels.Task() {ProjectId = 7, ProjectName = "TestProject-5", TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "TestTask7", ParentTaskName = "TestParentTask2", ParentTaskId = 202},
            }.AsEnumerable();

            var testDataModelList = new Models.DataModels.Task[] {
                new Models.DataModels.Task() {ProjectId = 1, Project =new Models.DataModels.Project() { ProjectName = "test-1", ProjectId = 1 }, TaskOwnerId = Guid.NewGuid(), Priority = 10 , TaskId = 10, TaskName = "As-Is-Task1"},
                new Models.DataModels.Task() {ProjectId = 2, Project =new Models.DataModels.Project() { ProjectName = "test-2", ProjectId = 2 }, TaskOwnerId = Guid.NewGuid(), Priority = 5 , TaskId = 11, TaskName = "As-Is-Task2"},
                new Models.DataModels.Task() {ProjectId = 3, Project =new Models.DataModels.Project() { ProjectName = "test-3", ProjectId = 3 }, TaskOwnerId = Guid.NewGuid(), Priority = 15 , TaskId = 9, TaskName = "As-Is-Task3"},
                new Models.DataModels.Task() {ProjectId = 4, Project =new Models.DataModels.Project() { ProjectName = "test-4", ProjectId = 4 }, TaskOwnerId = Guid.NewGuid(), Priority = 20 , TaskId = 7, TaskName = "As-Is-Task4"},
                new Models.DataModels.Task() {ProjectId = 5, Project =new Models.DataModels.Project() { ProjectName = "test-5", ProjectId = 5 }, TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "As-Is-Task5", },
                new Models.DataModels.Task() {ProjectId = 6, Project =new Models.DataModels.Project() { ProjectName = "test-6", ProjectId = 6 }, TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "As-Is-Task6", },
                new Models.DataModels.Task() {ProjectId = 7, Project =new Models.DataModels.Project() { ProjectName = "test-7", ProjectId = 7 }, TaskOwnerId = Guid.NewGuid(), Priority = 30 , TaskId = 5, TaskName = "As-Is-Task7", },
            }.AsEnumerable();

            // Act
            var actualDataModelResult = tasksList.AsDataModel(testDataModelList);

            // Assert
            Assert.Equal(testDataModelList.Select(t => t.ProjectId), actualDataModelResult.Select(t => t.ProjectId));
            Assert.Equal(tasksList.Select(t => t.TaskName), actualDataModelResult.Select(t => t.TaskName));
        }
        #endregion
    }
}
