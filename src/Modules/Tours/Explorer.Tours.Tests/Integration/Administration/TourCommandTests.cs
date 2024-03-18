using Explorer.Tours.API.Dtos;
using Explorer.API.Controllers.Author.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Tours.API.Public.Administration;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]

public class TourCommandTests: BaseToursIntegrationTest
{
    public TourCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new TourDto
        {
            Name = "Planinarenje po Kopaoniku",
            Description = "Višednevno šetanje po planinskim predelima Kopaonika i okoline",
            DemandignessLevel = "Easy",
            Price = 100,
            Tags = new List<string> { "planina","priroda"},
            AuthorId = 2,
            Status = "Draft"
        };

        // Act
        var result = ((ObjectResult)controller.CreateAsync(newEntity).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.AuthorId.ShouldBe(newEntity.AuthorId);
        result.AuthorId.ShouldNotBe(0);

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new TourDto
        {
            Description = "Planinarenje u jesen"
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
        var equipmentDto1 = new EquipmentDto { Id = 1, Name = "Kanap", Description = "Dugacak" };
        var equipmentDto2 = new EquipmentDto { Id = 1, Name = "Jakna", Description = "Vodootporna" };
        var updatedEntity = new TourDto
        {
            Id=-1,
            Name = "Obilazak nekoliko lepih mesta po Vojvodini",
            Description = "Višednevno putovanje autobusom po Vojvodini",
            DemandignessLevel = "Easy",
            Price = 100,
            Tags = new List<string> { "priroda", "vojvodina" },
            AuthorId = -12,
            Status = "Draft",
            Equipment = new List<EquipmentDto>()
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(i => i.Name == "Obilazak nekoliko lepih mesta po Vojvodini");
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        var oldEntity = dbContext.Tours.FirstOrDefault(i => i.Name == "Obilazak Novog Sada");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        //var equipmentDto1 = new EquipmentDto{Id = 1, Name = "Kanap", Description = "Dugacak"};
        //var equipmentDto2 = new EquipmentDto{Id = 1, Name = "Jakna", Description = "Vodootporna"};
        var updatedEntity = new TourDto
        {
            Id = -1000,
            Name = "Planinarenje po Kopaoniku",
            Description = "Višednevno šetanje po planinskim predelima Kopaonika i okoline",
            DemandignessLevel = "Easy",
            Price = 100,
            Tags = new List<string> { "planina", "priroda" },
            AuthorId = -12,
            Status = "Draft",
            Equipment = new List<EquipmentDto>()
        };

        // Act
        var result = (ObjectResult)controller.Update(updatedEntity).Result;

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
        var result = (OkResult)controller.Delete(-2);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedCourse = dbContext.Tours.FirstOrDefault(i => i.Id == -2);
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

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-12")
        };
    }
}