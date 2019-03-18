using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PM.Api.Controllers;
using PM.Models.ViewModels;
using Xunit;

namespace PM.UnitTests.Controllers
{
    public class AppLogsUnitTest
    {
        private Mock<ILogger<LogsController>> loggerInstance;
        private LogsController mockController;

        public AppLogsUnitTest()
        {
            loggerInstance = new Mock<ILogger<LogsController>>();
            mockController = new LogsController(loggerInstance.Object);
        }

        [Fact(DisplayName = "Test for Posting app logs returns Ok result")]
        public void Test_Post_Log()
        {
            // Arrange
            var testLogInfo = new AppLogs() { AppName = "UnitTest", LogType = LogLevel.Information, Message = "Unit testing Log method", Method = "TestMethod()", Module = "AppLogsUnitTest" };

            // Act
            var result = mockController.Log(testLogInfo);
            var actualData = (OkObjectResult)result;

            // Assert
            Assert.NotNull(actualData);
            Assert.Equal(StatusCodes.Status200OK, actualData.StatusCode);
            Assert.True((bool)actualData.Value);
        }
    }
}
