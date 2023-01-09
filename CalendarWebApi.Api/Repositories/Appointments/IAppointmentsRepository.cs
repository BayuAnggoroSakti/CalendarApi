using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalendarWebApi.Api.Entities;

namespace CalendarWebApi.Api.Repositories.Appointments
{
    public interface IAppointmentsRepository
    {
        Task CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> CheckAppointmentConflictAsync(String username, DateTime? startDate, DateTime? endDate);
        Task<Appointment> CheckAppointmentConflictWithIdAsync(Guid id,String username, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<Appointment>> GetAppointmentsAsync();
        Task<Appointment> GetAppointmentsAsync(Guid id);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(Guid id);

    }
}