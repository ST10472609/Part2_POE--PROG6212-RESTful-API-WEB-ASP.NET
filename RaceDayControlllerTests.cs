using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaceDay_API.Controllers;
using RaceDay_API.Data;
using RaceDay_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RaceDayApi.Tests
{
    public class RaceDayControllerTests
    {
        private UsersDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new UsersDbContext(options);

            context.Participants.Add(new Participants
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                Age = 30,
                Weight = 70.5,
                Height = 175,
                Username = "johndoe",
                PasswordHash = "hashedpassword",
                Role = "Participant"
            });

            context.SaveChanges();

            return context;
        }

        private IConfiguration GetConfiguration()
        {
            var settings = new Dictionary<string, string>();

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        [Fact]
        public async Task CreateParticipant_ReturnsCreatedParticipant()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UsersDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new UsersDbContext(options);

            var controller = new RaceDayController(
                context,
                GetConfiguration());

            var participant = new Participants
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                Age = 30,
                Weight = 70.5,
                Height = 175,
                Username = "johndoe",
                PasswordHash = "hashedpassword",
                Role = "Participant"
            };

            // Act
            var result = await controller.CreateParticipant(participant);

            // Assert
            var createdResult =
                Assert.IsType<CreatedAtActionResult>(result.Result);

            var createdParticipant =
                Assert.IsType<Participants>(createdResult.Value);

            Assert.Equal("John", createdParticipant.Name);
        }

        [Fact]
        public async Task GetParticipants_ReturnsAllParticipants()
        {
            // Arrange
            var context = GetDatabaseContext();

            var controller = new RaceDayController(
                context,
                GetConfiguration());

            // Act
            var result = await controller.GetParticipants();

            // Assert
            Assert.NotNull(result);

            var participants = result.Value;

            Assert.NotNull(participants);
            Assert.Single(participants);
        }

        [Fact]
        public async Task GetParticipant_ReturnsParticipant_WhenParticipantExists()
        {
            // Arrange
            var context = GetDatabaseContext();

            var controller = new RaceDayController(
                context,
                GetConfiguration());

            // Act
            var result = await controller.GetParticipant(1);

            // Assert
            Assert.NotNull(result.Value);
            Assert.Equal("John", result.Value.Name);
        }

        [Fact]
        public async Task GetParticipant_ReturnsNotFound_WhenParticipantDoesNotExist()
        {
            // Arrange
            var context = GetDatabaseContext();

            var controller = new RaceDayController(
                context,
                GetConfiguration());

            // Act
            var result = await controller.GetParticipant(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateParticipant_ReturnsNoContent_WhenParticipantExists()
        {
            // Arrange
            var context = GetDatabaseContext();

            var controller = new RaceDayController(
                context,
                GetConfiguration());

            var participant = await context.Participants.FirstAsync();

            participant.Name = "Updated John";

            // Act
            var result =
                await controller.UpdateParticipant(
                    participant.Id,
                    participant);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var updatedParticipant =
                await context.Participants.FindAsync(1);

            Assert.Equal(
                "Updated John",
                updatedParticipant.Name);
        }

        [Fact]
        public async Task DeleteParticipant_RemovesParticipant()
        {
            // Arrange
            var context = GetDatabaseContext();

            var controller = new RaceDayController(
                context,
                GetConfiguration());

            // Act
            var result =
                await controller.DeleteParticipant(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var participants =
                await context.Participants.ToListAsync();

            Assert.Empty(participants);
        }
    }
}