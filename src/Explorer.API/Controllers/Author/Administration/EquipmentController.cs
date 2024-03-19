using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Explorer.API.Controllers.Author.Administration
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/manipulation/equipment")]
    public class EquipmentController : BaseApiController
    {
        private string microserviceUrl = "http://localhost:8081";
        private HttpClient _httpClient = new HttpClient();

        public EquipmentController(IEquipmentService equipmentService)
        {

        }

        [HttpPost("get-available/{tourId:int}")]
        public async Task<ActionResult<List<EquipmentDto>>> GetAvailableEquipment([FromBody] List<long> currentEquipmentIds, int tourId)
        {
            try
            {
                // Serialize the currentEquipmentIds into JSON
                var jsonContent = JsonConvert.SerializeObject(currentEquipmentIds);

                // Create HTTP request content with JSON content
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send HTTP POST request to the microservice
                var response = await _httpClient.PostAsync($"{microserviceUrl}/equipments/available/{tourId}", httpContent);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response content to EquipmentDto list
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var availableEquipment = JsonConvert.DeserializeObject<List<EquipmentDto>>(responseContent);

                    // Return the list of available equipment
                    return Ok(availableEquipment);
                }
                else
                {
                    // If the response is unsuccessful, return appropriate error status
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // Handle and log any exceptions that occur
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500);
            }
        }
    }

}
