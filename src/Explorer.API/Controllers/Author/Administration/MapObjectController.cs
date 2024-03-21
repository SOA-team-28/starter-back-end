using Azure;
using Explorer.API.Services;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.Core.Domain.Encounters;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.UseCases.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Explorer.API.Controllers.Author.Administration
{
    [Authorize(Policy = "administratorAndAuthorPolicy")]
    [Route("api/administration/mapObject")]
    public class MapObjectController : BaseApiController
    {
        private readonly IMapObjectService _mapObjectService;
        private readonly ImageService _imageService;
        private readonly HttpClient _httpClient;

        public MapObjectController(IMapObjectService mapObjectService)
        {
            _mapObjectService = mapObjectService;
            _imageService = new ImageService();
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public async Task<ActionResult<List<MapObjectDto>>> GetAllAsync()
        {
            var microserviceUrl = "http://localhost:8081";

            try
            {
                var response = await _httpClient.GetAsync($"{microserviceUrl}/mapObjects-get-all");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var mapObjects = JsonConvert.DeserializeObject<List<MapObjectDto>>(responseBody);
                    return Ok(mapObjects);
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


        [HttpPost]
        public ActionResult<MapObjectDto> Create([FromBody] MapObjectDto mapObject)
        {
            var result = _mapObjectService.Create(mapObject);
            return CreateResponse(result);
        }

        [HttpPut("{id:int}")]
        public ActionResult<MapObjectDto> Update([FromForm] MapObjectDto mapObject, IFormFile picture = null)
        {
            if (picture != null)
            {
                var pictureUrl = _imageService.UploadImages(new List<IFormFile> { picture });
                mapObject.PictureURL = pictureUrl[0];
            }
            var result = _mapObjectService.Update(mapObject);
            return CreateResponse(result);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result = _mapObjectService.DeleteObjectAndRequest(id);
            return CreateResponse(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<MapObjectDto> GetObject(int id)
        {
            var result = _mapObjectService.Get(id);
            return CreateResponse(result);
        }

        [HttpPost("create/{userId:int}/{status}")]
        public async Task<ActionResult<MapObjectDto>> CreateAsync([FromForm] MapObjectDto mapObject, [FromRoute] int userId, [FromRoute] string status, IFormFile picture = null)
        {
            if (picture != null)
            {
                var pictureUrl = _imageService.UploadImages(new List<IFormFile> { picture });
                mapObject.PictureURL = pictureUrl[0];
            }
            //var result = _mapObjectService.Create(mapObject, userId, status);
            //return CreateResponse(result);

            var microserviceUrl = "http://localhost:8081";
            /*
            try
            {
                // Izvucite JWT token iz zaglavlja zahteva
                var jwt = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                // Kreirajte novi zahtev ka mikroservisu i postavite JWT token kao Authorization zaglavlje
                var request = new HttpRequestMessage(HttpMethod.Post, $"{microserviceUrl}/mapObjects/{userId}/{status}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                // Konvertujte mapObject u JSON format
                var jsonContent = JsonConvert.SerializeObject(mapObject);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Postavite sadržaj HTTP zahteva i pošaljite ga ka mikroservisu
                request.Content = httpContent;
                var response = await _httpClient.SendAsync(request);

                // Obrada odgovora od mikroservisa
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var createdMapObject = JsonConvert.DeserializeObject<MapObjectDto>(responseBody);
                    return Ok(createdMapObject);
                }
                else
                {
                    // Ako odgovor nije uspešan, vratite odgovarajući status kod
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            */
            
            try
            {
                var jsonContent = JsonConvert.SerializeObject(mapObject);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{microserviceUrl}/mapObjects/{userId}/{status}", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var createdMapObject = JsonConvert.DeserializeObject<MapObjectDto>(responseBody);
                    return Ok(createdMapObject);
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
    }
}
