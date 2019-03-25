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
    [EnableCors("ProjectManagerApiCors")]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly ITaskLogic taskLogic;
        private readonly ILogger<TasksController> _logger;

        /// <summary>
        /// Injection Constructor
        /// </summary>
        /// <param name="_logic">task Logic layer instance</param>
        /// <param name="logger">Logger instnace</param>
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
        /// <summary>
        /// Get a specific task details
        /// </summary>
        /// <param name="id">Task Id</param>
        /// <returns>Task Entity</returns>
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

        /// <summary>
        /// Creates a new Task
        /// </summary>
        /// <param name="value">Task information</param>
        /// <returns>Created Task information</returns>
        [HttpPost]
        public IActionResult Post([FromBody] Task value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = taskLogic.CreateTask(value);
                    //var createdUrl = string.Join("/", Request.Path, result.TaskId);
                    return Created(string.Concat("/", result.TaskId), result);
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
        /// <summary>
        /// Update a selected task with new information
        /// </summary>
        /// <param name="id">task Id</param>
        /// <param name="value">New information</param>
        /// <returns>boolean status of the update</returns>
        [HttpPut]
        public IActionResult Put(int id, [FromBody]Task value)
        {
            if (value.TaskId != id)
                return BadRequest("Identifier doesnt match");

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
        /// <summary>
        /// Deletes a given task by the Task Id
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Boolean status of the delete request</returns>
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

        /// <summary>
        /// Ends a task by marking the Enddate as today
        /// </summary>
        /// <param name="taskId">Task Id</param>
        /// <returns>Boolean status of the Task End request</returns>
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
