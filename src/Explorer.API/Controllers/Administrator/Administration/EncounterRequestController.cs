using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Explorer.Encounters.Core.Domain.Encounters;
using System.Text;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "touristAndAdministratorPolicy")]
    [Route("api/administration/encounterRequests")]
    public class EncounterRequestController : BaseApiController
    {
        private readonly IEncounterRequestService _encounterRequestService;
        private readonly HttpClient _httpClient;

        public EncounterRequestController(IEncounterRequestService encounterRequestService)
        {
            _encounterRequestService = encounterRequestService;
            _httpClient = new HttpClient();
        }

        [HttpPost]
        public ActionResult<EncounterRequestDto> Create([FromBody] EncounterRequestDto request)
        {
            var result = _encounterRequestService.Create(request);
            return CreateResponse(result);
        }

        [HttpGet]
        public async Task< ActionResult<List<EncounterRequestDto>>> GetAll()
        {
            /*
            var result = _encounterRequestService.GetPaged(0,0);
            return CreateResponse(result);
            */

            List<EncounterRequestDto> requests = new List<EncounterRequestDto>();
            var microserviceUrl = "http://encounters_server:8082";
            try
            {
                // Napravite HTTP GET zahtjev ka mikroservisu za dobavljanje encounter-a po ID-u
                var response = await _httpClient.GetAsync($"{microserviceUrl}/requests");

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    // Ukoliko je odgovor uspješan, izvucite podatke iz odgovora
                    var jsonString = await response.Content.ReadAsStringAsync();
                    requests = JsonConvert.DeserializeObject<List<EncounterRequestDto>>(jsonString);

                    return requests;
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return requests; // Vratite null ili neku drugu indikaciju da je dobavljanje encounter-a neuspješno
                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return requests; // Vratite null ili neku drugu indikaciju o grešci
            }
        }

        [HttpPut("accept/{id:int}")]
        public async Task<ActionResult<EncounterRequestDto>> AcceptRequest(int id)
        {
            /*
            var result = _encounterRequestService.AcceptRequest(id);
            return CreateResponse(result);
            */
            var microserviceUrl = "http://encounters_server:8082";

            try
            {
                

                // Pošaljite HTTP POST zahtjev ka Go mikroservisu
                var response = await _httpClient.PutAsync($"{microserviceUrl}/requests/accept/{id}",null);

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP status successfull {response.StatusCode}");
                    return null;
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return null;

                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;

            }
        }

        [HttpPut("reject/{id:int}")]
        public async Task<ActionResult<EncounterRequestDto>> RejectRequest(int id)
        {
            /*
            var result = _encounterRequestService.RejectRequest(id);
            return CreateResponse(result);
            */
            var microserviceUrl = "http://encounters_server:8082";

            try
            {


                // Pošaljite HTTP POST zahtjev ka Go mikroservisu
                var response = await _httpClient.PutAsync($"{microserviceUrl}/requests/reject/{id}", null);

                // Provjerite odgovor
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP status successfull {response.StatusCode}");
                    return null;
                }
                else
                {
                    // Ukoliko je odgovor neuspješan, obradite grešku na odgovarajući način
                    Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    return null;

                }
            }
            catch (Exception ex)
            {
                // Uhvatite i obradite izuzetak ako se desi
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;

            }
        }

    }
}
