using System;
using Xunit;
using Moq;
using CalendarWebApi.Api.Controllers;
using CalendarWebApi.Api.Dtos;
using CalendarWebApi.Api.Entities;
using CalendarWebApi.Api.Repositories.Users;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CalendarWebApi.unitTest
{
    public class UsersControllerTest
    {
        private readonly Mock<IUsersRepository> repositoryStub = new();
        private readonly Mock<ILogger<UsersController>> loggerStub = new();
        private readonly Random rand = new();
        [Fact]
        public async Task GetUsersync_WithUnexistingUser_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            var controller = new UsersController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.GetUserAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

         [Fact]
        public async Task GetUserAsync_WithExistingUser_ReturnsExpectedUser()
        {
            // Arrange
            User expectedUser = CreateRandomUser();

            repositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedUser);

            var controller = new UsersController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.GetUserAsync(Guid.NewGuid());

            // Assert
            result.Value.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetUsersAsync_WithExistingUsers_ReturnsAllUsers()
        {
            // Arrange
            var expectedUsers = new[] { CreateRandomUser(), CreateRandomUser(), CreateRandomUser() };

            repositoryStub.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(expectedUsers);

            var controller = new UsersController(repositoryStub.Object, loggerStub.Object);

            // Act
            var actualUsers = await controller.GetUsersAsync();

            // Assert
            actualUsers.Should().BeEquivalentTo(expectedUsers);
        }

         [Fact]
        public async Task GetUsersAsync_WithMatchingUsers_ReturnsMatchingUsers()
        {
            // Arrange
            var allUsers = new[]
            {
                new User(){ Username = "User1"},
                new User(){ Username = "User2"},
                new User(){ Username = "User3"}
            };

            var nameToMatch = "User1";

            repositoryStub.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(allUsers);

            var controller = new UsersController(repositoryStub.Object, loggerStub.Object);

            // Act
            IEnumerable<UserDto> foundUsers = await controller.GetUsersAsync(nameToMatch);

            // Assert
            foundUsers.Should().OnlyContain(
                User => User.Username == allUsers[0].Username || User.Username == allUsers[2].Username
            );
        }
        private User CreateRandomUser()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                FullName = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
