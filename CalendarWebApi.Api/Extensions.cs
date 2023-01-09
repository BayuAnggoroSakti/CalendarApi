using CalendarWebApi.Api.Dtos;
using CalendarWebApi.Api.Entities;

namespace CalendarWebApi.Api
{
    public static class Extensions
    {
        public static UserDto AsDto(this User user)
        {
            return new UserDto(user.Id, user.Username, user.Password, user.FullName, user.CreatedDate);
        }
    }
}