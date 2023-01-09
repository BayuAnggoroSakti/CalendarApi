using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalendarWebApi.Dtos;
using CalendarWebApi.Entities;
using CalendarWebApi.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CalendarWebApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository repository;
        private readonly ILogger<UsersController> logger;

        public UsersController(IUsersRepository repository, ILogger<UsersController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

       // // GET /Users
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsersAsync(string username = null)
        {
            var Users = (await repository.GetUsersAsync())
                        .Select(user => user.AsDto());

            if (!string.IsNullOrWhiteSpace(username))
            {
                Users = Users.Where(user => user.Username.Contains(username, StringComparison.OrdinalIgnoreCase));
            }

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {Users.Count()} Users");

            return Users;
        }

        // // GET /Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid id)
        {
            var user = await repository.GetUserAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            return user.AsDto();
        }
    }
}