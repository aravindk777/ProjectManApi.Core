using PM.Api.IntegrationTest.Environment;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Xunit;

namespace PM.Api.IntegrationTest.Api
{
    public class UserApiIntegrationTest
    {
        private readonly HttpClient userApiHttpClient;
        public UserApiIntegrationTest()
        {
            //using (var httpClient = new TestEnvironmentClientProvider(new TestWebApplicationFactory<Startup>()).Client)
            //{
            //    userApiHttpClient = httpClient;
            //}
            userApiHttpClient = new TestEnvironmentClientProvider(new TestWebApplicationFactory<Startup>()).Client;
        }

        [Fact(DisplayName = "Test for Get All Users")]
        public async Task Test_Get_All_Users()
        {
            var response = await userApiHttpClient.GetAsync("/api/Users");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Get Active Users")]
        public async Task Test_Get_Active_Users()
        {
            var response = await userApiHttpClient.GetAsync("/api/Users/Active");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Get a particular User by UserId")]
        public async Task Test_Get_User_By_UserId()
        {
            var response = await userApiHttpClient.GetAsync("/api/Users/Test-User1");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content.ReadAsAsync<Models.ViewModels.User>().Result);
        }

        [Fact]
        public async Task Test_Create_New_User()
        {
            var newUser = new Models.ViewModels.User() { UserId = "NewTestUser", FirstName = "NewTestFirstName", LastName = "NewTestLastName" };
            var response = await userApiHttpClient.PostAsync("/api/Users", newUser, new JsonMediaTypeFormatter(), "application/json");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Update User api")]
        public async Task Test_Update_User()
        {
            var getUser3 = await userApiHttpClient.GetAsync("/api/Users/Test-User3");
            var userToUpdate = getUser3.Content.ReadAsAsync<Models.ViewModels.User>().Result;
            userToUpdate.FirstName = "UpdatedFirstName3";
            userToUpdate.LastName = "UpdatedLastName3";

            var response = await userApiHttpClient.PutAsync("/api/Users/Test-User3", userToUpdate, new JsonMediaTypeFormatter(), "application/json");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Delete User api")]
        public async Task Test_Delete_User()
        {
            var userIdToDelete = "Test-User2";
            var response = await userApiHttpClient.DeleteAsync($"/api/Users/{userIdToDelete}");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Get User's Projects")]
        public async Task Test_Get_User_Projects()
        {
            var userId = "Test-User1";
            var response = await userApiHttpClient.GetAsync($"/api/Users/{userId}/Projects");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Get User's Tasks")]
        public async Task Test_Get_User_Tasks()
        {
            var userId = "Test-User1";
            var response = await userApiHttpClient.GetAsync($"/api/Users/{userId}/Tasks");

            response.EnsureSuccessStatusCode();
            Assert.NotEmpty(response.Content.ToString()); //ReadAsAsync<IEnumerable<Models.ViewModels.Task>>());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Test for Get User's Project related Tasks")]
        public async Task Test_Get_User_ProjectTasks()
        {
            var userId = "Test-User1";
            var testProjectId = 1;
            var response = await userApiHttpClient.GetAsync($"/api/Users/{userId}/Projects/{testProjectId}/Tasks");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
