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

         public static AppointmentDto AsDto(this Appointment appointment)
        {
            return new AppointmentDto(appointment.Id, appointment.Username, appointment.StartDate, appointment.EndDate,appointment.Remark ,appointment.CreatedDate, appointment.UpdatedDate);
        }
    }
}