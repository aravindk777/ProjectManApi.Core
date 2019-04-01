using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.Extensions.Options;
using Moq;
using PM.Api.Controllers;
using PM.Models.Config;
using PM.Models.ViewModels;
using System;
using Xunit;

namespace PM.UnitTests.Controllers
{
    public class AppLogsUnitTest
    {
        private Mock<ILogger<LogsController>> loggerInstance;
        private Mock<IOptions<ApplicationSettings>> mockSettings;
        private LogsController mockController;

        public AppLogsUnitTest()
        {
            loggerInstance = new Mock<ILogger<LogsController>>();
            mockSettings = new Mock<IOptions<ApplicationSettings>>();
            mockController = new LogsController(loggerInstance.Object, mockSettings.Object);
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
            loggerInstance.Verify(x => x.Log(testLogInfo.LogType, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
            Assert.Equal(StatusCodes.Status200OK, actualData.StatusCode);
            Assert.True((bool)actualData.Value);
        }
    }
}
