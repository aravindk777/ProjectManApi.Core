using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PM.Api.Controllers;
using PM.Data.Repos.Users;
using System;
using System.Linq;
using Xunit;

namespace PM.UnitTests.Controllers
{
    public class HealthApiUnitTest
    {
        #region Test Elements
        private Mock<IUserRepository> mockUserRepo;
        private HealthController testHlthCtrler;
        private Mock<ILogger<HealthController>> mockLogger;

        public HealthApiUnitTest()
        {
            SetupTesting();
        }
        #endregion

        /// <summary>
        /// Initialize the Test elements
        /// </summary>
        void SetupTesting()
        {
            mockUserRepo = new Mock<IUserRepository>();
            mockLogger = new Mock<ILogger<HealthController>>();
            testHlthCtrler = new HealthController(mockUserRepo.Object, mockLogger.Object);
        }

        [Fact(DisplayName = "Test GET - API Status")]
        //[Test(Description = "Test GET - API Status")]
        //[TestCase(TestName = "Test for Get API Status - returns True")]
        public void Test_For_Api_Status_GET()
        {
            // Arrange
            // -- nothing to arrange for this test

            // Act
            var actualResultType = testHlthCtrler.ServiceStatus();
            var actualResult = ((OkObjectResult)actualResultType).Value;

            // Assert
            Assert.NotNull(actualResultType);
            Assert.IsType<OkObjectResult>(actualResultType);
            Assert.True((bool)actualResult);
        }

        [Fact(DisplayName = "Test GET - Database Status")]
        //[TestCase(10, TestName = "Test for Get DB Status - returns Valid Result")]
        public void Test_For_DB_Status_GET_Valid()
        {
            // Arrange
            int expectedCount = 5;
            mockUserRepo.Setup(d => d.Count ()).Returns(expectedCount);
            // Act
            var actualResultType = testHlthCtrler.DbStatus();           
            var actualResultCount = ((OkObjectResult)actualResultType).Value;
            // Assert
            Assert.NotNull(actualResultType);
            Assert.NotNull(actualResultCount);
            Assert.IsType<OkObjectResult>(actualResultType);
            Assert.Equal(expectedCount, actualResultCount);
        }

        [Fact(DisplayName = "Test GET - Database Status")]
        //[TestCase(TestName = "Test for Get DB Status - throws Exception")]
        public void Test_For_DB_Status_Throws_Exception()
        {
            // Arrange
            var expectedErrMsg = "Db health status failed";
            mockUserRepo.Setup(u => u.Count()).Throws(new Exception(expectedErrMsg));

            // Act
            var result = testHlthCtrler.DbStatus();
            var actualResult = ((ObjectResult)result).StatusCode;
            var resultMsg = (ObjectResult)result;

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, actualResult.Value);
            Assert.Equal(expectedErrMsg, resultMsg.Value);
        }
    }
}
