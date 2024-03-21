using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Author.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class MapObjectQueryTests : BaseToursIntegrationTest
{
    public MapObjectQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void RetrievesAll()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateMapObjectController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAllAsync(0, 0).Result)?.Value as PagedResult<MapObjectDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(3);
        result.TotalCount.ShouldBe(3);
    }

    private static MapObjectController CreateMapObjectController(IServiceScope scope)
    {
        return new MapObjectController(scope.ServiceProvider.GetRequiredService<IMapObjectService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
