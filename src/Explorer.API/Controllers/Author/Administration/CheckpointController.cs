using Explorer.API.Services;
using Explorer.Blog.Core.Domain.BlogPosts;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain.Tours;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Explorer.API.Controllers.Author.Administration
{
    [Route("api/administration/checkpoint")]
    public class CheckpointController : BaseApiController
    {
        private readonly ICheckpointService _checkpointService;
        private readonly ImageService _imageService;
        private string microserviceUrl = "http://localhost:8081";
        private HttpClient _httpClient = new HttpClient();

        public CheckpointController(ICheckpointService checkpointService)
        {
            _checkpointService = checkpointService;
            _imageService = new ImageService();
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "authorPolicy")]
        public ActionResult<List<CheckpointDto>> GetAllByTour([FromQuery] int page, [FromQuery] int pageSize, int id)
        {
            return StatusCode(500);
        }

        [HttpGet("details/{id:int}")]
        [Authorize(Policy = "authorPolicy")]
        public ActionResult<CheckpointDto> GetById(int id)
        {
            var result = _checkpointService.Get(id);
            return CreateResponse(result);
        }


        [HttpPut("{id:int}")]
        [Authorize(Policy = "authorPolicy")]
        public ActionResult<CheckpointDto> Update([FromForm] CheckpointDto checkpoint, int id, [FromForm] List<IFormFile>? pictures = null)
        {
            return StatusCode(500);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "authorPolicy")]
        public ActionResult Delete(int id)
        {
            var result = _checkpointService.Delete(id, User.PersonId());
            return CreateResponse(result);
        }

        [HttpPut("createSecret/{id:int}")]
        [Authorize(Policy = "authorPolicy")]
        public ActionResult<CheckpointDto> CreateCheckpointSecret([FromForm] CheckpointSecretDto secretDto, int id, [FromForm] List<IFormFile>? pictures = null)
        {
            return StatusCode(500);
        }

        [HttpPut("updateSecret/{id:int}")]
        [Authorize(Policy = "authorPolicy")]
        public ActionResult<CheckpointDto> UpdateCheckpointSecret([FromForm] CheckpointSecretDto secretDto, int id, [FromForm] List<IFormFile>? pictures = null)
        {
            return StatusCode(500);
        }

        [HttpPost("create/{status}")]
        [Authorize(Policy = "authorPolicy")]
        public async Task<ActionResult<CheckpointDto>> CreateAsync([FromForm] CheckpointDto checkpoint, [FromRoute] string status, [FromForm] List<IFormFile>? pictures = null)
        {
            if (pictures != null && pictures.Any())
            {
                var imageNames = _imageService.UploadImages(pictures);
                checkpoint.Pictures = imageNames;
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(checkpoint);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{microserviceUrl}/checkpoints", httpContent);

                // Check the response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var receivedTour = JsonConvert.DeserializeObject<CheckpointDto>(responseContent);
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

        [HttpGet]
        public ActionResult<PagedResult<CheckpointDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            return StatusCode(500);
        }
    }
}
