using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalendarWebApi.Api.Dtos;
using CalendarWebApi.Api.Entities;
using CalendarWebApi.Api.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CalendarWebApi.Api.Controllers
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

          // POST /Users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUserAsync(CreateUserDto userDto)
        {
            //check username existing
            var dataUsers = (await repository.GetUsersAsync())
                        .Select(user => user.AsDto());
            dataUsers = dataUsers.Where(user => user.Username.Contains(userDto.username, StringComparison.OrdinalIgnoreCase));
            if (dataUsers.Count() > 0)
            {
                return BadRequest(new { status = false, message = "Cannot insert duplicate username"});
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.password);
            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = userDto.username,
                Password = passwordHash,
                FullName = userDto.fullname,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetUsersAsync), new { id = user.Id }, user.AsDto());
        }

        // // PUT /Users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserAsync(Guid id, UpdateUserDto userDto)
        {
            var existingUser = await repository.GetUserAsync(id);

            if (existingUser is null)
            {
                return NotFound();
            }

            existingUser.FullName = userDto.fullname;
            existingUser.Username = userDto.username;

            await repository.UpdateUserAsync(existingUser);

            return Ok(new { status = true, message = "Update user is succesfully"});
        }

        // DELETE /Users/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserAsync(Guid id)
        {
            var existingUser = await repository.GetUserAsync(id);

            if (existingUser is null)
            {
                return NotFound();
            }

            await repository.DeleteUserAsync(id);

             return Ok(new { status = true, message = "Delete user is succesfully"});
        }
    }
}