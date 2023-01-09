using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalendarWebApi.Api.Entities;

namespace CalendarWebApi.Api.Repositories.Users
{
    public interface IUsersRepository
    {
        Task<User> GetUserAsync(Guid id);
        Task<User> GetUserByUsernameAsync(String username);
        Task<IEnumerable<User>> GetUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
    }
}