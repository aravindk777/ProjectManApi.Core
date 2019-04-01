using Newtonsoft.Json;
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
    public class ProjectsApiIntegrationTest
    {
        private readonly HttpClient apiTestClient;

        public ProjectsApiIntegrationTest()
        {
            apiTestClient = new TestEnvironmentClientProvider(new TestWebApplicationFactory<Startup>()).Client;
            apiTestClient.Timeout = TimeSpan.FromSeconds(200);
        }

        [Fact(DisplayName = "Test for Get All projects")]
        public async Task Test_Get_All_Projects()
        {
            var response = await apiTestClient.GetAsync("/api/Projects");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Get specific project by Id")]
        public async Task Test_Get_Project_By_Id()
        {
            var projectIdToGet = 2;
            var response = await apiTestClient.GetAsync($"/api/Projects/{projectIdToGet}");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content.ReadAsAsync<Models.ViewModels.Project>().Result);
        }

        [Fact(DisplayName = "Test for Create project")]
        public async Task Test_Create_Projects()
        {
            //var managerIdToTest = await apiTestClient.GetAsync("/api/Users/Test-User1").Result.Content.ReadAsAsync<Models.ViewModels.User>();
            var projectIdToCreate = new {ProjectId = 11, ProjectName = "NewTestProject", Priority = 20, ManagerId = Guid.NewGuid(), ProjectStart = DateTime.Today };
            var response = await apiTestClient.PostAsJsonAsync("/api/Projects", projectIdToCreate);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotEqual(0, (response.Content.ReadAsAsync<Models.ViewModels.Project>().Result).ProjectId);
        }

        [Fact(DisplayName = "Test for End specific project")]
        public async Task Test_End_Project()
        {
            var projectIdToEnd = 5;
            var response = await apiTestClient.PostAsync($"/api/Projects/{projectIdToEnd}/End",projectIdToEnd, new JsonMediaTypeFormatter(), "application/json");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory(DisplayName = "Test for Update Project api")]
        [InlineData(3, 3, HttpStatusCode.OK)]
        [InlineData(21, 3, HttpStatusCode.NotFound)]
        public async Task Test_Update_Project(int projectIdToUpdate, int existingProjectId, HttpStatusCode expectedStatus)
        { 
            var getExistingProj = await apiTestClient.GetAsync($"/api/Projects/{existingProjectId}");
            var projToUpdate = getExistingProj.Content.ReadAsAsync<Models.ViewModels.Project>().Result;
            projToUpdate.ProjectName = "UpdatedProjectName";

            var response = await apiTestClient.PutAsync($"/api/Projects/{projectIdToUpdate}", projToUpdate, new JsonMediaTypeFormatter(), "application/json");

            //response.EnsureSuccessStatusCode();
            Assert.Equal(expectedStatus, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Getting Project's Tasks")]
        public async Task Test_Get_Project_Tasks()
        {
            var projectIdToGet = 1;
            var response = await apiTestClient.GetAsync($"/api/Projects/{projectIdToGet}/Tasks");

            response.EnsureSuccessStatusCode();
            Assert.NotEmpty(response.Content.ToString()); //ReadAsAsync<IEnumerable<Models.ViewModels.Task>>());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
