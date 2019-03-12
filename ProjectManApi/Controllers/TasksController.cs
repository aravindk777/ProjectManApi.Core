using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Api.Extensions;
using PM.BL.Tasks;
using PM.Models.ViewModels;
using System;

namespace PM.Api.Controllers
{
    /// <summary>
    /// Tasks Controller
    /// </summary>
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly ITaskLogic taskLogic;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskLogic _logic, ILogger<TasksController> logger)
        {
            taskLogic = _logic;
            _logger = logger;
        }

        // GET: api/Tasks
        /// <summary>
        /// Get all Tasks
        /// </summary>
        /// <returns>List of all Tasks</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(taskLogic.GetTasks());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GET All Tasks");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var result = taskLogic.GetTask(id);
                if (result == null)
                {
                    _logger.LogWarning($"No data available for GET Task by ID - {id}.");
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during GET Task by Id - {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Tasks/taskname
        //[HttpGet]
        //[ActionName("GetByName")]
        //public IActionResult GetByName(string name)
        //{
        //    try
        //    {
        //        var result = taskLogic.GetTask(0, name);
        //        if (result == null)
        //        {
        //            _logger.LogWarning($"No data available for GET Task by Name: {name}");
        //            return NotFound();
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error during GET Task by Name: {name}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        // POST: api/Tasks


        [HttpPost]
        public IActionResult Post([FromBody] Task value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = taskLogic.CreateTask(value);
                    var createdUrl = string.Join("/", Request.Path, result.TaskId);
                    return Created(createdUrl, result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during Creating new Task with value - {value.Stringify()}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Invalid/Incomplete Task Information - {0}", value.Stringify());
                return BadRequest(ModelState); //"Invalid request information. Please verify the information entered.", 
            }
        }

        // PUT: api/Tasks/5
        [HttpPut]
        public IActionResult Put(int id, [FromBody]Task value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return Ok(taskLogic.UpdateTask(id, value));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during Updating Task by Id - {id} with new values: {value.Stringify()}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Invalid/Incomplete Task Information - {0}", value.Stringify());
                return BadRequest(ModelState); //"Invalid request information. Please verify the information entered.", 
            }
        }

        // DELETE: api/Tasks/5
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                return Ok(taskLogic.DeleteTask(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during Deleting Task Id - {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //// GET: api/Projects/{ProjId}/Tasks
        //[HttpGet("{ProjectId}/Tasks")]
        //[Route("api/Projects/{ProjectId}/Tasks")]
        //public IActionResult GetAllTasksForProject(int projectId)
        //{
        //    try
        //    {
        //        return Ok(taskLogic.GetAllTasksForProject(projectId));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error during GET Tasks by ProjectId - {projectId}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        //// GET: api/Users/{UserId}/Tasks
        //[HttpGet("Users/{UserId}/Tasks")]
        //[Route("api/Users/{UserId}/Tasks")]
        //public IActionResult GetAllTasksForUser(string UserId)
        //{
        //    try
        //    {
        //        return Ok(taskLogic.GetAllTasksForUser(UserId));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error during GET Tasks by User Id - {UserId}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        //// GET: api/Users/{UserId}/Projects/{ProjectId}/Tasks
        //[HttpGet("Users/{UserId}/Projects/{projId}/Tasks")]
        //[Route("api/Users/{UserId}/Projects/{projId}/Tasks")]
        //public IActionResult GetAllTasksForUserByProject(string UserId, int projId)
        //{
        //    try
        //    {
        //        return Ok(taskLogic.GetUserProjectTasks(UserId, projId));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error during GET Tasks by User Id - {UserId} by Project Id - {projId}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("api/Tasks/{taskId}/End")]
        public IActionResult EndTask(int taskId)
        {
            try
            {
                return Ok(taskLogic.EndTask(taskId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during Ending the Task Id - {taskId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
