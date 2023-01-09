using System;

namespace CalendarWebApi.Api.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remark { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}