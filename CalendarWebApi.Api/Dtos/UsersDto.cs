using System;
using System.ComponentModel.DataAnnotations;

namespace CalendarWebApi.Api.Dtos
{
    public record UserDto(Guid Id, string Username, string Password, string FullName, DateTimeOffset CreatedDate);
    public record CreateUserDto([Required] string username,[Required] string password, [Required] string fullname);
    public record UpdateUserDto([Required] string username,string password,string fullname);
}