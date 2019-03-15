﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PM.Api.Extensions;
using PM.BL.Projects;
using PM.BL.Tasks;
using PM.BL.Users;
using PM.Models.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PM.Api.Controllers
{
    /// <summary>
    /// Users Controller
    /// </summary>
    //[EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IUserLogic _userOrchestrator;
        private IProjectLogic projOrchestrator;
        private ITaskLogic taskOrchestrator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserLogic _userlogicInstance, ILogger<UsersController> logInstance, IProjectLogic _projectLogicInstance, ITaskLogic _taskLogicInstance)
        {
            _userOrchestrator = _userlogicInstance;
            projOrchestrator = _projectLogicInstance;
            taskOrchestrator = _taskLogicInstance;
            _logger = logInstance;
        }

        /// <summary>
        /// GET: api/Users | Get All Users
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        //[ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        public IActionResult GetAllUsers()
        {
            try
            {
                var result = _userOrchestrator.GetUsers();
                _logger.LogDebug("GetAllUsers invoked with count - " + result.Count());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GetAllUsers", ex.InnerException, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get all Active Users
        /// </summary>
        /// <returns>List of Active Users</returns>
        [HttpGet("active")]
        //[ActionName("GetAllUsers")]
        public IActionResult GetActiveUsers()
        {
            try
            {
                return Ok(_userOrchestrator.GetUsers(true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GetAllUsers");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        //[Route("api/users/{UserId:alpha}")]
        //[ActionName("GetById")]
        public IActionResult Get(string id)
        {
            try
            {
                return Ok(_userOrchestrator.GetUserById(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when trying to GET User by {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/Users
        [HttpPost]
        public IActionResult Post([FromBody] User value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _userOrchestrator.AddUser(value);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during Creating a new user. Data attempted in JSON format: {0}", value.Stringify());
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Invalid/Incomplete User Information - {0}", value.Stringify());
                return BadRequest("Invalid request information. Please verify the information entered.");
            }
        }

        // PUT: api/Users/5
        [HttpPut]
        public IActionResult Put(string id, [FromBody]User value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _userOrchestrator.EditUser(id, value);
                    if (result)
                        return Ok(result);
                    else
                        return NotFound();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during Update with the values supplied in JSON Format - {0}", value.Stringify());
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Invalid input during Update for the User - {1}. Check the model state information - {0}", ModelState.Values.Stringify(), id);
                return BadRequest("Invalid request information. Please verify the information entered.");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete]
        public IActionResult Delete(string id)
        {
            try
            {
                var result = _userOrchestrator.DeleteUser(id);
                _logger.LogWarning($"User {id} was attempted to be deleted and it status - {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Delete for UserId - {0}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Search for users with any keyword that may match their Firstname, Last name or User ID
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="matchExact"></param>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        [HttpGet("Search")]
        [ActionName("Search")]
        public IActionResult Search(string keyword, bool matchExact = false, string fieldType = "")
        {
            try
            {
                return Ok(_userOrchestrator.Search(keyword, matchExact, fieldType));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during Search by {keyword} with additional params exactMatch-{matchExact} and fieldType- {fieldType}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Returns list of projects associated to the manager by UserId
        /// </summary>
        /// <param name="userId">UserId of the manager</param>
        /// <returns>List of Projects belonging to the User</returns>
        /// <example>api/Users/user1/Projects</example>
        [HttpGet("{userId}/Projects")]
        //[Route("api/Users/{userId}/Projects")]
        public IActionResult GetUserProjects(string userId)
        {
            return Ok(projOrchestrator.GetUserProjects(userId));
        }

        // GET: api/Users/{UserId}/Tasks
        [HttpGet("{UserId}/Tasks")]
        public IActionResult GetAllTasksForUser(string UserId)
        {
            try
            {
                return Ok(taskOrchestrator.GetAllTasksForUser(UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during GET Tasks by User Id - {UserId}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Users/{UserId}/Projects/{ProjectId}/Tasks
        [HttpGet("{UserId}/Projects/{projId}/Tasks")]
        public IActionResult GetAllTasksForUserByProject(string UserId, int projId)
        {
            try
            {
                return Ok(taskOrchestrator.GetUserProjectTasks(UserId, projId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during GET Tasks by User Id - {UserId} by Project Id - {projId}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}