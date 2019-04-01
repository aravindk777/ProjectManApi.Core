using PM.Api.IntegrationTest.Environment;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PM.Api.IntegrationTest.Api
{
    public class TasksApiIntegrationTest
    {
        private readonly HttpClient apiTestClient;

        public TasksApiIntegrationTest()
        {
            apiTestClient = new TestEnvironmentClientProvider(new TestWebApplicationFactory<Startup>()).Client;
            apiTestClient.Timeout = TimeSpan.FromSeconds(200);
        }

        [Fact(DisplayName = "Test for Get All Tasks")]
        public async Task Test_Get_All_Tasks()
        {
            var response = await apiTestClient.GetAsync("/api/Tasks");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Get specific Task by Id")]
        public async Task Test_Get_Task_By_Id()
        {
            var TaskIdToGet = 2;
            var response = await apiTestClient.GetAsync($"/api/Tasks/{TaskIdToGet}");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content.ReadAsAsync<Models.ViewModels.Task>().Result);
        }

        [Fact(DisplayName = "Test for Create Task")]
        public async Task Test_Create_Tasks()
        {
            //var managerIdToTest = await apiTestClient.GetAsync("/api/Users/Test-User1").Result.Content.ReadAsAsync<Models.ViewModels.User>();
            var TaskIdToCreate = new Models.ViewModels.Task { TaskId = 11, TaskName = "NewTestTask", Priority = 20, TaskOwnerId = Guid.NewGuid(), StartDate = DateTime.Today, ProjectId = 1 };
            var response = await apiTestClient.PostAsJsonAsync("/api/Tasks", TaskIdToCreate);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotEqual(0, (response.Content.ReadAsAsync<Models.ViewModels.Task>().Result).TaskId);
        }

        [Fact(DisplayName = "Test for End specific Task")]
        public async Task Test_End_Task()
        {
            var TaskIdToEnd = 5;
            var response = await apiTestClient.PostAsync($"/api/Tasks/{TaskIdToEnd}/End", TaskIdToEnd, new JsonMediaTypeFormatter(), "application/json");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory(DisplayName = "Test for Update Task api")]
        [InlineData(3, 3, HttpStatusCode.OK)]
        [InlineData(21, 3, HttpStatusCode.BadRequest)]
        public async Task Test_Update_Task(int TaskIdToUpdate, int existingTaskId, HttpStatusCode expectedStatus)
        {
            var getExistingTask = await apiTestClient.GetAsync($"/api/Tasks/{existingTaskId}");
            var taskToUpdate = getExistingTask.Content.ReadAsAsync<Models.ViewModels.Task>().Result;
            taskToUpdate.TaskName = "UpdatedTaskName";

            var response = await apiTestClient.PutAsync($"/api/Tasks/{TaskIdToUpdate}", taskToUpdate, new JsonMediaTypeFormatter(), "application/json");
            //response.EnsureSuccessStatusCode();
            Assert.Equal(expectedStatus, response.StatusCode);
        }
    }
}
