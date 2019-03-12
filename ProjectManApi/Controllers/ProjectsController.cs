using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Api.Extensions;
using PM.BL.Projects;
using PM.Models.ViewModels;
using System;
using System.Linq;

namespace PM.Api.Controllers
{
    /// <summary>
    /// Projects Controller
    /// </summary>
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        private IProjectLogic _projectOrhestrator;
        private ILogger<ProjectsController> logger;

        public ProjectsController(IProjectLogic projectOrhestrator, ILogger<ProjectsController> _logInstance)
        {
            _projectOrhestrator = projectOrhestrator;
            logger = _logInstance;
        }

        // GET: api/Projects
        /// <summary>
        /// Get all Projects list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var result = _projectOrhestrator.GetAllProjects();
                logger.LogInformation("Get All - total records found: " + result.Count());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Get All Projects", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var result = _projectOrhestrator.GetProject(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/Projects
        [HttpPost]
        public IActionResult Post([FromBody]Project value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _projectOrhestrator.CreateProject(value);
                    var createdUrl = string.Join("/", Request.Path, result.ProjectId);
                    return Created(createdUrl, result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during POST for Projects with incoming Values: {0}", value.Stringify());
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                logger.LogWarning("Invalid ModelState. See below for details.\nModelState: {0}\nData supplied:{1}", ModelState.Stringify(), value.Stringify());
                return BadRequest(ModelState);
            }
        }

        // PUT: api/Projects/5
        [HttpPut]
        public IActionResult Put(int id, [FromBody]Project value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _projectOrhestrator.Modify(id, value);
                    if (result)
                        return Ok(result);
                    else
                        return NotFound();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error during Update to Project Id {id} with values: {value.Stringify()}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                logger.LogWarning("Model state is invalid .See below for details\nModelState: {0}\nIncoming changes: {1}", ModelState.Stringify(), value.Stringify());
                return BadRequest(ModelState);
            }
        }

        // DELETE: api/Projects/5
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _projectOrhestrator.Remove(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Deleting the Project with Id {0}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Returns list of projects associated to the manager by UserId
        /// </summary>
        /// <param name="userId">UserId of the manager</param>
        /// <returns>List of Projects belonging to the User</returns>
        /// <example>api/Users/user1/Projects</example>
        //[HttpGet("{UserId}/Projects")]
        //[Route("api/Users/{userId}/Projects")]
        //public IActionResult GetUserProjects(string userId)
        //{
        //    return Ok(_projectOrhestrator.GetUserProjects(userId));
        //}
    }
}
