using System;

namespace CalendarWebApi.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}