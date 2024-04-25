using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.Core.Domain.Encounters;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain.Tours;
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
        private string microserviceUrl = "http://tours_server:8081";
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
        public async Task<ActionResult<TourDto>> Update([FromBody] TourDto tour, int id)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(tour);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{microserviceUrl}/tours/{id}", httpContent);

                // Check the response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var updatedTour = JsonConvert.DeserializeObject<TourDto>(responseContent);
                        return Ok(updatedTour);
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{microserviceUrl}/tours/{id}");

                // Check the response
                if (response.IsSuccessStatusCode)
                {
                    return NoContent(); // 204 No Content
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

        [HttpGet("by-author")]
        public async Task<ActionResult<List<TourDto>>> GetToursByAuthor([FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {

                // Get the JWT token from the authorization header
                var authorizationHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                {
                    // JWT token not found in the authorization header
                    return Unauthorized(); // Return 401 Unauthorized
                }

                var jwtToken = authorizationHeader.Substring("Bearer ".Length);

                // Parse the JWT token
                var jwtPayload = ParseJwtPayload(jwtToken);
                if (jwtPayload == null || !jwtPayload.ContainsKey("id"))
                {
                    // ID not found in the JWT payload
                    return Unauthorized(); // Return 401 Unauthorized
                }

                var userId = jwtPayload["id"].ToString();

                var url = $"{microserviceUrl}/tours/byauthor/{userId}";
                var response = await _httpClient.GetAsync(url);

                // Check the response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var tourList = JsonConvert.DeserializeObject<List<TourDto>>(responseContent);
                        return Ok(tourList);
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


        [HttpGet]
        public ActionResult<PagedResult<TourDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            return StatusCode(500);
        }

        [HttpPut("add/{tourId}/{equipmentId}")]
        public async Task<ActionResult<TourDto>> AddEquipment(int tourId, int equipmentId)
        {
            try
            {
                var requestBody = JsonConvert.SerializeObject(new { TourId = tourId, EquipmentId = equipmentId });
                var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{microserviceUrl}/tours/add/{tourId}/{equipmentId}", httpContent);

                // Check the response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var addedEquipment = JsonConvert.DeserializeObject<TourDto>(responseContent);
                    return Ok(addedEquipment);
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPut("remove/{tourId}/{equipmentId}")]
        public async Task<ActionResult<TourDto>> RemoveEquipment(int tourId, int equipmentId)
        {
            try
            {
                var requestBody = JsonConvert.SerializeObject(new { TourId = tourId, EquipmentId = equipmentId });
                var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{microserviceUrl}/tours/remove/{tourId}/{equipmentId}", httpContent);

                // Check the response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var removedEquipment = JsonConvert.DeserializeObject<TourDto>(responseContent);
                    return Ok(removedEquipment);
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("details/{id:int}")]
        public async Task<ActionResult<TourDto>> GetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{microserviceUrl}/tours/{id}");

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

        [HttpPut("publishedTours/{id:int}")]
        public ActionResult<TourDto> Publish(int id)
        {
            return StatusCode(500);
        }

        [HttpPut("archivedTours/{id:int}")]
        public ActionResult<TourDto> Archive(int id)
        {
            return StatusCode(500);
        }

        [HttpPut("tourTime/{id:int}")]
        public ActionResult<TourDto> AddTime(TourTimesDto tourTimesDto, int id)
        {
            return StatusCode(500);
        }

        private Dictionary<string, object> ParseJwtPayload(string jwtToken)
        {
            try
            {
                // Split the JWT token into header, payload, and signature
                var tokenParts = jwtToken.Split('.');
                if (tokenParts.Length != 3)
                {
                    return null; // Invalid JWT token format
                }

                // Decode the payload part
                var payloadBytes = Convert.FromBase64String(tokenParts[1]);
                var payloadJson = Encoding.UTF8.GetString(payloadBytes);

                // Deserialize the payload JSON into a dictionary
                var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadJson);
                return payload;
            }
            catch (Exception)
            {
                return null; // Error parsing JWT token
            }
        }

    }



}
