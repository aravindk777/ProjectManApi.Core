using Moq;
using PM.BL.Common;
using PM.BL.Users;
using PM.Data.Repos.Users;
using System;
using System.Linq;
using Xunit;

namespace PM.UnitTests.Logic
{
    public class UserLogicUnitTests
    {
        private Mock<IUserRepository> mockUserRepo;
        private IUserLogic userLogicTest;

        public UserLogicUnitTests()
        {
            mockUserRepo = new Mock<IUserRepository>();
            userLogicTest = new UserLogic(mockUserRepo.Object);
        }

        [Fact(DisplayName = "Test For Add User")]
        public void Test_For_AddUser()
        {
            // Arrange
            var testUserInfoToAdd = new Models.ViewModels.User() { UserId = "TestUser1", FirstName = "TestFirstName", LastName = "TestLastName" };
            var expectedUserInfo = new Models.DataModels.User() { UserId = "TestUser1", FirstName = "TestFirstName", LastName = "TestLastName", Id = Guid.NewGuid(), Created = DateTime.Today };
            mockUserRepo.Setup(repo => repo.Create(It.IsAny<Models.DataModels.User>())).Returns(expectedUserInfo);

            // Act
            var actualResult = userLogicTest.AddUser(testUserInfoToAdd);

            // Assert
            Assert.NotNull(actualResult);
            Assert.NotEqual(Guid.Empty, actualResult.Id);
            Assert.Equal(expectedUserInfo.UserId, actualResult.UserId);
            Assert.True(actualResult.Active);
        }

        [Theory(DisplayName = "Test for Delete User")]
        [InlineData("TestUser10", true)]
        [InlineData("TestUser20", false)]
        public void Test_For_Delete_User(string testUserIdToDelete, bool expectedResult)
        {
            // Arrange
            mockUserRepo.Setup(repo => repo.DeleteUser(testUserIdToDelete)).Returns(expectedResult);
            // Act
            var actualResult = userLogicTest.DeleteUser(testUserIdToDelete);
            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory(DisplayName = "Test for Edit User scenarios")]
        [InlineData("TestUser1", true)]
        [InlineData("TestUser2", false)]
        public void Test_For_Edit_User(string userIdToEdit, bool expectedResult)
        {
            // Arrange
            var userVMToEdit = new Models.ViewModels.User() { UserId = userIdToEdit, FirstName = "TestUserName", LastName = "TestUserLastName", Id = Guid.NewGuid() };
            Models.DataModels.User userDMToGet;
            if (expectedResult)
                userDMToGet = new Models.DataModels.User() { Id = userVMToEdit.Id, UserId = userVMToEdit.UserId, FirstName = userVMToEdit.FirstName, LastName = userVMToEdit.LastName };
            else
                userDMToGet = null;

            mockUserRepo.Setup(repo => repo.GetById(userIdToEdit)).Returns(userDMToGet);
            mockUserRepo.Setup(repo => repo.Update(It.IsAny<Models.DataModels.User>())).Returns(expectedResult);
            // Act
            var actualResult = userLogicTest.EditUser(userIdToEdit, userVMToEdit);
            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory(DisplayName = "Test for Get User By UserId")]
        [InlineData("TestUser100")]
        public void Test_For_Get_User_By_Id(string testUserIdToGet)
        {
            // Arrange
            var expectedUser = new Models.DataModels.User() { UserId = testUserIdToGet, FirstName = "TestFirstName", LastName = "TestLastName", Created = DateTime.Today, Id = Guid.NewGuid() };
            mockUserRepo.Setup(repo => repo.GetById(testUserIdToGet)).Returns(expectedUser);
            // Act
            var actualUserResult = userLogicTest.GetUserById(testUserIdToGet);
            // Assert
            Assert.NotNull(actualUserResult);
            Assert.NotEqual(Guid.Empty, actualUserResult.Id);
            Assert.True(actualUserResult.Active);
        }

