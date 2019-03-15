using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Data.Repos.Users;
using System;
using System.Linq;

namespace PM.Api.Controllers
{
    /// <summary>
    /// Health check
    /// </summary>
    //[EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private readonly IUserRepository userRepo;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IUserRepository _userRepo, ILogger<HealthController> logInstance)
        {
            userRepo = _userRepo;
            _logger = logInstance;
        }

        [HttpGet("[action]")]
        [ActionName("Service")]
        public IActionResult ServiceStatus()
        {
            return Ok(true);
        }

        [HttpGet("[action]")]
        [ActionName("Db")]
        public IActionResult DbStatus()
        {
            try
            {
                return Ok(userRepo.GetAll().Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Data repos or Db", ex.InnerException, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
