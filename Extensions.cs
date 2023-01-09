using CalendarWebApi.Dtos;
using CalendarWebApi.Entities;

namespace CalendarWebApi
{
    public static class Extensions
    {
        public static UserDto AsDto(this User user)
        {
            return new UserDto(user.Id, user.Username, user.Password, user.FullName, user.CreatedDate);
        }
    }
}