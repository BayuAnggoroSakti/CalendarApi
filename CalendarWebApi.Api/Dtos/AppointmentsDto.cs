using System;
using System.ComponentModel.DataAnnotations;

namespace CalendarWebApi.Api.Dtos
{
    public record AppointmentDto(Guid Id, string Username, DateTime? StartDate, DateTime? EndDate, string Remark, DateTimeOffset CreatedDate, DateTimeOffset UpdatedDate);
    public record CreateAppointmentDto([Required] string username,[Required] DateTime? startdate,[Required] DateTime? enddate, [Required] string remark);
    public record UpdateAppointmentDto([Required] string username,[Required] DateTime? startdate,[Required] DateTime? enddate, [Required] string remark);
}