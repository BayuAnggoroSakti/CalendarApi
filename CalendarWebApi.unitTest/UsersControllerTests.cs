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

           [Fact]
        public async Task CreateUserAsync_WithUserToCreate_ReturnsCreatedUser()
        {
            // Arrange
            var UserToCreate = new CreateUserDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            var controller = new UsersController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.CreateUserAsync(UserToCreate);

            // Assert
            var createdUser = (result.Result as CreatedAtActionResult).Value as UserDto;
            UserToCreate.Should().BeEquivalentTo(
                createdUser,
                options => options.ComparingByMembers<UserDto>().ExcludingMissingMembers()
            );
            createdUser.Id.Should().NotBeEmpty();
            createdUser.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
        }

         [Fact]
        public async Task UpdateUserAsync_WithExistingUser_ReturnsNoContent()
        {
            // Arrange
            User existingUser = CreateRandomUser();
            repositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);

            var UserId = existingUser.Id;
            var UserToUpdate = new UpdateUserDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            );

            var controller = new UsersController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.UpdateUserAsync(UserId, UserToUpdate);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteUserAsync_WithExistingUser_ReturnsNoContent()
        {
            // Arrange
            User existingUser = CreateRandomUser();
            repositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);

            var controller = new UsersController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.DeleteUserAsync(existingUser.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
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
