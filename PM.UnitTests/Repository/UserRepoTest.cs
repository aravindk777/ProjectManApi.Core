using Microsoft.EntityFrameworkCore;
using PM.Data.Entities;
using PM.Data.Repos.Users;
using PM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PM.UnitTests.Repository
{
    public class UserRepoTest: RepoTestSetup
    {
        private IUserRepository testingUserRepo;

        public UserRepoTest(): base()
        {
            InitializeUserContext();
            testingUserRepo = new UserRepository(mockContext);
        }

        void InitializeUserContext()
        {
            var testUsers = Enumerable.Range(1, 10)
                                .Select(counter => new User
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = $"TestUser{counter}",
                                    Created = DateTime.Today,
                                    FirstName = $"User{counter}FirstName",
                                    LastName = $"User{counter}LastName"
                                }).AsQueryable();

            mockContext.Users.AddRange(testUsers);
            mockContext.SaveChanges(true);
        }

        [Theory(DisplayName = "Test For DeleteUser to set end date")]
        [InlineData("TestUser1", true)]
        [InlineData("TestUser21", false)]
        public void Test_For_Delete_User(string testUserId, bool expectedResult)
        {
            var actualResult = testingUserRepo.DeleteUser(testUserId);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void Test_For_GetCount()
        {
            var expectedCount = (mockContext).Users.Count();
            var actualCount = testingUserRepo.Count();
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact(DisplayName = "Test for Get all Repo method")]
        public void Test_For_GetAllRecords()
        {
            var expectedCount = mockContext.Users.Count();
            var actualResult = testingUserRepo.GetAll();
            Assert.IsAssignableFrom<IEnumerable<User>>(actualResult);
            Assert.Equal(expectedCount, actualResult.Count());
        }

        [Fact(DisplayName = "Test for Create Entity returns Created Entity")]
        public void Test_For_CreateEntity()
        {
            var userToAdd = new User() { FirstName = "TestUser20FirstName", LastName = "TestUser20LastName", UserId = "TestUser20", Created = DateTime.Today };
            var actualUser = testingUserRepo.Create(userToAdd);
            Assert.NotNull(actualUser);
            Assert.NotEqual(Guid.Empty, actualUser.Id);
        }

        [Fact(DisplayName = "Test for Create Entity Throws Exception")]
        public void Test_For_CreateEntity_Throws_Exception()
        {
            var testUserID = mockContext.Users.FirstOrDefault().Id;
            var userToAdd = new User() { LastName = "TestUser20LastName", UserId = "TestUser2", Id=testUserID, Created = DateTime.Today };
            Assert.ThrowsAny<Exception>(() => testingUserRepo.Create(userToAdd));
            
        }
    }
}
