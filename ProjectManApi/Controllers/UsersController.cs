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
    public class UsersController : ControllerBase
    {
        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return new List<User>();
        }

        // GET: api/Users/5
        [HttpGet("{id}", Name = "Get")]
        public User Get(string id)
        {
            return new User();
        }

        // POST: api/Users
        [HttpPost]
        public void Post([FromBody] User value)
        {
        }

        // PUT: api/Users/5
        [HttpPut("{UserId}")]
        public void Put(string id, [FromBody] User value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{UserId}")]
        public void Delete(string id)
        {
        }
    }
}
