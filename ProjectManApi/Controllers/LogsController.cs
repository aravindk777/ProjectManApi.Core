using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PM.Models.Config;
using PM.Models.ViewModels;

namespace PM.Api.Controllers
{
    /// <summary>
    /// Logging Controller for Angular UI to be used
    /// </summary>
    [EnableCors("ProjectManagerApiCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : Controller
    {
        private ILogger<LogsController> logger;
        private IOptions<ApplicationSettings> configSettings;

        /// <summary>
        /// Injection contructor with Log instance
        /// </summary>
        /// <param name="logInstance"></param>
        /// <param name="_settings"></param>
        public LogsController(ILogger<LogsController> logInstance, IOptions<ApplicationSettings> _settings)
        {
            logger = logInstance;
            configSettings = _settings;
        }

        /// <summary>
        /// Log method to log any info from UI. This will be utilized from the Angular UI so have better logging than just console.log
        /// </summary>
        /// <param name="logInfo">Logging parameters</param>
        /// <returns>true</returns>
        [HttpPost]
        public IActionResult Log([FromBody] AppLogs logInfo)
        {
            string completeLogInfo = $"\n----------------------------------------------------------------------------------------\n" +
                                     $"{logInfo.AppName}\t|\tModule:{logInfo.Module}\t|\tMethod:{logInfo.Method}\nMessage:{logInfo.Message}\n" +
                                     ((!string.IsNullOrEmpty(logInfo.ErrorDetails)) ? $"\nError details:{logInfo.ErrorDetails}": string.Empty) + Environment.NewLine +
                                     $"----------------------------------------------------------------------------------------\n";

            if (!string.IsNullOrEmpty(logInfo.ErrorDetails))
                logger.LogError(logInfo.ErrorDetails);
            else
                logger.Log(logInfo.LogType, message: completeLogInfo);
            return Ok(true);
        }
    }
}