using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.Models.ViewModels;

namespace PM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        // GET: api/Projects
        /// <summary>
        /// Get all Projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Project> Get()
        {
            return new Project[] { };
        }

        // GET: api/Projects/5
        [HttpGet("{id}", Name = "GetProject")]
        public Project Get(int id)
        {
            return new Project();
        }

        // POST: api/Projects
        [HttpPost]
        public IActionResult Post([FromBody] Project value)
        {
            return Created("", null);
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Project value)
        {
            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
