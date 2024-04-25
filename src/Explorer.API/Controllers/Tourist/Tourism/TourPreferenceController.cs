using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.Core.Domain;
using System.Text;

namespace Explorer.API.Controllers.Tourist.Tourism
{

    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourism/preference")]
    public class TourPreferenceController : BaseApiController
    {
        private readonly ITourPreferenceService _tourPreferenceService;
        private readonly HttpClient _httpClient;

        public TourPreferenceController(ITourPreferenceService tourPreferenceService)
        {
            _tourPreferenceService = tourPreferenceService;
            _httpClient = new HttpClient();
        }

        [HttpPost]
        public async Task<ActionResult<TourPreferenceDto>> CreateAsync([FromBody] TourPreferenceDto preference)
        {
            //var result = _tourPreferenceService.Create(preference);
            //return CreateResponse(result);

            var microserviceUrl = "http://tours_server:8081";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(preference);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{microserviceUrl}/tourPreferences", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var createdTourPreference = JsonConvert.DeserializeObject<MapObjectDto>(responseBody);
                    return Ok(createdTourPreference);
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TourPreferenceDto>> UpdateAsync([FromBody] TourPreferenceDto preference)
        {
            //var result = _tourPreferenceService.Update(preference);
            //return CreateResponse(result);
            var microserviceUrl = "http://tours_server:8081";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(preference);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{microserviceUrl}/tourPreferences", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var updatedTourPreference = JsonConvert.DeserializeObject<MapObjectDto>(responseBody);
                    return Ok(updatedTourPreference);
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            //var result = _tourPreferenceService.Delete(id);
            //return CreateResponse(result);
            var microserviceUrl = "http://tours_server:8081";

            try
            {
                var response = await _httpClient.DeleteAsync($"{microserviceUrl}/tourPreferences/{id}");
                return StatusCode((int)response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<List<TourPreferenceDto>>> GetPreferenceByCreatorAsync([FromQuery] int page, [FromQuery] int pageSize, int id)
        {
            //var result = _tourPreferenceService.GetPreferenceByCreator(page, pageSize, id);
            //return CreateResponse(result);
            var microserviceUrl = "http://tours_server:8081";

            try
            {
                var response = await _httpClient.GetAsync($"{microserviceUrl}/tourPreferences/get-by-creator/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var tourPreferences = JsonConvert.DeserializeObject<List<TourPreferenceDto>>(responseBody);
                    return Ok(tourPreferences);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public ActionResult<PagedResult<TourPreferenceDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourPreferenceService.GetPaged(page, pageSize);
            return CreateResponse(result);
        }

    }

}
