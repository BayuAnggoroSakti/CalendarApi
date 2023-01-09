using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarWebApi.Api.Dtos;
using CalendarWebApi.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CalendarWebApi.Api.Repositories.Appointments;
using CalendarWebApi.Api.Repositories.Users;

namespace CalendarWebApi.Api.Controllers
{
    [ApiController]
    [Route("appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentsRepository repository;
        private readonly IUsersRepository userRepository;
        private readonly ILogger<AppointmentsController> logger;

        public AppointmentsController(IAppointmentsRepository repository,IUsersRepository userRepository, ILogger<AppointmentsController> logger)
        {
            this.repository = repository;
            this.userRepository = userRepository;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto appointmentDto)
        {
            //check username is exist
            var user = await userRepository.GetUserByUsernameAsync(appointmentDto.username);
            if (user is null)
            {
                return NotFound(new { status = false, message = "Username not found, please make sure username is exist"});
            }

            //check startdate < enddate
            if (appointmentDto.startdate>appointmentDto.enddate)
            {
                return BadRequest(new { status = false, message = "EndDate must be greater than StartDate"});
            }

            //check conflict appointment
            var appointmentConflict = await repository.CheckAppointmentConflictAsync(appointmentDto.username, appointmentDto.startdate, appointmentDto.enddate);
            if (appointmentConflict is not null)
            {
                return BadRequest(new { status = false, message = "This user already has an appointment on the date"});
            }

            Appointment appointment = new()
            {
                Id = Guid.NewGuid(),
                Username = appointmentDto.username,
                StartDate = appointmentDto.startdate,
                EndDate = appointmentDto.enddate,
                Remark = appointmentDto.remark,
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateAppointmentAsync(appointment);
            return CreatedAtAction(nameof(GetAppointmentsAsync), new { id = appointment.Id }, appointment.AsDto());
        }

        [HttpGet]
        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync(string username = null)
        {
            var Appointments = (await repository.GetAppointmentsAsync())
                        .Select(appointment => appointment.AsDto());

            if (!string.IsNullOrWhiteSpace(username))
            {
                Appointments = Appointments.Where(appointment => appointment.Username.Contains(username, StringComparison.OrdinalIgnoreCase));
            }

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {Appointments.Count()} Appointment");

            return Appointments;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentAsync(Guid id)
        {
            var appointment = await repository.GetAppointmentsAsync(id);

            if (appointment is null)
            {
                return NotFound();
            }

            return appointment.AsDto();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAppointmentAsync(Guid id, UpdateAppointmentDto appointmentDto)
        {
            var existingAppointment = await repository.GetAppointmentsAsync(id);

            if (existingAppointment is null)
            {
                return NotFound();
            }

            //check username is exist
            var user = await userRepository.GetUserByUsernameAsync(appointmentDto.username);
            if (user is null)
            {
                return NotFound(new { status = false, message = "Username not found, please make sure username is exist"});
            }

            //check startdate < enddate
            if (appointmentDto.startdate>appointmentDto.enddate)
            {
                return BadRequest(new { status = false, message = "EndDate must be greater than StartDate"});
            }

            //check conflict appointment
            var appointmentConflict = await repository.CheckAppointmentConflictWithIdAsync(id,appointmentDto.username, appointmentDto.startdate, appointmentDto.enddate);
            if (appointmentConflict is not null)
            {
                return BadRequest(new { status = false, message = "This user already has an appointment on the date"});
            }

            existingAppointment.Username = appointmentDto.username;
            existingAppointment.StartDate = appointmentDto.startdate;
            existingAppointment.EndDate = appointmentDto.enddate;
            existingAppointment.Remark = appointmentDto.remark;
            existingAppointment.UpdatedDate = DateTimeOffset.UtcNow;

            await repository.UpdateAppointmentAsync(existingAppointment);

            return Ok(new { status = true, message = "Update appointment is succesfully"});
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointmentAsync(Guid id)
        {
            var existingAppointment = await repository.GetAppointmentsAsync(id);

            if (existingAppointment is null)
            {
                return NotFound();
            }

            await repository.DeleteAppointmentAsync(id);

             return Ok(new { status = true, message = "Delete appointment is succesfully"});
        }
    }
}