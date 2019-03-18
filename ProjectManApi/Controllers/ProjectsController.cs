using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Api.Extensions;
using PM.BL.Projects;
using PM.BL.Tasks;
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
        private ITaskLogic _taskOrhestrator;
        private ILogger<ProjectsController> logger;

        /// <summary>
        /// Injection constructor for Projects Controller
        /// </summary>
        /// <param name="projectOrhestrator"></param>
        /// <param name="_logInstance"></param>
        /// <param name="taskLogicInstance"></param>
        public ProjectsController(IProjectLogic projectOrhestrator, ILogger<ProjectsController> _logInstance, ITaskLogic taskLogicInstance)
        {
            _projectOrhestrator = projectOrhestrator;
            _taskOrhestrator = taskLogicInstance;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        // POST to End a project
        [HttpPost]
        [Route("{id}/End")]
        public IActionResult EndProject(int id)
        {
            try
            {
                var status = _projectOrhestrator.EndProject(id);
                return Ok(status);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error during Ending a project for " + id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/Projects/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        // GET: api/Projects/{ProjId}/Tasks
        /// <summary>
        /// Get all Tasks for project Id
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <returns>List of tasks under the project ID</returns>
        [HttpGet("{projectId}/Tasks")]
        public IActionResult GetAllTasksForProject(int projectId)
        {
            try
            {
                return Ok(_taskOrhestrator.GetAllTasksForProject(projectId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error during GET Tasks by ProjectId - {projectId}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
