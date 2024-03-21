using Explorer.API.Controllers.Tourist.Tourism;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourism
{
    [Collection("Sequential")]

    public class TourPreferenceCommandTests : BaseToursIntegrationTest
    {
        public TourPreferenceCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new TourPreferenceDto
            {
                Id = 1,
                CreatorId = 1,
                Difficulty = "Easy",
                Walk = 1,
                Bike = 2,
                Car = 0,
                Boat = 1,
                Tags = new List<string> { "#streetfood", "#art" },
            };

            // Act
            var result = ((ObjectResult)controller.CreateAsync(newEntity).Result)?.Value as TourPreferenceDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.CreatorId.ShouldBe(newEntity.CreatorId);
            result.CreatorId.ShouldNotBe(0);

            // Assert - Database
            var storedEntity = dbContext.TourPreference.FirstOrDefault(i => i.Id == newEntity.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.Id.ShouldBe(result.Id);
        }

        [Fact]
        public void Create_fails_invalid_data()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new TourPreferenceDto
            {
                Bike = -1,
            };


            // Act
            var result = (ObjectResult)controller.CreateAsync(updatedEntity).Result;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var updatedEntity = new TourPreferenceDto
            {
                Id = -1,
                CreatorId = 3,
                Difficulty = "Easy",
                Walk = 1,
                Bike = 1,
                Car = 1,
                Boat = 1,
                Tags = new List<string> { "#streetfood2", "#art2" },
            };

            // Act
            var result = ((ObjectResult)controller.UpdateAsync(updatedEntity).Result)?.Value as TourPreferenceDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(-1);
            result.CreatorId.ShouldBe(updatedEntity.CreatorId);
            result.Bike.ShouldBe(updatedEntity.Bike);
            result.Car.ShouldBe(updatedEntity.Car);
            result.Boat.ShouldBe(updatedEntity.Boat);
            result.Walk.ShouldBe(updatedEntity.Walk);

            // Assert - Database
            var storedEntity = dbContext.TourPreference.FirstOrDefault(i => i.CreatorId == 3);
            storedEntity.ShouldNotBeNull();
            storedEntity.CreatorId.ShouldBe(updatedEntity.CreatorId);
            var oldEntity = dbContext.TourPreference.FirstOrDefault(i => i.CreatorId == 2);
            oldEntity.ShouldBeNull();
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new TourPreferenceDto
            {
                Id = -10,
                CreatorId = 2,
                Difficulty = "Medium",
                Walk = 1,
                Bike = 1,
                Car = 1,
                Boat = 1,
                Tags = new List<string> { "#streetfood2", "#art2" },
            };

            // Act
            var result = (ObjectResult)controller.UpdateAsync(updatedEntity).Result;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }

        [Fact]
        public void Deletes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Act
            var result = (OkResult)controller.DeleteAsync(-3);

            // Assert - Response
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Assert - Database
            var storedCourse = dbContext.TourPreference.FirstOrDefault(i => i.Id == -3);
            storedCourse.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = (ObjectResult)controller.DeleteAsync(-1000);

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }

        private static TourPreferenceController CreateController(IServiceScope scope)
        {
            return new TourPreferenceController(scope.ServiceProvider.GetRequiredService<ITourPreferenceService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}