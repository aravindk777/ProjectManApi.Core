using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Models.ViewModels;

namespace PM.Api.Controllers
{
    /// <summary>
    /// Logging Controller for Angular UI to be used
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private ILogger<LogsController> logger;

        /// <summary>
        /// Injection contructor with Log instance
        /// </summary>
        /// <param name="logInstance"></param>
        public LogsController(ILogger<LogsController> logInstance)
        {
            logger = logInstance;
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
                                     $"{logInfo.AppName}\t|\tModule:{logInfo.Module}\t|\tMethod:{logInfo.Method}\nMessage:{logInfo.Message}" +
                                     ((!string.IsNullOrEmpty(logInfo.ErrorDetails)) ? $"\nError details:{logInfo.ErrorDetails}": string.Empty) + Environment.NewLine + 
                                     $"----------------------------------------------------------------------------------------\n";
            logger.Log(logInfo.LogType, message: completeLogInfo);
            return Ok(true);
        }
    }
}