        [Theory(DisplayName = "Test for Get All Users and active only")]
        [InlineData(true, 6)]
        [InlineData(false, 10)]
        public void Test_For_Get_All_Users(bool isActiveOnly, int expectedTotal)
        {
            // Arrange
            var testUsersList = new Models.DataModels.User[]
            {
                new Models.DataModels.User() { UserId ="User1", FirstName = "TestFirstName1", LastName = "TestLastName1", Created = DateTime.Today, Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User2", FirstName = "TestFirstName2", LastName = "TestLastName2", Created = DateTime.Today.AddDays(-5), Id = Guid.NewGuid(), EndDate = DateTime.Today },
                new Models.DataModels.User() { UserId ="User3", FirstName = "TestFirstName3", LastName = "TestLastName3", Created = DateTime.Today.AddMonths(-8), Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User4", FirstName = "TestFirstName4", LastName = "TestLastName4", Created = DateTime.Today.AddYears(-3), Id = Guid.NewGuid(), EndDate = DateTime.Today.AddMonths(-5)},
                new Models.DataModels.User() { UserId ="User5", FirstName = "TestFirstName5", LastName = "TestLastName5", Created = DateTime.Today, Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User6", FirstName = "TestFirstName6", LastName = "TestLastName6", Created = DateTime.Today.AddMonths(-10), Id = Guid.NewGuid(), EndDate = DateTime.Today.AddMonths(-1)},
                new Models.DataModels.User() { UserId ="User7", FirstName = "TestFirstName7", LastName = "TestLastName7", Created = DateTime.Today, Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User8", FirstName = "TestFirstName8", LastName = "TestLastName8", Created = DateTime.Today.AddDays(-10), Id = Guid.NewGuid(),EndDate = DateTime.Today.AddDays(-1) },
                new Models.DataModels.User() { UserId ="User9", FirstName = "TestFirstName9", LastName = "TestLastName9", Created = DateTime.Today, Id = Guid.NewGuid(), EndDate = null },
                new Models.DataModels.User() { UserId ="User10", FirstName = "TestFirstName10", LastName = "TestLastName10", Created = DateTime.Today, Id = Guid.NewGuid(), EndDate = DateTime.MinValue },
            };
            var activeList = new Models.DataModels.User[]
            {
                new Models.DataModels.User() { UserId ="User1", FirstName = "TestFirstName1", LastName = "TestLastName1", Created = DateTime.Today, Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User3", FirstName = "TestFirstName3", LastName = "TestLastName3", Created = DateTime.Today.AddMonths(-8), Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User5", FirstName = "TestFirstName5", LastName = "TestLastName5", Created = DateTime.Today, Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User7", FirstName = "TestFirstName7", LastName = "TestLastName7", Created = DateTime.Today, Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User9", FirstName = "TestFirstName1", LastName = "TestLastName1", Created = DateTime.Today, Id = Guid.NewGuid(), EndDate = null },
                new Models.DataModels.User() { UserId ="User10", FirstName = "TestFirstName10", LastName = "TestLastName10", Created = DateTime.Today, Id = Guid.NewGuid(), EndDate = DateTime.MinValue },
            };
            if (isActiveOnly)
                mockUserRepo.Setup(repo => repo.GetAll()).Returns(activeList);
            else
                mockUserRepo.Setup(repo => repo.GetAll()).Returns(testUsersList);

            // Act
            var actualList = userLogicTest.GetUsers(isActiveOnly);
            // Assert
            Assert.NotNull(actualList);
            Assert.Equal(expectedTotal, actualList.Count());
        }

        [Theory(DisplayName = "Test for Search User")]
        [InlineData("TestFirstName", false, "")]
        [InlineData("TestFirstName", false, "firstname")]
        [InlineData("TestLastName", false, "lastname")]
        [InlineData("User", false, "userid")]
        [InlineData("User", false, "nonexistingfield")]
        public void Test_For_Search_Users(string searchText, bool exactMatch, string fieldName)
        {
            // Arrange
            var testSearchResults = new Models.DataModels.User[]
            {
                new Models.DataModels.User() { UserId ="User1", FirstName = "TestFirstName1", LastName = "TestLastName1", Created = DateTime.Today, Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User2", FirstName = "TestFirstName2", LastName = "TestLastName2", Created = DateTime.Today.AddDays(-5), Id = Guid.NewGuid(), EndDate = DateTime.Today },
                new Models.DataModels.User() { UserId ="User3", FirstName = "TestFirstName3", LastName = "TestLastName3", Created = DateTime.Today.AddMonths(-8), Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User4", FirstName = "TestFirstName4", LastName = "TestLastName4", Created = DateTime.Today.AddYears(-3), Id = Guid.NewGuid(), EndDate = DateTime.Today.AddMonths(-5)},
                new Models.DataModels.User() { UserId ="User5", FirstName = "TestFirstName5", LastName = "TestLastName5", Created = DateTime.Today, Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User6", FirstName = "TestFirstName6", LastName = "TestLastName6", Created = DateTime.Today.AddMonths(-10), Id = Guid.NewGuid(), EndDate = DateTime.Today.AddMonths(-1)},
                new Models.DataModels.User() { UserId ="User7", FirstName = "TestFirstName7", LastName = "TestLastName7", Created = DateTime.Today, Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId ="User8", FirstName = "TestFirstName8", LastName = "TestLastName8", Created = DateTime.Today.AddDays(-10), Id = Guid.NewGuid(),EndDate = DateTime.Today.AddDays(-1) },
            };
            mockUserRepo.Setup(repo => repo.Search(It.IsAny<System.Linq.Expressions.Expression<Func<Models.DataModels.User, bool>>>())).Returns(testSearchResults);
            // Act
            var actualResult = userLogicTest.Search(searchText, exactMatch, fieldName);
            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(testSearchResults.Count(), actualResult.Count());
        }

        [Fact(DisplayName = "Test - Get Count for Users")]
        public void Test_For_GetCount_Users()
        {
            // Arrange
            int expectedCount = 10;
            mockUserRepo.Setup(repo => repo.Count()).Returns(expectedCount);
            // Act
            var actualCount = userLogicTest.Count();
            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact(DisplayName = "Test for Converters User AsDataModel List")]
        public void Test_For_Converter_AsDataModelList()
        {
            // Arrange
            var usersVMList = new Models.ViewModels.User[]
            {
                new Models.ViewModels.User() { UserId = "TestUser1", FirstName = "TestFirstName1", LastName = "TestLastName1", Id = Guid.NewGuid() },
                new Models.ViewModels.User() { UserId = "TestUser2", FirstName = "TestFirstName2", LastName = "TestLastName2", Id = Guid.NewGuid() },
                new Models.ViewModels.User() { UserId = "TestUser3", FirstName = "TestFirstName3", LastName = "TestLastName3", Id = Guid.NewGuid() },
                new Models.ViewModels.User() { UserId = "TestUser4", FirstName = "TestFirstName4", LastName = "TestLastName4", Id = Guid.NewGuid() },
                new Models.ViewModels.User() { UserId = "TestUser5", FirstName = "TestFirstName5", LastName = "TestLastName5", Id = Guid.NewGuid() },
            }.AsEnumerable();
            //mockUserRepo.Setup(repo => repo.GetAll()).Returns(usersVMList.AsDataModel());
            // Act
            var actualResultAsDataModel = usersVMList.AsDataModel();
            // Assert
            Assert.Equal(usersVMList.Count(), actualResultAsDataModel.Count());
            Assert.Equal(usersVMList.Select(vm => vm.Id), actualResultAsDataModel.Select(dm => dm.Id));
        }

        [Theory(DisplayName = "Test for Conversion from ViewModel to DataModel")]
        [InlineData(true)]
        [InlineData(false)]
        public void Test_For_Converter_FromViewModel_To_DataModel(bool passModelForConversion)
        {
            var expectedGuid = Guid.NewGuid();
            var testUserViewModel = new Models.ViewModels.User() { UserId = "TestUser1", FirstName = "TestFirstName1", LastName = "TestLastName1", Id = expectedGuid };
            var testUserDataModel = new Models.DataModels.User()
            {
                UserId = "AsIsUser1",
                FirstName = "AsIsFirstName",
                LastName = "AsIsLastName",
                Created = DateTime.Today.AddDays(-1),
                Id = Guid.NewGuid()
            };

            Models.DataModels.User actualResult;
            if (passModelForConversion)
                actualResult = testUserViewModel.AsDataModel(It.IsAny<bool>(), testUserDataModel);
            else
                actualResult = testUserViewModel.AsDataModel(It.IsAny<bool>());

            Assert.Equal(expectedGuid, actualResult.Id);
            Assert.Equal(testUserViewModel.FirstName, actualResult.FirstName);
            Assert.Equal(testUserViewModel.LastName, actualResult.LastName);
            Assert.Equal(testUserViewModel.UserId, actualResult.UserId);
        }

        [Fact(DisplayName = "Test for Model Converter - AsDataModel list")]
        public void Test_For_Converting_AsDataModel_List()
        {
            // Arrange
            var usersVMList = new Models.ViewModels.User[]
            {
                new Models.ViewModels.User() { UserId = "TestUser1", FirstName = "TestFirstName1", LastName = "TestLastName1", Id = Guid.NewGuid() },
                new Models.ViewModels.User() { UserId = "TestUser2", FirstName = "TestFirstName2", LastName = "TestLastName2", Id = Guid.NewGuid() },
                new Models.ViewModels.User() { UserId = "TestUser3", FirstName = "TestFirstName3", LastName = "TestLastName3", Id = Guid.NewGuid() },
                new Models.ViewModels.User() { UserId = "TestUser4", FirstName = "TestFirstName4", LastName = "TestLastName4", Id = Guid.NewGuid() },
                new Models.ViewModels.User() { UserId = "TestUser5", FirstName = "TestFirstName5", LastName = "TestLastName5", Id = Guid.NewGuid() },
            }.AsEnumerable();

            var testDataModelList = new Models.DataModels.User[] {
                new Models.DataModels.User() { UserId = "As-Is-User1", FirstName = "As-Is-FirstName1", LastName = "As-Is-LastName1", Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId = "As-Is-User2", FirstName = "As-Is-FirstName2", LastName = "As-Is-LastName2", Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId = "As-Is-User3", FirstName = "As-Is-FirstName3", LastName = "As-Is-LastName3", Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId = "As-Is-User4", FirstName = "As-Is-FirstName4", LastName = "As-Is-LastName4", Id = Guid.NewGuid() },
                new Models.DataModels.User() { UserId = "As-Is-User5", FirstName = "As-Is-FirstName5", LastName = "As-Is-LastName5", Id = Guid.NewGuid() },
            }.AsEnumerable();

            // Act
            var actualDataModelResult = usersVMList.AsDataModel(testDataModelList);

            // Assert
            Assert.Equal(usersVMList.Select(t => t.UserId), actualDataModelResult.Select(t => t.UserId));
            Assert.Equal(usersVMList.Select(t => t.Id), actualDataModelResult.Select(t => t.Id));
        }
    }
}
