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
        public IEnumerable<Projects> Get()
        {
            return new Projects[] { };
        }

        // GET: api/Projects/5
        [HttpGet("{id}", Name = "GetProject")]
        public Projects Get(int id)
        {
            return new Projects();
        }

        // POST: api/Projects
        [HttpPost]
        public IActionResult Post([FromBody] Projects value)
        {
            return Created("", null);
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Projects value)
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
