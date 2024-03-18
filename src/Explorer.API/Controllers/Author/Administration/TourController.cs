using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.Core.Domain.Encounters;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Explorer.API.Controllers.Author.Administration
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/administration/tour")]
    public class TourController : BaseApiController
    {
        private readonly ITourService _tourService;
        private string microserviceUrl = "http://localhost:8081";
        private HttpClient _httpClient = new HttpClient();


        public TourController(ITourService tourService)
        {
            _tourService = tourService;

        }

        [HttpPost]
        public async Task<ActionResult<TourDto>> Create([FromBody] TourDto tour)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(tour);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{microserviceUrl}/tours", httpContent);

                // Check the response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var receivedTour = JsonConvert.DeserializeObject<TourDto>(responseContent);
                        return Ok(receivedTour);
                    }
                    catch (JsonSerializationException ex)
                    {
                        Console.WriteLine($"Error deserializing JSON response: {ex.Message}");
                        // Log or handle the deserialization error
                        return StatusCode(500); // Return an appropriate status code
                    }
                }
                else
                {
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult<TourDto> Update([FromBody] TourDto tour)
        {
            tour.Equipment = new List<EquipmentDto>();
            var result = _tourService.Update(tour, User.PersonId());
            return CreateResponse(result);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result = _tourService.Delete(id, User.PersonId());
            return CreateResponse(result);
        }

        [HttpGet("by-author")]
        public ActionResult<List<TourDto>> GetToursByAuthor([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourService.GetToursByAuthor(page, pageSize, User.PersonId());
            return CreateResponse(result);
        }

        [HttpGet]
        public ActionResult<PagedResult<TourDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourService.GetPaged(page, pageSize);
            return CreateResponse(result);
        }

        [HttpPut("add/{tourId:int}/{equipmentId:int}")]
        public ActionResult<TourDto> AddEquipment(int tourId, int equipmentId)
        {
            var result = _tourService.AddEquipment(tourId, equipmentId, User.PersonId());
            return CreateResponse(result);
        }

        [HttpPut("remove/{tourId:int}/{equipmentId:int}")]
        public ActionResult<TourDto> RemoveEquipment(int tourId, int equipmentId)
        {
            var result = _tourService.RemoveEquipment(tourId, equipmentId, User.PersonId());
            return CreateResponse(result);
        }

        [HttpGet("details/{id:int}")]
        public ActionResult<TourDto> Get(int id)
        {
            var result = _tourService.Get(id);
            return CreateResponse(result);
        }

        [HttpPut("publishedTours/{id:int}")]
        public ActionResult<TourDto> Publish(int id)
        {
            var result = _tourService.Publish(id, User.PersonId());
            return CreateResponse(result);
        }

        [HttpPut("archivedTours/{id:int}")]
        public ActionResult<TourDto> Archive(int id)
        {
            var result = _tourService.Archive(id, User.PersonId());
            return CreateResponse(result);
        }

        [HttpPut("tourTime/{id:int}")]
        public ActionResult<TourDto> AddTime(TourTimesDto tourTimesDto, int id)
        {
            var result = _tourService.AddTime(tourTimesDto, id, User.PersonId());
            return CreateResponse(result);
        }

    }
}
