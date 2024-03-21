using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Author.Administration;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class CheckpointCommandTests : BaseToursIntegrationTest
    {
        public CheckpointCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new CheckpointDto
            {
                TourId = -1,
                AuthorId = -12,
                Longitude = 45,
                Latitude = 45,
                Name = "Katedrala",
                Description = "Objekat se nalazi na centru Novog Sada",
                Pictures = new List<string> { "https://www.google.com/url?sa=i&url=https%3A%2F%2Filovenovisad.com%2Fvodic%2Fsakralni-objekti%2Fkatolicka-crkva-ime-marijino%2F&psig=AOvVaw2UZQxIT-Z_WvM0CTWNA8yC&ust=1697737791159000&source=images&cd=vfe&opi=89978449&ved=0CA8QjRxqFwoTCNjjwaGUgIIDFQAAAAAdAAAAABAD" },
                EncounterId = 0,
                IsSecretPrerequisite = false
            };

            // Act
            var result = ((ObjectResult)controller.CreateAsync(newEntity, "Private").Result)?.Value as CheckpointDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.TourId.ShouldNotBe(0);
            result.Longitude.ShouldBe(newEntity.Longitude);
            result.Latitude.ShouldBe(newEntity.Latitude);
            result.Name.ShouldBe(newEntity.Name);
            result.Description.ShouldBe(newEntity.Description);
            result.Pictures.ShouldBe(newEntity.Pictures);

            // Assert - Database
            var storedEntity = dbContext.Checkpoints.FirstOrDefault(i => i.Name == newEntity.Name);
            storedEntity.ShouldNotBeNull();
            storedEntity.Id.ShouldBe(result.Id);
        }

        [Fact]
        public void Create_fails_invalid_data()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new CheckpointDto
            {
                Name = "Test",
                TourId = -1,
                Pictures = new List<string> { "https://www.google.com/url?sa=i&url=https%3A%2F%2Filovenovisad.com%2Fvodic%2Fsakralni-objekti%2Fkatolicka-crkva-ime-marijino%2F&psig=AOvVaw2UZQxIT-Z_WvM0CTWNA8yC&ust=1697737791159000&source=images&cd=vfe&opi=89978449&ved=0CA8QjRxqFwoTCNjjwaGUgIIDFQAAAAAdAAAAABAD" }
            };

            // Act
            //var result = (ObjectResult)controller.Create(updatedEntity, 2).Result;

            // Assert
            //result.ShouldNotBeNull();
            //result.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var updatedEntity = new CheckpointDto
            {
                Id = -2,
                TourId = -1,
                AuthorId = -12,
                Longitude = 45,
                Latitude = 45,
                Name = "Petrovaradin",
                Description = "Tvrdjava",
                Pictures = new List<string> { "https://www.google.com/url?sa=i&url=https%3A%2F%2Fen.wikipedia.org%2Fwiki%2FPetrovaradin_Fortress&psig=AOvVaw0IqEUr-KgumgRwJuFwJfy5&ust=1697738244154000&source=images&cd=vfe&opi=89978449&ved=0CA8QjRxqFwoTCJiG3vmVgIIDFQAAAAAdAAAAABAD" },
                EncounterId = 0,
                IsSecretPrerequisite = false
            };

            // Act
            var result = ((ObjectResult)controller.Update(updatedEntity, -2).Result)?.Value as CheckpointDto;

            // Assert - Response

            result.ShouldNotBeNull();
            result.Id.ShouldBe(-2);
            result.TourId.ShouldNotBe(0);
            result.Longitude.ShouldBe(updatedEntity.Longitude);
            result.Latitude.ShouldBe(updatedEntity.Latitude);
            result.Name.ShouldBe(updatedEntity.Name);
            result.Description.ShouldBe(updatedEntity.Description);
            result.Pictures.ShouldBe(updatedEntity.Pictures);

            // Assert - Database
            var storedEntity = dbContext.Checkpoints.FirstOrDefault(i => i.Name == "Petrovaradin");
            storedEntity.ShouldNotBeNull();
            storedEntity.TourId.ShouldBe(updatedEntity.TourId);
            storedEntity.Longitude.ShouldBe(updatedEntity.Longitude);
            storedEntity.Latitude.ShouldBe(updatedEntity.Latitude);
            storedEntity.Description.ShouldBe(updatedEntity.Description);
            storedEntity.Pictures.ShouldBe(updatedEntity.Pictures);
            var oldEntity = dbContext.Checkpoints.FirstOrDefault(i => i.Name == "Fakultet tehnickih nauka");
            oldEntity.ShouldBeNull();
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new CheckpointDto
            {
                Id = -1000,
                TourId = -1,
                AuthorId = -12,
                Longitude = 45,
                Latitude = 45,
                Name = "Test",
                Description = "Test",
                Pictures = new List<string> { "https://www.google.com/url?sa=i&url=https%3A%2F%2Fen.wikipedia.org%2Fwiki%2FPetrovaradin_Fortress&psig=AOvVaw0IqEUr-KgumgRwJuFwJfy5&ust=1697738244154000&source=images&cd=vfe&opi=89978449&ved=0CA8QjRxqFwoTCJiG3vmVgIIDFQAAAAAdAAAAABAD" }
            };

            // Act
            var result = (ObjectResult)controller.Update(updatedEntity, -1000).Result;

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
            var result = (OkResult)controller.Delete(-1);

            // Assert - Response
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Assert - Database
            var storedCourse = dbContext.Checkpoints.FirstOrDefault(i => i.Id == 1);
            storedCourse.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = (ObjectResult)controller.Delete(-1000);

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }

        private static CheckpointController CreateController(IServiceScope scope)
        {
            return new CheckpointController(scope.ServiceProvider.GetRequiredService<ICheckpointService>())
            {
                ControllerContext = BuildContext("-12")
            };
        }
    }
}