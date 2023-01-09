using System;
using Xunit;
using Moq;
using CalendarWebApi.Api.Controllers;
using CalendarWebApi.Api.Dtos;
using CalendarWebApi.Api.Entities;
using CalendarWebApi.Api.Repositories.Appointments;
using CalendarWebApi.Api.Repositories.Users;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace appointment.unitTest
{
    public class AppointmentControllerTest
    {
        private readonly Mock<IUsersRepository> userRepositoryStub = new();
         private readonly Mock<IAppointmentsRepository> repositoryStub = new();
        private readonly Mock<ILogger<AppointmentsController>> loggerStub = new();
        private readonly Random rand = new();
        [Fact]
        public async Task GetAppointmentAsync_WithUnexistingAppointments_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetAppointmentsAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Appointment)null);

            var controller = new AppointmentsController(repositoryStub.Object,userRepositoryStub.Object ,loggerStub.Object);

            // Act
            var result = await controller.GetAppointmentAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

         [Fact]
        public async Task GetAppointmentAsync_WithExistingAppointment_ReturnsExpectedAppointment()
        {
            // Arrange
            Appointment expectedAppointment = CreateRandomAppointment();

            repositoryStub.Setup(repo => repo.GetAppointmentsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedAppointment);

            var controller = new AppointmentsController(repositoryStub.Object,userRepositoryStub.Object ,loggerStub.Object);

            // Act
            var result = await controller.GetAppointmentAsync(Guid.NewGuid());

            // Assert
            result.Value.Should().BeEquivalentTo(expectedAppointment);
        }

        [Fact]
        public async Task GetAppointmentsAsync_WithExistingAppointments_ReturnsAllAppointments()
        {
            // Arrange
            var expectedAppointments = new[] { CreateRandomAppointment(), CreateRandomAppointment(), CreateRandomAppointment() };

            repositoryStub.Setup(repo => repo.GetAppointmentsAsync())
                .ReturnsAsync(expectedAppointments);

            var controller = new AppointmentsController(repositoryStub.Object, userRepositoryStub.Object ,loggerStub.Object);

            // Act
            var actualAppointments = await controller.GetAppointmentsAsync();

            // Assert
            actualAppointments.Should().BeEquivalentTo(expectedAppointments);
        }

         [Fact]
        public async Task GetAppointmentsAsync_WithMatchingAppointments_ReturnsMatchingAppointments()
        {
            // Arrange
            var allAppointments = new[]
            {
                new Appointment(){ Username = "User1"},
                new Appointment(){ Username = "User2"},
                new Appointment(){ Username = "User3"}
            };

            var nameToMatch = "User1";

            repositoryStub.Setup(repo => repo.GetAppointmentsAsync())
                .ReturnsAsync(allAppointments);

            var controller = new AppointmentsController(repositoryStub.Object,userRepositoryStub.Object, loggerStub.Object);

            // Act
            IEnumerable<AppointmentDto> foundAppointments = await controller.GetAppointmentsAsync(nameToMatch);

            // Assert
            foundAppointments.Should().OnlyContain(
                Appointment => Appointment.Username == allAppointments[0].Username || Appointment.Username == allAppointments[2].Username
            );
        }

        [Fact]
        public async Task DeleteAppointmentAsync_WithExistingAppointment_ReturnsNoContent()
        {
            // Arrange
            Appointment existingAppointment = CreateRandomAppointment();
            repositoryStub.Setup(repo => repo.GetAppointmentsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingAppointment);

            var controller = new AppointmentsController(repositoryStub.Object, userRepositoryStub.Object ,loggerStub.Object);

            // Act
            var result = await controller.DeleteAppointmentAsync(existingAppointment.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }


        private Appointment CreateRandomAppointment()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Username = Guid.NewGuid().ToString(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                Remark = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
